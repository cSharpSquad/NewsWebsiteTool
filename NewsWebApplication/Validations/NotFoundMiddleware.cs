namespace Validations
{
	public class NotFoundMiddleware
	{
		private readonly RequestDelegate next;
		public NotFoundMiddleware(RequestDelegate next)
		{
			this.next = next;
		}
		public async Task Invoke(HttpContext context)
		{
			await next(context);
			if (context.Response.StatusCode == StatusCodes.Status404NotFound)
			{
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync("{\"error\": \"Resource not found\"}");
			}
		}
	}
}
