namespace BizFirst.Integration.Hedera.Services;

/// <summary>
/// Free, eventually-consistent Mirror Node REST client. Resolves the per-network base URL from
/// <see cref="HederaNetworkOptions"/> and performs GET requests via a rate-limit-aware typed
/// HttpClient (Guideline 04 Flavour B — see <see cref="HederaMirrorRateLimitHandler"/>). Returns the
/// raw status + body so services can map a 404 to <c>MIRROR_NOT_FOUND</c> (read-after-write ingestion
/// lag) and parse defensively. Thread-safe: registered as a singleton over <see cref="IHttpClientFactory"/>.
/// </summary>
public sealed class HederaMirrorClient
{
    public const string HttpClientName = "HederaMirror";

    private readonly IHttpClientFactory _httpFactory;
    private readonly HederaNetworkOptions _options;
    private readonly ILogger<HederaMirrorClient> _logger;

    public HederaMirrorClient(
        IHttpClientFactory httpFactory,
        IOptions<HederaNetworkOptions> options,
        ILogger<HederaMirrorClient> logger)
    {
        _httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>Resolves the mirror base URL for a network name (falls back to the configured default).</summary>
    public string ResolveBaseUrl(string? network)
    {
        var name = string.IsNullOrWhiteSpace(network) ? _options.DefaultNetwork : network;
        if (!_options.Networks.TryGetValue(name, out var cfg) || string.IsNullOrWhiteSpace(cfg.MirrorRestUrl))
            throw new HederaNetworkNotConfiguredException(name);
        return cfg.MirrorRestUrl.TrimEnd('/');
    }

    /// <summary>
    /// GET a mirror-node REST path (e.g. "/api/v1/accounts/0.0.1234"). Never throws on a non-success
    /// HTTP status (including 404). Transport faults (DNS/connection) and timeouts surface as exceptions
    /// for the caller to classify (real cancellation vs. request timeout).
    /// </summary>
    public async Task<HederaMirrorResponse> GetAsync(string? network, string relativePath, CancellationToken ct)
    {
        var url = ResolveBaseUrl(network) + relativePath;
        var client = _httpFactory.CreateClient(HttpClientName);

        _logger.LogDebug("[HederaMirror] GET {Url}", url);
        using var resp = await client.GetAsync(url, ct).ConfigureAwait(false);
        var body = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (!resp.IsSuccessStatusCode && resp.StatusCode != HttpStatusCode.NotFound)
            _logger.LogWarning("[HederaMirror] GET {Url} returned HTTP {Status}", url, (int)resp.StatusCode);

        return new HederaMirrorResponse((int)resp.StatusCode, resp.IsSuccessStatusCode, body);
    }
}

/// <summary>Raw mirror-node HTTP response (status + body) for defensive parsing by services.</summary>
public readonly record struct HederaMirrorResponse(int StatusCode, bool IsSuccess, string Body)
{
    public bool IsNotFound => StatusCode == (int)HttpStatusCode.NotFound;
}
