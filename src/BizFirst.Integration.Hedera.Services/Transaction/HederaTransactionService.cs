namespace BizFirst.Integration.Hedera.Services;

/// <summary>Transaction reads via the Mirror Node REST API (TXN01 get by id).</summary>
public sealed class HederaTransactionService
{
    private readonly HederaMirrorClient _mirror;
    private readonly ILogger<HederaTransactionService> _logger;

    public HederaTransactionService(HederaMirrorClient mirror, ILogger<HederaTransactionService> logger)
    {
        _mirror = mirror ?? throw new ArgumentNullException(nameof(mirror));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// TXN01 — GET /api/v1/transactions/{id}. Accepts the native <c>payerAccountId@validStart</c>
    /// form and normalises it to the mirror's dashed form (<c>0.0.x-seconds-nanos</c>).
    /// </summary>
    public async Task<HederaTransactionGetResult> GetTransactionAsync(string? network, string transactionId, CancellationToken ct)
    {
        var normalized = NormalizeTransactionId(transactionId);
        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, $"/api/v1/transactions/{Uri.EscapeDataString(normalized)}", ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaTransaction] get timed out for {TransactionId}", transactionId);
            return HederaTransactionGetResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaTransactionGetResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaTransaction] get transport failure for {TransactionId}", transactionId);
            return HederaTransactionGetResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaTransactionGetResult.Fail("MIRROR_NOT_FOUND", $"Transaction {transactionId} not found on the mirror node.");
        if (!resp.IsSuccess) return HederaTransactionGetResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            if (!doc.RootElement.TryGetProperty("transactions", out var txs) || txs.ValueKind != JsonValueKind.Array || txs.GetArrayLength() == 0)
                return HederaTransactionGetResult.Fail("MIRROR_NOT_FOUND", $"Transaction {transactionId} not found.");
            var tx = txs[0];
            return HederaTransactionGetResult.Ok(
                tx.TryGetProperty("transaction_id", out var idp) ? idp.GetString() ?? transactionId : transactionId,
                tx.TryGetProperty("result", out var rp) ? rp.GetString() ?? string.Empty : string.Empty,
                tx.TryGetProperty("consensus_timestamp", out var cp) ? cp.GetString() ?? string.Empty : string.Empty,
                tx.TryGetProperty("charged_tx_fee", out var fp) ? HederaJson.GetLongLoose(fp) : 0,
                tx.TryGetProperty("name", out var np) ? np.GetString() : null,
                tx.TryGetProperty("entity_id", out var ep) ? ep.GetString() : null);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaTransaction] get parse failure for {TransactionId}", transactionId);
            return HederaTransactionGetResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }

    /// <summary>Converts "0.0.1234@1690000000.000000000" → "0.0.1234-1690000000-000000000" (mirror form).</summary>
    internal static string NormalizeTransactionId(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId) || !transactionId.Contains('@')) return transactionId;
        var at = transactionId.Split('@', 2);
        return $"{at[0]}-{at[1].Replace('.', '-')}";
    }
}
