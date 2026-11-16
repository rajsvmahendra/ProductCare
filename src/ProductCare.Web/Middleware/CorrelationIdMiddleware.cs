using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProductCare.Web.Middleware;

/// <summary>
/// Intercepts inbound HTTP requests and assigns or extracts a unique correlation identifier.
/// Populates response headers and enriches logging scopes for end-to-end request tracing.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public const string CorrelationHeaderKey = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationHeaderKey].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("D");
        }

        context.TraceIdentifier = correlationId;

        // Add correlation header to response
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationHeaderKey))
            {
                context.Response.Headers.Append(CorrelationHeaderKey, correlationId);
            }
            return Task.CompletedTask;
        });

        // Enrich logger context with correlation identifier
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }
}
