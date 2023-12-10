namespace MMBot.Blazor.Server.Middleware;

public class ExceptionHandlingMiddleware(ILoggerFactory loggerFactory) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var logger = loggerFactory.CreateLogger(context.GetEndpoint().DisplayName);

        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong...");
        }
    }
}
