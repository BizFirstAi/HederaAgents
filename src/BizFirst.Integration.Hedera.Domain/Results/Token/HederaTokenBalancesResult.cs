namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of TOK09 — token/getAccountTokenBalances (Mirror Node REST, list).</summary>
public sealed record HederaTokenBalancesResult(
    bool Success,
    string AccountId,
    IReadOnlyList<HederaTokenBalance> Tokens,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaTokenBalancesResult Ok(string accountId, IReadOnlyList<HederaTokenBalance> tokens) =>
        new(true, accountId, tokens, string.Empty, string.Empty);

    public static HederaTokenBalancesResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, Array.Empty<HederaTokenBalance>(), errorCode, errorMessage);
}
