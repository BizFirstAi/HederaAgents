namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of TXN01 — transaction/getTransaction (Mirror Node REST).</summary>
public sealed record HederaTransactionGetResult(
    bool Success,
    string TransactionId,
    string Result,
    string ConsensusTimestamp,
    long ChargedTxFee,
    string? Name,
    string? EntityId,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaTransactionGetResult Ok(
        string transactionId, string result, string consensusTimestamp, long chargedFee, string? name, string? entityId) =>
        new(true, transactionId, result, consensusTimestamp, chargedFee, name, entityId, string.Empty, string.Empty);

    public static HederaTransactionGetResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, string.Empty, string.Empty, 0, null, null, errorCode, errorMessage);
}
