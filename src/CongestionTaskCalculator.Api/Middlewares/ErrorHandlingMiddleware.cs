public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = Guid.NewGuid().ToString();
        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{correlationId}] {ex.Message}");
            httpContext.Response.StatusCode = 500;
            await Results.Problem(
                detail: ex.Message,
                title: "Internal Server Error",
                statusCode: 500
            ).ExecuteAsync(httpContext);
        }
    }
}