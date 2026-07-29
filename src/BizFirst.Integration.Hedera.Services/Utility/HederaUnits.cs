namespace BizFirst.Integration.Hedera.Services;

/// <summary>Exact tinybar ↔ HBAR conversion (no floating point). 1 HBAR = 100,000,000 tinybar.</summary>
public static class HederaUnits
{
    public const long TinybarsPerHbar = 100_000_000L;

    /// <summary>Formats a tinybar amount as an HBAR decimal string (up to 8 dp, invariant).</summary>
    public static string FormatHbar(long tinybars)
        => ((decimal)tinybars / TinybarsPerHbar).ToString("0.########", CultureInfo.InvariantCulture);
}
