namespace BizFirst.Integration.Hedera.Services;

/// <summary>Topic (HCS) reads via the Mirror Node REST API (TOP06 messages).</summary>
public sealed class HederaTopicService
{
    private readonly HederaMirrorClient _mirror;
    private readonly ILogger<HederaTopicService> _logger;

    public HederaTopicService(HederaMirrorClient mirror, ILogger<HederaTopicService> logger)
    {
        _mirror = mirror ?? throw new ArgumentNullException(nameof(mirror));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>TOP06 — ordered topic messages from GET /api/v1/topics/{id}/messages (base64-decoded).</summary>
    public async Task<HederaTopicMessagesResult> GetMessagesAsync(
        string? network, string topicId, int limit, long? sequenceFrom, bool decodeUtf8, CancellationToken ct)
    {
        var clamped = limit is < 1 or > 100 ? 25 : limit;
        var path = $"/api/v1/topics/{Uri.EscapeDataString(topicId)}/messages?limit={clamped}&order=asc";
        if (sequenceFrom is > 0) path += $"&sequencenumber=gte:{sequenceFrom}";

        HederaMirrorResponse resp;
        try { resp = await _mirror.GetAsync(network, path, ct).ConfigureAwait(false); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("[HederaTopic] getMessages timed out for {TopicId}", topicId);
            return HederaTopicMessagesResult.Fail("MIRROR_TIMEOUT", "Mirror node request timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (HederaNetworkNotConfiguredException ex) { return HederaTopicMessagesResult.Fail("CFG_MISSING_NETWORK", ex.Message); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HederaTopic] getMessages transport failure for {TopicId}", topicId);
            return HederaTopicMessagesResult.Fail("MIRROR_UNAVAILABLE", ex.Message);
        }

        if (resp.IsNotFound) return HederaTopicMessagesResult.Fail("MIRROR_NOT_FOUND", $"Topic {topicId} not found on the mirror node.");
        if (!resp.IsSuccess) return HederaTopicMessagesResult.Fail("MIRROR_UNAVAILABLE", $"Mirror node returned HTTP {resp.StatusCode}.");

        try
        {
            using var doc = JsonDocument.Parse(resp.Body);
            var messages = new List<HederaTopicMessage>();
            if (doc.RootElement.TryGetProperty("messages", out var msgs) && msgs.ValueKind == JsonValueKind.Array)
                foreach (var msg in msgs.EnumerateArray())
                {
                    var rawB64 = msg.TryGetProperty("message", out var mp) ? mp.GetString() ?? string.Empty : string.Empty;
                    var contents = decodeUtf8 ? DecodeBase64Utf8(rawB64) : rawB64;
                    messages.Add(new HederaTopicMessage(
                        msg.TryGetProperty("sequence_number", out var sp) ? HederaJson.GetLongLoose(sp) : 0,
                        msg.TryGetProperty("consensus_timestamp", out var cp) ? cp.GetString() ?? string.Empty : string.Empty,
                        contents,
                        msg.TryGetProperty("running_hash", out var rp) ? rp.GetString() ?? string.Empty : string.Empty,
                        msg.TryGetProperty("payer_account_id", out var pp) ? pp.GetString() : null,
                        msg.TryGetProperty("topic_id", out var tp) ? tp.GetString() ?? topicId : topicId));
                }
            return HederaTopicMessagesResult.Ok(topicId, messages);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "[HederaTopic] getMessages parse failure for {TopicId}", topicId);
            return HederaTopicMessagesResult.Fail("MIRROR_PARSE_ERROR", ex.Message);
        }
    }

    private static string DecodeBase64Utf8(string b64)
    {
        if (string.IsNullOrEmpty(b64)) return string.Empty;
        try { return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(b64)); }
        catch (FormatException) { return b64; }
    }
}
