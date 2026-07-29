using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>UTIL01 — utility/validateAddress.</summary>
internal sealed class UtilityValidateAddressInfo : BaseHederaOperationInfo
{
    public string? Address { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        base.LoadFrom(reader);
        Address = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.Address);
    }

    public (string Code, string Message)? Validate() =>
        string.IsNullOrWhiteSpace(Address) ? (HederaDomain.ErrorCodes.ValidationMissingAddress, $"Config key '{HederaDomain.InputKeys.Address}' is required for utility/validateAddress.") : null;

    public override Dictionary<string, object> ToDictionary() => new()
    {
        [HederaDomain.InputKeys.Address] = Address ?? string.Empty,
    };
}
