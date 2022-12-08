namespace MMBot.Blazor.Server.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILoggerFactory _loggerFactory;

    public ExceptionHandlingMiddleware(ILoggerFactory loggerFactory)
        => _loggerFactory = loggerFactory;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var logger = _loggerFactory.CreateLogger(context.GetEndpoint().DisplayName);

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
