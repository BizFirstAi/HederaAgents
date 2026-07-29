namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of TOK08 — token/getTokenInfo (Mirror Node REST).</summary>
public sealed record HederaTokenInfoResult(
    bool Success,
    string TokenId,
    string Name,
    string Symbol,
    int Decimals,
    string TotalSupply,
    string TokenType,
    string? TreasuryAccountId,
    bool Deleted,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaTokenInfoResult Ok(
        string tokenId, string name, string symbol, int decimals, string totalSupply,
        string tokenType, string? treasury, bool deleted) =>
        new(true, tokenId, name, symbol, decimals, totalSupply, tokenType, treasury, deleted, string.Empty, string.Empty);

    public static HederaTokenInfoResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, string.Empty, string.Empty, 0, "0", string.Empty, null, false, errorCode, errorMessage);
}
