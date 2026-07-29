using System.Globalization;
using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>TOP06 — topic/getMessages.</summary>
internal sealed class TopicGetMessagesInfo : BaseHederaOperationInfo
{
    public string? TopicID { get; private set; }
    public int Limit { get; private set; } = 25;
    public long? SequenceFrom { get; private set; }
    public bool DecodeUtf8 { get; private set; } = true;

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        TopicID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.TopicID);
        Limit = int.TryParse(reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.Limit), NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) ? l : 25;
        SequenceFrom = long.TryParse(reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.SequenceFrom), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) ? s : null;
        DecodeUtf8 = !string.Equals(reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.DecodeUtf8), "false", StringComparison.OrdinalIgnoreCase);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(TopicID) ? (HederaDomain.ErrorCodes.ValidationMissingTopicID, $"Config key '{HederaDomain.InputKeys.TopicID}' is required for topic/getMessages.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.TopicID] = TopicID ?? string.Empty,
        [HederaDomain.InputKeys.Limit] = Limit,
        [HederaDomain.InputKeys.SequenceFrom] = SequenceFrom,
        [HederaDomain.InputKeys.DecodeUtf8] = DecodeUtf8,
    };
}
