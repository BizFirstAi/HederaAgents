namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of UTIL01 — utility/validateAddress (local parse).</summary>
public sealed record HederaValidateAddressResult(
    bool Success,
    bool IsValid,
    string Format,
    string Normalized,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaValidateAddressResult Ok(bool isValid, string format, string normalized) =>
        new(true, isValid, format, normalized, string.Empty, string.Empty);

    public static HederaValidateAddressResult Fail(string errorCode, string errorMessage) =>
        new(false, false, string.Empty, string.Empty, errorCode, errorMessage);
}
