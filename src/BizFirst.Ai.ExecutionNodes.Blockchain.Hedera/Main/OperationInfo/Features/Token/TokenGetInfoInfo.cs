using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>TOK08 — token/getInfo.</summary>
internal sealed class TokenGetInfoInfo : BaseHederaOperationInfo
{
    public string? TokenID { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        TokenID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.TokenID);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(TokenID) ? (HederaDomain.ErrorCodes.ValidationMissingTokenID, $"Config key '{HederaDomain.InputKeys.TokenID}' is required for token/getInfo.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.TokenID] = TokenID ?? string.Empty,
    };
}
