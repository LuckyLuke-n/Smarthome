using InfluxDB.Client;
using InfluxDB.Client.Writes;
using LSoftware.Metrics.Abstractions;
using LSoftware.Metrics.InfluxDb.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LSoftware.Metrics.InfluxDb
{
	public class InfluxMetricsLogger<T> : IMetricsLogger<T> where T : IMetricsObject
	{
		private bool _disposedValue;

		private InfluxDBClient Client { get; }
		private ILogger<InfluxMetricsLogger<T>> Logger { get; }

		private int BufferSize { get; }
		private Queue<PointData> WriteBuffer { get; } = [];

		public InfluxMetricsLogger( InfluxDBClient client, ILogger<InfluxMetricsLogger<T>> logger, IOptions<InfluxDbConfiguration> influxOptions )
		{
			Client = client;
			Logger = logger;
			BufferSize = influxOptions.Value.CacheSize;
		}

		public void Send( T data )
		{
			WriteBuffer.Enqueue( data.ToInfluxDbDataPoint() );

			if ( WriteBuffer.Count < BufferSize )
				return;

			try
			{
				using var writeApi = Client.GetWriteApi();
				writeApi.WritePoints( WriteBuffer.ToArray() );
				WriteBuffer.Clear();
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
