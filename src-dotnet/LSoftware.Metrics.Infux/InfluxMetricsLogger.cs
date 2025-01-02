using InfluxDB.Client;
using LSoftware.Metrics.Abstractions;
using Microsoft.Extensions.Logging;

namespace LSoftware.Metrics.Infux
{
	public class InfluxMetricsLogger<T> : IMetricsLogger<T> where T : IMetricsObject
	{
		private bool _disposedValue;

		private InfluxDBClient Client { get; }
		private ILogger<InfluxMetricsLogger<T>> Logger { get; }

		public InfluxMetricsLogger( InfluxDBClient client, ILogger<InfluxMetricsLogger<T>> logger )
		{
			Client = client;
			Logger = logger;
		}

		public void Send( T data )
		{
			try
			{
				using var writeApi = Client.GetWriteApi();
				writeApi.WritePoint( data.ToInfluxDbDataPoint() );
			}
			catch ( Exception ex )
			{
				Logger.LogWarning( ex, "Cannot write to influx db." );
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !_disposedValue )
			{
				if ( disposing )
				{
					Client.Dispose();
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
