using Almoxarifado.API.Configuration;
using Serilog.Context;

namespace Almoxarifado.API.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrGenerateCorrelationId(context);

        context.Items[HttpConstants.CorrelationIdHeader] = correlationId;
        
        AddCorrelationIdToResponse(context, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HttpConstants.CorrelationIdHeader, out var correlationId) && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId!;
        }

        return Guid.NewGuid().ToString();
    }

    private static void AddCorrelationIdToResponse(HttpContext context, string correlationId)
    {
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(HttpConstants.CorrelationIdHeader))
            {
                context.Response.Headers.Append(HttpConstants.CorrelationIdHeader, correlationId);
            }
            return Task.CompletedTask;
        });
    }
}
