using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>UTIL02 — utility/convertUnits (tinybar ↔ HBAR).</summary>
internal sealed class UtilityConvertUnitsInfo : BaseHederaOperationInfo
{
    public string? Amount { get; private set; }
    public string FromUnit { get; private set; } = "hbar";
    public string ToUnit { get; private set; } = "tinybar";

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        Amount = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.Amount);
        FromUnit = reader.ReadConfigByKey(HederaDomain.InputKeys.FromUnit, "hbar");
        ToUnit = reader.ReadConfigByKey(HederaDomain.InputKeys.ToUnit, "tinybar");
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(Amount) ? (HederaDomain.ErrorCodes.ValidationInvalidAmount, $"Config key '{HederaDomain.InputKeys.Amount}' is required for utility/convertUnits.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.Amount] = Amount ?? string.Empty,
        [HederaDomain.InputKeys.FromUnit] = FromUnit,
        [HederaDomain.InputKeys.ToUnit] = ToUnit,
    };
}
