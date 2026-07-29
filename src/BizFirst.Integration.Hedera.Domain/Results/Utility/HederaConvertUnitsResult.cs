namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of UTIL02 — utility/convertUnits (tinybar ↔ HBAR, local).</summary>
public sealed record HederaConvertUnitsResult(
    bool Success,
    string Result,
    string FromUnit,
    string ToUnit,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaConvertUnitsResult Ok(string result, string fromUnit, string toUnit) =>
        new(true, result, fromUnit, toUnit, string.Empty, string.Empty);

    public static HederaConvertUnitsResult Fail(string errorCode, string errorMessage) =>
        new(false, "0", string.Empty, string.Empty, errorCode, errorMessage);
}
