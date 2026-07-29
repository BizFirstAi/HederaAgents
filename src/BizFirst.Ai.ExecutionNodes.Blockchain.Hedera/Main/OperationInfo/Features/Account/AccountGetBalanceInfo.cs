using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>ACC02 — account/getBalance.</summary>
internal sealed class AccountGetBalanceInfo : BaseHederaOperationInfo
{
    public string? AccountID { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        AccountID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.AccountID);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(AccountID) ? (HederaDomain.ErrorCodes.ValidationMissingAccountID, $"Config key '{HederaDomain.InputKeys.AccountID}' is required for account/getBalance.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.AccountID] = AccountID ?? string.Empty,
    };
}
