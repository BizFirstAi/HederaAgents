namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of ACC03 — account/getInfo (Mirror Node REST).</summary>
public sealed record HederaAccountInfoResult(
    bool Success,
    string AccountId,
    string? EvmAddress,
    string? Memo,
    long? AutoRenewPeriodSeconds,
    bool Deleted,
    long TinybarBalance,
    string? Key,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaAccountInfoResult Ok(
        string accountId, string? evmAddress, string? memo, long? autoRenew, bool deleted, long tinybars, string? key) =>
        new(true, accountId, evmAddress, memo, autoRenew, deleted, tinybars, key, string.Empty, string.Empty);

    public static HederaAccountInfoResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, null, null, null, false, 0, null, errorCode, errorMessage);
}
