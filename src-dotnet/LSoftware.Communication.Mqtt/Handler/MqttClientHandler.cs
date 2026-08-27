using System.Text;
using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Formatter;

namespace LSoftware.Communication.Mqtt.Handler
{
	public class MqttClientHandler : ISubscriber, IDisposable
	{
		public IMqttClient? MqttClient { get; set; }
		public string Topic { get; private set; } = string.Empty;
		
		private string ConnectionString { get; }
		private ILogger<MqttClientHandler> Logger { get; }
		private bool IsConnected { get; set; }
		private MqttClientFactory MqttFactory { get; }
		private CancellationTokenSource CancellationTokenSource { get; } = new();

		private Action<string, string>? MessageReceived { get; set; }

		private int _usageCount;
		private bool _disposedValue;

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
			CancellationTokenSource.Token.ThrowIfCancellationRequested();

			MqttConfiguration mqttConfiguration = new();
			if ( MqttConfiguration.TryCreate( ConnectionString, out var configuration ) )
				mqttConfiguration = configuration;
			else
			{
				Logger.LogError( "Invalid or missing connection string for mqtt." );
				return false;
			}

			MqttClient = MqttFactory.CreateMqttClient();

			// Create MQTT client options
			var mqttClientOptions = new MqttClientOptionsBuilder()
				.WithTcpServer( mqttConfiguration.Host, mqttConfiguration.Port )
				.WithCredentials( mqttConfiguration.Username, mqttConfiguration.Password ) // Set username and password
				.WithClientId( $"Smarthome.Api-{Guid.NewGuid()}" )
				.WithProtocolVersion( MqttProtocolVersion.V311 )
				.WithCleanSession();

			if ( mqttConfiguration.TlsEnabled )
				mqttClientOptions.WithTlsOptions( o =>
				{
					o.UseTls();
				} );


			CancellationTokenSource.Token.ThrowIfCancellationRequested();

			MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
			var result = await MqttClient.ConnectAsync( mqttClientOptions.Build(), CancellationTokenSource.Token ).ConfigureAwait( false );

			if (result.ResultCode != MqttClientConnectResultCode.Success)
			{
				Logger.LogError( "No connection made to mqtt broker '{Host}' with reason {NoConnectionReason}.", mqttConfiguration.Host, result.ResultCode );
				return false;
			}

			IsConnected = true;
			return true;
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
			MqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

		}

		private async Task MqttClient_DisconnectedAsync( MqttClientDisconnectedEventArgs arg )
		{
			if ( arg.ClientWasConnected )
			{
				await ConnectAsync().ConfigureAwait( false );
			}
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
		public bool TryDispose()
		{
			if ( _disposedValue )
				return false;

			if ( _usageCount > 0 )
				return false;

			Dispose();
			return true;
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !_disposedValue )
			{
				if ( disposing )
				{
					if ( MqttClient is not null )
					{
						MqttClient.DisconnectedAsync -= MqttClient_DisconnectedAsync;
						if ( !string.IsNullOrWhiteSpace( Topic ) )
							MqttClient.UnsubscribeAsync( Topic, CancellationTokenSource.Token ).GetAwaiter().GetResult();
						MqttClient.DisconnectAsync();
					}

					CancellationTokenSource.Cancel();
				}

				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}