using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>ACC03 — account/getInfo.</summary>
internal sealed class AccountGetInfoInfo : BaseHederaOperationInfo
{
    public string? AccountID { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        AccountID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.AccountID);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(AccountID) ? (HederaDomain.ErrorCodes.ValidationMissingAccountID, $"Config key '{HederaDomain.InputKeys.AccountID}' is required for account/getInfo.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.AccountID] = AccountID ?? string.Empty,
    };
}
