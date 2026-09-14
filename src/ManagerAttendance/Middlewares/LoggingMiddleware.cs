using System.Diagnostics;

namespace ManagerAttendance.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        _logger.LogInformation("HTTP Request Incoming: {Method} {Path}", request.Method, request.Path);

        await _next(context);

        stopwatch.Stop();
        var response = context.Response;

        _logger.LogInformation("HTTP Response Completed: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
            request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
