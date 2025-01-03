namespace Smarthome.Api.Middleware
{
	public class ApiKeyMiddleware
	{
		private readonly RequestDelegate _next;
		private string ApiKeyName { get; } = "X-Api-Key";
		private string ApiKeyValue { get; }

		public ApiKeyMiddleware( RequestDelegate next )
		{
			_next = next;
			ApiKeyValue = Environment.GetEnvironmentVariable( "SMARTHOME_Api__Key" ) ?? throw new InvalidOperationException( "ApiKey must be set in environment varaible  SMARTHOME_Api__Key" );
		}

		public async Task Invoke( HttpContext context )
		{
			if ( !context.Request.Headers.TryGetValue( ApiKeyName, out var extractedApiKey ) )
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync( "Unauthorized client. Proivde an Api Key." );
				return;
			}

			if ( !ApiKeyValue.Equals( extractedApiKey ) )
			{
				context.Response.StatusCode = 403;
				await context.Response.WriteAsync( "Forbidden client. Povide a valid Api Key." );
				return;
			}

			await _next( context );
		}
	}
}
