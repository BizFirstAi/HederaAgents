namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of ACC02 — account/getBalance (Mirror Node REST).</summary>
public sealed record HederaAccountBalanceResult(
    bool Success,
    string AccountId,
    long TinybarBalance,
    string HbarBalance,
    IReadOnlyList<HederaTokenBalance> TokenBalances,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaAccountBalanceResult Ok(
        string accountId, long tinybars, string hbar, IReadOnlyList<HederaTokenBalance> tokens) =>
        new(true, accountId, tinybars, hbar, tokens, string.Empty, string.Empty);

    public static HederaAccountBalanceResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, 0, "0", Array.Empty<HederaTokenBalance>(), errorCode, errorMessage);
}
