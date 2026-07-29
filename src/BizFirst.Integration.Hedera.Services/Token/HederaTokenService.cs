namespace BizFirst.Integration.Hedera.Services;

/// <summary>Token (HTS) reads via the Mirror Node REST API (TOK08 info, TOK09 account holdings).</summary>
public sealed class HederaTokenService
{
    private readonly HederaMirrorClient _mirror;
    private readonly ILogger<HederaTokenService> _logger;

    public HederaTokenService(HederaMirrorClient mirror, ILogger<HederaTokenService> logger)
    {
        _mirror = mirror ?? throw new ArgumentNullException(nameof(mirror));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>TOK08 — token metadata from GET /api/v1/tokens/{id}.</summary>
    public async Task<HederaTokenInfoResult> GetTokenInfoAsync(string? network, string tokenId, CancellationToken ct)
    {
        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, $"/api/v1/tokens/{Uri.EscapeDataString(tokenId)}", ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaToken] getInfo timed out for {TokenId}", tokenId);
            return HederaTokenInfoResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaTokenInfoResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaToken] getInfo transport failure for {TokenId}", tokenId);
            return HederaTokenInfoResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaTokenInfoResult.Fail("MIRROR_NOT_FOUND", $"Token {tokenId} not found on the mirror node.");
        if (!resp.IsSuccess) return HederaTokenInfoResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            var root = doc.RootElement;
            return HederaTokenInfoResult.Ok(
                root.TryGetProperty("token_id", out var idp) ? idp.GetString() ?? tokenId : tokenId,
                root.TryGetProperty("name", out var n) ? n.GetString() ?? string.Empty : string.Empty,
                root.TryGetProperty("symbol", out var s) ? s.GetString() ?? string.Empty : string.Empty,
                root.TryGetProperty("decimals", out var dec) ? HederaJson.GetIntLoose(dec) : 0,
                root.TryGetProperty("total_supply", out var ts) ? HederaJson.GetStringLoose(ts) : "0",
                root.TryGetProperty("type", out var tp) ? tp.GetString() ?? string.Empty : string.Empty,
                root.TryGetProperty("treasury_account_id", out var tr) ? tr.GetString() : null,
                root.TryGetProperty("deleted", out var d) && d.ValueKind == JsonValueKind.True);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaToken] getInfo parse failure for {TokenId}", tokenId);
            return HederaTokenInfoResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }

    /// <summary>TOK09 — account token holdings from GET /api/v1/accounts/{id}/tokens.</summary>
    public async Task<HederaTokenBalancesResult> GetAccountTokenBalancesAsync(string? network, string accountId, int limit, CancellationToken ct)
    {
        var clamped = limit is < 1 or > 100 ? 25 : limit;
        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, $"/api/v1/accounts/{Uri.EscapeDataString(accountId)}/tokens?limit={clamped}", ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaToken] getBalances timed out for {AccountId}", accountId);
            return HederaTokenBalancesResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaTokenBalancesResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaToken] getBalances transport failure for {AccountId}", accountId);
            return HederaTokenBalancesResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaTokenBalancesResult.Fail("MIRROR_NOT_FOUND", $"Account {accountId} not found on the mirror node.");
        if (!resp.IsSuccess) return HederaTokenBalancesResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            var tokens = new List<HederaTokenBalance>();
            if (doc.RootElement.TryGetProperty("tokens", out var tks) && tks.ValueKind == JsonValueKind.Array)
                foreach (var t in tks.EnumerateArray())
                    tokens.Add(new HederaTokenBalance(
                        t.TryGetProperty("token_id", out var idp) ? idp.GetString() ?? string.Empty : string.Empty,
                        t.TryGetProperty("balance", out var bp) ? HederaJson.GetLongLoose(bp) : 0,
                        t.TryGetProperty("decimals", out var dp) ? HederaJson.GetIntLoose(dp) : 0));
            return HederaTokenBalancesResult.Ok(accountId, tokens);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaToken] getBalances parse failure for {AccountId}", accountId);
            return HederaTokenBalancesResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }
}
