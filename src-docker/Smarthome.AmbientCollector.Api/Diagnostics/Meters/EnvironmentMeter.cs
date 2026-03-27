using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Smarthome.AmbientCollector.Api.Diagnostics.Meters
{
    public static class EnvironmentMeter
    {
        public static string Name => "EnvironmentSensor.Readings";
        private static readonly Meter Meter = new(Name, "1.0");

        private static readonly ConcurrentDictionary<string, Measurement<double>> TemperatureMeasurements = [];
        private static readonly ConcurrentDictionary<string, Measurement<double>> HumidityMeasurements = [];
        private static readonly ConcurrentDictionary<string, Measurement<double>> PressureMeasurements = [];

        private static readonly ObservableGauge<double> TemperatureGauge;
        private static readonly ObservableGauge<double> HumidityGauge;
        private static readonly ObservableGauge<double> PressureGauge;

        static EnvironmentMeter()
        {
            // Initialize the metrics
            TemperatureGauge = Meter.CreateObservableGauge<double>(
                "envsensor.temperature",
                () => TemperatureMeasurements.Values,
                "degC",
                "Current temperature in Celsius");
            
            
            HumidityGauge = Meter.CreateObservableGauge<double>(
                "envsensor.humidity",
                () => HumidityMeasurements.Values,
                "%",
                "Current humidity in %");
            
            PressureGauge = Meter.CreateObservableGauge<double>(
                "envsensor.pressure",
                () => PressureMeasurements.Values,
                "hPa",
                "Current ambient pressure in hPa");
        }

        public static void Update( double temperature, double humidity, double pressure, string location, string sensorModel )
        {
            var tags = new KeyValuePair<string, object?>[]
            {
                new("sensor.location", location),
                new("sensor.model", sensorModel),
            };

            TemperatureMeasurements.TryAdd(location, new Measurement<double>(temperature, tags));
            HumidityMeasurements.TryAdd(location, new Measurement<double>(humidity, tags));
            PressureMeasurements.TryAdd(location, new Measurement<double>(pressure, tags));
        }

        public static void TryRemoveSensor(string location)
        {
            TemperatureMeasurements.TryRemove(location, out _);
            HumidityMeasurements.TryRemove(location, out _);
            PressureMeasurements.TryRemove(location, out _);
        }
    }
}