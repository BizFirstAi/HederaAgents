namespace BizFirst.Integration.Hedera.Services;

/// <summary>Account reads via the Mirror Node REST API (ACC02 balance, ACC03 info). No signature required.</summary>
public sealed class HederaAccountService
{
    private readonly HederaMirrorClient _mirror;
    private readonly ILogger<HederaAccountService> _logger;

    public HederaAccountService(HederaMirrorClient mirror, ILogger<HederaAccountService> logger)
    {
        _mirror = mirror ?? throw new ArgumentNullException(nameof(mirror));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>ACC02 — HBAR + token balances from GET /api/v1/accounts/{id}.</summary>
    public async Task<HederaAccountBalanceResult> GetBalanceAsync(string? network, string accountId, CancellationToken ct)
    {
        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, $"/api/v1/accounts/{Uri.EscapeDataString(accountId)}", ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaAccount] getBalance timed out for {AccountId}", accountId);
            return HederaAccountBalanceResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaAccountBalanceResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaAccount] getBalance transport failure for {AccountId}", accountId);
            return HederaAccountBalanceResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaAccountBalanceResult.Fail("MIRROR_NOT_FOUND", $"Account {accountId} not found on the mirror node (may not yet be ingested).");
        if (!resp.IsSuccess) return HederaAccountBalanceResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            var root = doc.RootElement;
            long tinybars = 0;
            var tokens = new List<HederaTokenBalance>();
            if (root.TryGetProperty("balance", out var bal) && bal.ValueKind == JsonValueKind.Object)
            {
                if (bal.TryGetProperty("balance", out var b)) tinybars = HederaJson.GetLongLoose(b);
                if (bal.TryGetProperty("tokens", out var tks) && tks.ValueKind == JsonValueKind.Array)
                    foreach (var t in tks.EnumerateArray())
                        tokens.Add(new HederaTokenBalance(
                            t.TryGetProperty("token_id", out var idp) ? idp.GetString() ?? string.Empty : string.Empty,
                            t.TryGetProperty("balance", out var tbp) ? HederaJson.GetLongLoose(tbp) : 0,
                            0));
            }
            var accId = root.TryGetProperty("account", out var ap) ? ap.GetString() ?? accountId : accountId;
            return HederaAccountBalanceResult.Ok(accId, tinybars, HederaUnits.FormatHbar(tinybars), tokens);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaAccount] getBalance parse failure for {AccountId}", accountId);
            return HederaAccountBalanceResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }

    /// <summary>ACC03 — account metadata from GET /api/v1/accounts/{id}.</summary>
    public async Task<HederaAccountInfoResult> GetInfoAsync(string? network, string accountId, CancellationToken ct)
    {
        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, $"/api/v1/accounts/{Uri.EscapeDataString(accountId)}", ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaAccount] getInfo timed out for {AccountId}", accountId);
            return HederaAccountInfoResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaAccountInfoResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaAccount] getInfo transport failure for {AccountId}", accountId);
            return HederaAccountInfoResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaAccountInfoResult.Fail("MIRROR_NOT_FOUND", $"Account {accountId} not found on the mirror node.");
        if (!resp.IsSuccess) return HederaAccountInfoResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            var root = doc.RootElement;
            long tinybars = 0;
            if (root.TryGetProperty("balance", out var bal) && bal.ValueKind == JsonValueKind.Object && bal.TryGetProperty("balance", out var b))
                tinybars = HederaJson.GetLongLoose(b);
            string? key = null;
            if (root.TryGetProperty("key", out var k) && k.ValueKind == JsonValueKind.Object && k.TryGetProperty("key", out var kk)) key = kk.GetString();
            return HederaAccountInfoResult.Ok(
                root.TryGetProperty("account", out var ap) ? ap.GetString() ?? accountId : accountId,
                root.TryGetProperty("evm_address", out var ev) ? ev.GetString() : null,
                root.TryGetProperty("memo", out var m) ? m.GetString() : null,
                root.TryGetProperty("auto_renew_period", out var ar) && ar.ValueKind == JsonValueKind.Number && ar.TryGetInt64(out var arv) ? arv : null,
                root.TryGetProperty("deleted", out var d) && d.ValueKind == JsonValueKind.True,
                tinybars,
                key);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaAccount] getInfo parse failure for {AccountId}", accountId);
            return HederaAccountInfoResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }
}
