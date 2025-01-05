namespace Smarthome.Api.Repositories.WeatherReport
{
	public class WeatherRepositoryResponse<TFail> where TFail : new()
	{
		public bool IsSuccess { get; private set; }
		public TFail ValueFail { get; private set; } = new();

		public static WeatherRepositoryResponse<TFail> CreateSuccess() => new() { IsSuccess = true };

		public static WeatherRepositoryResponse<TFail> CreateFail( TFail fail ) => new() { IsSuccess = false, ValueFail = fail };
	}

	public class WeatherRepositoryResponse<TSuccess, TFail> where TSuccess : class where TFail : new()
	{
		public bool IsSuccess { get; private set; }
		public TSuccess? ValueSuccess { get; private set; }
		public TFail ValueFail { get; private set; } = new();

		public static WeatherRepositoryResponse<TSuccess, TFail> CreateSuccess( TSuccess success )
		{
			WeatherRepositoryResponse<TSuccess, TFail> result = new()
			{
				IsSuccess = true,
				ValueSuccess = success
			};

			return result;
		}

		public static WeatherRepositoryResponse<TSuccess, TFail> CreateFail( TFail fail )
		{
			WeatherRepositoryResponse<TSuccess, TFail> result = new()
			{
				IsSuccess = false,
				ValueFail = fail
			};

			return result;
		}
	}
}
