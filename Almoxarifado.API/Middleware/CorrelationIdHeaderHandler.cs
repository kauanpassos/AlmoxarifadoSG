using Almoxarifado.API.Configuration;

namespace Almoxarifado.API.Middleware;

public sealed class CorrelationIdHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?.Items[HttpConstants.CorrelationIdHeader]?.ToString();

        if (!string.IsNullOrWhiteSpace(correlationId) && !request.Headers.Contains(HttpConstants.CorrelationIdHeader))
        {
            request.Headers.Add(HttpConstants.CorrelationIdHeader, correlationId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
