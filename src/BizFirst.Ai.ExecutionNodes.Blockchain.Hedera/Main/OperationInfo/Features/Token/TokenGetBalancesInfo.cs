using System.Globalization;
using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>TOK09 — token/getBalances (an account's HTS holdings).</summary>
internal sealed class TokenGetBalancesInfo : BaseHederaOperationInfo
{
    public string? AccountID { get; private set; }
    public int Limit { get; private set; } = 25;

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        AccountID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.AccountID);
        Limit = int.TryParse(reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.Limit), NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) ? l : 25;
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(AccountID) ? (HederaDomain.ErrorCodes.ValidationMissingAccountID, $"Config key '{HederaDomain.InputKeys.AccountID}' is required for token/getBalances.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.AccountID] = AccountID ?? string.Empty,
        [HederaDomain.InputKeys.Limit] = Limit,
    };
}
