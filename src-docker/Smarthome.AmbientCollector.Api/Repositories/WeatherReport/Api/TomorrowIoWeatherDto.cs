namespace Smarthome.AmbientCollector.Api.Repositories.WeatherReport.Api
{
	[Serializable]
	public class TomorrowIoWeatherDto
	{
		public Data? Data { get; set; }
		public Location? Location { get; set; }

		public WeatherReport ToWeatherReport()
		{
			WeatherReport weatherReport = new()
			{
				Location = Location?.Name ?? "N/A",
				Latitude = Location?.Lat ?? 0,
				Longitude = Location?.Lon ?? 0,
				Temperature = Data?.Values?.Temperature ?? 0,
				TemperatureApparent = Data?.Values?.TemperatureApparent ?? 0,
				Humidity = Data?.Values?.Humidity ?? 0,
				RainIntensity = Data?.Values?.RainIntensity ?? 0,
				PressureSurfaceLevel = Data?.Values?.PressureSurfaceLevel ?? 0,
				PressureSeaLevel = Data?.Values?.PressureSeaLevel ?? 0,
				WindSpeed = Data?.Values?.WindSpeed ?? 0,
			};

			return weatherReport;
		}
	}

	[Serializable]
	public class Data
	{
		public DateTime Time { get; set; }
		public Values? Values { get; set; }
	}

	[Serializable]
	public class Values
	{
		//public double? CloudBase { get; set; }
		//public double? CloudCeiling { get; set; }
		//public int CloudCover { get; set; }
		//public double DewPoint { get; set; }
		//public double FreezingRainIntensity { get; set; }
		//public double HailProbability { get; set; }
		//public double HailSize { get; set; }
		public double Humidity { get; set; }
		// public int PrecipitationProbability { get; set; }
		public double PressureSurfaceLevel { get; set; }
		public double PressureSeaLevel { get; set; }
		public double RainIntensity { get; set; }
		//public double SleetIntensity { get; set; }
		//public double SnowIntensity { get; set; }
		public double Temperature { get; set; }
		public double TemperatureApparent { get; set; }
		//public double UvHealthConcern { get; set; }
		//public double UvIndex { get; set; }
		//public double Visibility { get; set; }
		//public int WeatherCode { get; set; }
		//public double? WindDirection { get; set; }
		//public double WindGust { get; set; }
		public double WindSpeed { get; set; }
	}

	[Serializable]
	public class Location
	{
		public double Lat { get; set; }
		public double Lon { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
	}
}
