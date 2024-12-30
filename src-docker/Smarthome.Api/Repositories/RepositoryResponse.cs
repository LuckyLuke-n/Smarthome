namespace Smarthome.Api.Repositories
{
	public class RepositoryResponse<TFail> where TFail : new()
	{
		public bool IsSuccess { get; private set; }
		public TFail ValueFail { get; private set; } = new();

		public static RepositoryResponse<TFail> CreateSuccess() => new() { IsSuccess = true };

		public static RepositoryResponse<TFail> CreateFail( TFail fail ) => new() { IsSuccess = false, ValueFail = fail };
	}

	public class RepositoryResponse<TSuccess, TFail> where TSuccess : class where TFail : new()
	{
		public bool IsSuccess { get; private set; }
		public TSuccess? ValueSuccess { get; private set; }
		public TFail ValueFail { get; private set; } = new();

		public static RepositoryResponse<TSuccess, TFail> CreateSuccess( TSuccess success )
		{
			RepositoryResponse<TSuccess, TFail> result = new()
			{
				IsSuccess = true,
				ValueSuccess = success
			};

			return result;
		}

		public static RepositoryResponse<TSuccess, TFail> CreateFail( TFail fail )
		{
			RepositoryResponse<TSuccess, TFail> result = new()
			{
				IsSuccess = false,
				ValueFail = fail
			};

			return result;
		}
	}
}