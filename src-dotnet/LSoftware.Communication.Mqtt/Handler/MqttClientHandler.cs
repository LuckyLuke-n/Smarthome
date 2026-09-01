using System.Text;
using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using LSoftware.Communication.Mqtt.Diagnostics.Meters;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Formatter;

namespace LSoftware.Communication.Mqtt.Handler
{
	public class MqttClientHandler : ISubscriber, IAsyncDisposable
	{
		public IMqttClient? MqttClient { get; set; }
		public string Topic { get; private set; } = string.Empty;
		
		private string ConnectionString { get; }
		private ILogger<MqttClientHandler> Logger { get; }
		private bool IsConnected { get; set; }
		private MqttClientFactory MqttFactory { get; }
		private CancellationTokenSource CancellationTokenSource { get; } = new();
		private string MqttHost { get; set; } = string.Empty;

		private Action<string, string>? MessageReceived { get; set; }

		private readonly SemaphoreSlim _connectionSemaphore = new( 1, 1 );
		private string? _clientId;
		private int _usageCount;
		private bool _disposedValue;
		private int _isReconnecting;

		public MqttClientHandler( string connectionString, ILogger<MqttClientHandler> logger )
		{
			ConnectionString = connectionString;
			Logger = logger;
			MqttFactory = new MqttClientFactory();
		}

		internal void IncreaseCount() => Interlocked.Increment( ref _usageCount );

		internal void DecreaseCount() => Interlocked.Decrement( ref _usageCount );

		/// <summary>
		/// Connects the client to the broker. Creates the <see cref="CancellationTokenSource"/>.
		/// <returns>Returns true if the connection was established. Otherwise false.</returns>
		/// </summary>
		public async Task<bool> ConnectAsync()
		{
			await _connectionSemaphore.WaitAsync( CancellationTokenSource.Token ).ConfigureAwait( false );
			try
			{
				if ( MqttClient is not null && MqttClient.IsConnected )
				{
					return true;
				}

				MqttConfiguration mqttConfiguration = new();
				if ( MqttConfiguration.TryCreate( ConnectionString, out var configuration ) )
					mqttConfiguration = configuration;
				else
				{
					Logger.LogError( "Invalid or missing connection string for mqtt." );
					return false;
				}

				if ( MqttClient is null )
				{
					MqttClient = MqttFactory.CreateMqttClient();
					MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
					MqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
				}

				_clientId ??= $"Smarthome.Api-{Guid.NewGuid()}";

				// Create MQTT client options
				MqttHost = mqttConfiguration.Host + ":" + mqttConfiguration.Port;
				var mqttClientOptions = new MqttClientOptionsBuilder()
					.WithTcpServer( mqttConfiguration.Host, mqttConfiguration.Port )
					.WithCredentials( mqttConfiguration.Username, mqttConfiguration.Password ) // Set username and password
					.WithClientId( _clientId )
					.WithProtocolVersion( MqttProtocolVersion.V311 )
					.WithCleanSession( false );

				if ( mqttConfiguration.TlsEnabled )
					mqttClientOptions.WithTlsOptions( o => { o.UseTls(); } );

				using var timeoutCts = new CancellationTokenSource( TimeSpan.FromSeconds( 30 ) );
				using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource( CancellationTokenSource.Token, timeoutCts.Token );

				var result = await MqttClient.ConnectAsync( mqttClientOptions.Build(), linkedCts.Token ).ConfigureAwait( false );

				if ( result.ResultCode != MqttClientConnectResultCode.Success )
				{
					Logger.LogError( "No connection made to mqtt broker '{Host}' with reason {NoConnectionReason}.", mqttConfiguration.Host, result.ResultCode );
					return false;
				}

				IsConnected = true;

				if ( !string.IsNullOrWhiteSpace( Topic ) )
					// during a reconnect the topic will already be set and can be resubscribed to
					await SubscribeAsync( Topic ).ConfigureAwait( false );

				return true;
			}
			catch ( Exception ex )
			{
				Logger.LogError( ex, "Error while connecting to mqtt broker '{Host}'.", MqttHost );
				return false;
			}
			finally
			{
				_connectionSemaphore.Release();
			}
		}

		public void RegisterCallback( Action<string, string> callback ) => MessageReceived = callback;

		internal async Task SubscribeAsync( string topic )
		{
			if ( !IsConnected || MqttClient is null )
			{
				Logger.LogWarning( "Cannot subscribe to {Topic}. Client not connected.", topic );
				return;
			}

			Topic = topic;

			var mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
				.WithTopicFilter( Topic )
				.Build();

			await MqttClient.SubscribeAsync( mqttSubscribeOptions, CancellationTokenSource.Token ).ConfigureAwait( false );
		}

		private Task MqttClient_DisconnectedAsync( MqttClientDisconnectedEventArgs arg )
		{
			IsConnected = false;
			if ( arg.ClientWasConnected )
				_ = StartReconnectLoop();

			return Task.CompletedTask;
		}

		private async Task MqttClient_ApplicationMessageReceivedAsync( MqttApplicationMessageReceivedEventArgs arg )
		{
			if ( _disposedValue )
				return;

			var payload = arg.ApplicationMessage.Payload;

			if ( payload.Length == 0 )
			{
				await Task.CompletedTask.ConfigureAwait( false );
				return;
			}

			StringBuilder sb = new();
			foreach ( var segment in payload )
				sb.Append( Encoding.UTF8.GetString( segment.Span ) );

			MessageReceived?.Invoke( Topic, sb.ToString() );
		}

		/// <summary>
		/// If the <see cref="_usageCount"/> is 0 the client will be disposed.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> TryDestroyAsync()
		{
			if ( _disposedValue )
				return false;

			if ( _usageCount > 0 )
				return false;

			await DisposeAsync().ConfigureAwait(false);
			return true;
		}

		private async Task StartReconnectLoop()
		{
			if ( Interlocked.CompareExchange( ref _isReconnecting, 1, 0 ) != 0 )
				return;

			try
			{
				Logger.LogWarning( "Starting reconnection loop for MQTT client on host '{Host}'.", MqttHost );

				int retries = 0;
				int maxRetries = 100;

				while ( !CancellationTokenSource.Token.IsCancellationRequested )
				{
					if ( retries == maxRetries )
					{
						Logger.LogError( "Maximum retry count of 100 exhausted on mqtt broker on host '{Host}'.", MqttHost );
						return;
					}

					retries++;

					MqttBrokerMeter.ReconnectAttempt( MqttHost );
					var success = await ConnectAsync().ConfigureAwait( false );

					if ( success )
					{
						Logger.LogInformation( "Successfully connected to mqtt broker on host '{Host}'.", MqttHost );
						return;
					}

					// some kind of jitter for random wait between 5 and 30 seconds
					var random = new Random();
					var waitTimeInMilliseconds = random.Next( 5, 30 ) * 1000;

					await Task.Delay( waitTimeInMilliseconds, CancellationTokenSource.Token );
				}
			}
			finally
			{
				_isReconnecting = 0;
			}
		}

		public async ValueTask DisposeAsync()
		{
			if ( !_disposedValue )
			{
				if ( MqttClient is not null )
				{
					MqttClient.ApplicationMessageReceivedAsync -= MqttClient_ApplicationMessageReceivedAsync;
					MqttClient.DisconnectedAsync -= MqttClient_DisconnectedAsync;

					if ( !string.IsNullOrWhiteSpace( Topic ) )
						await MqttClient.UnsubscribeAsync( Topic, CancellationTokenSource.Token ).ConfigureAwait( false );
					await MqttClient.DisconnectAsync().ConfigureAwait( false );
					MqttClient.Dispose();
				}

				CancellationTokenSource.Cancel();
				_connectionSemaphore.Dispose();

				_disposedValue = true;
			}
		}
	}
}