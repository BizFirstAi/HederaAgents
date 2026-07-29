namespace BizFirst.Integration.Hedera.Domain;

/// <summary>A single token holding for an account (base units + decimals).</summary>
public sealed record HederaTokenBalance(string TokenId, long Balance, int Decimals);
