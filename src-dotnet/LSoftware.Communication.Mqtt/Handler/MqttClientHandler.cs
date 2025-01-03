using LSoftware.Communication.Abstractions.MessageBus;
using LSoftware.Communication.Mqtt.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Formatter;
using System.Text;

namespace LSoftware.Communication.Mqtt.Handler
{
	public class MqttClientHandler : ISubscriber, IDisposable
	{
		public IMqttClient? MqttClient { get; set; }
		public string Topic { get; private set; } = string.Empty;


		private MqttConfiguration MqttConfiguration { get; }
		private ILogger<MqttClientHandler> Logger { get; }
		private bool IsConnected { get; set; }
		private MqttClientFactory MqttFactory { get; }
		private CancellationTokenSource CancellationTokenSource { get; } = new();

		private Action<string, string>? MessageReceived { get; set; }

		private int _usageCount;
		private bool _disposedValue;

		public MqttClientHandler( IOptions<MqttConfiguration> mqttOptions, ILogger<MqttClientHandler> logger )
		{
			MqttConfiguration = mqttOptions.Value;
			Logger = logger;

			MqttFactory = new MqttClientFactory();
		}

		internal void IncreaseCount() => Interlocked.Increment( ref _usageCount );

		internal void DecreaseCount() => Interlocked.Decrement( ref _usageCount );

		/// <summary>
		/// Connects the client to the broker. Creates the <see cref="CancellationTokenSource"/>.
		/// </summary>
		public async Task ConnectAsync()
		{
			CancellationTokenSource.Token.ThrowIfCancellationRequested();

			MqttClient = MqttFactory.CreateMqttClient();

			// Create MQTT client options
			var mqttClientOptions = new MqttClientOptionsBuilder()
				.WithTcpServer( MqttConfiguration.Host, int.Parse( MqttConfiguration.Port ) )
				.WithCredentials( MqttConfiguration.Username, MqttConfiguration.Password ) // Set username and password
				.WithClientId( $"Smarthome.Api-{Guid.NewGuid()}" )
				.WithProtocolVersion( MqttProtocolVersion.V311 )
				.WithCleanSession();

			if ( MqttConfiguration.TlsEnabled )
				mqttClientOptions.WithTlsOptions( o =>
				{
					o.UseTls();
				} );


			CancellationTokenSource.Token.ThrowIfCancellationRequested();

			MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
			var result = await MqttClient.ConnectAsync( mqttClientOptions.Build(), CancellationTokenSource.Token ).ConfigureAwait( false );

			if ( result.ResultCode != MqttClientConnectResultCode.Success )
				Logger.LogError( "No connection made to mqtt broker {Host}.", MqttConfiguration.Host );
			else
				IsConnected = true;
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