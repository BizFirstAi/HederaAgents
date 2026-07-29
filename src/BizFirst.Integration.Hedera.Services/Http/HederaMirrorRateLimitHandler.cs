namespace BizFirst.Integration.Hedera.Services;

/// <summary>
/// <see cref="DelegatingHandler"/> that retries transient Mirror Node REST failures — HTTP 429
/// (Too Many Requests; the public mirror nodes enforce per-IP rate budgets) and 502/503/504 (gateway/
/// unavailable). Honours a <c>Retry-After</c> header when present, otherwise backs off linearly.
/// Mirrors the Ethereum/Slack rate-limit-handler pattern (Guideline 04, Flavour B).
/// </summary>
public sealed class HederaMirrorRateLimitHandler : DelegatingHandler
{
    private const int MaxRetries = 3;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null!;

        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!IsTransient(response.StatusCode))
                return response;

            if (attempt == MaxRetries)
                break;

            var delay = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(2 * (attempt + 1));
            response.Dispose();
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private static bool IsTransient(HttpStatusCode status) => status switch
    {
        HttpStatusCode.TooManyRequests => true,   // 429
        HttpStatusCode.BadGateway => true,        // 502
        HttpStatusCode.ServiceUnavailable => true,// 503
        HttpStatusCode.GatewayTimeout => true,    // 504
        _ => false,
    };
}
