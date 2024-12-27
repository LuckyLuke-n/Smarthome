namespace Smarthome.Api.Repositories
{
	public class RepositoryResponse<TSuccess, TFail> where TSuccess : new() where TFail : new()
	{
		public bool IsSuccess { get; private set; }
		private TSuccess ValueSuccess { get; set; } = new();
		private TFail ValueFail { get; set; } = new();

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

		public void Resolve( Action<TSuccess> actionSuccess, Action<TFail> actionFail )
		{
			if ( IsSuccess )
				actionSuccess.Invoke( ValueSuccess );
			else
				actionFail.Invoke( ValueFail );
		}
	}
}