namespace BizFirst.Integration.Hedera.Services;

/// <summary>Thrown when an operation selects a Hedera network name that has no configured mirror endpoint.</summary>
public sealed class HederaNetworkNotConfiguredException : Exception
{
    public HederaNetworkNotConfiguredException(string network)
        : base($"Hedera network '{network}' is not configured. Add it to the 'Hedera:Networks' appsettings section.")
    {
    }
}
