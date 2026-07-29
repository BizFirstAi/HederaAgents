using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>TXN01 — transaction/get.</summary>
internal sealed class TransactionGetInfo : BaseHederaOperationInfo
{
    public string? TransactionID { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        TransactionID = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.TransactionID);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(TransactionID) ? (HederaDomain.ErrorCodes.ValidationMissingTransactionID, $"Config key '{HederaDomain.InputKeys.TransactionID}' is required for transaction/get.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.TransactionID] = TransactionID ?? string.Empty,
    };
}
