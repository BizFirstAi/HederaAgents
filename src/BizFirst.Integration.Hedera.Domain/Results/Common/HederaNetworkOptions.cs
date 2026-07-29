namespace BizFirst.Integration.Hedera.Domain;

/// <summary>
/// Static application configuration bound from the "Hedera" appsettings section — the mirror-node
/// endpoint catalogue and default network. Distinct from per-node-instance config (which selects a
/// network by name via the "network" config key on each operation) and from the signing credential
/// (resolved per-call from the vault via "credentialID").
///
/// Hedera consensus-node access is selected by network name (mainnet/testnet/previewnet) — the SDK
/// resolves the node addresses itself, so no per-node RPC URL is stored here (unlike Ethereum). Only
/// the free/eventually-consistent Mirror Node REST base URL is configured per network.
/// </summary>
public sealed class HederaNetworkOptions
{
    public const string SectionName = "Hedera";

    /// <summary>Network used when an operation omits the "network" config key.</summary>
    public string DefaultNetwork { get; init; } = "testnet";

    /// <summary>Network name (mainnet/testnet/previewnet) → mirror-node configuration.</summary>
    public Dictionary<string, HederaNetworkConfig> Networks { get; init; } =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["mainnet"]    = new HederaNetworkConfig { MirrorRestUrl = "https://mainnet-public.mirrornode.hedera.com" },
            ["testnet"]    = new HederaNetworkConfig { MirrorRestUrl = "https://testnet.mirrornode.hedera.com" },
            ["previewnet"] = new HederaNetworkConfig { MirrorRestUrl = "https://previewnet.mirrornode.hedera.com" },
        };

    /// <summary>Mirror-node REST timeout (seconds).</summary>
    public int MirrorTimeoutSeconds { get; init; } = 30;

    /// <summary>Default cap on the payer's willingness to pay per transaction, in HBAR.</summary>
    public decimal DefaultMaxTransactionFeeHbar { get; init; } = 2.0m;

    /// <summary>Default transaction valid-duration (seconds); network maximum is 180.</summary>
    public int DefaultTransactionValidDurationSeconds { get; init; } = 120;
}

/// <summary>Mirror-node configuration for a single Hedera network.</summary>
public sealed class HederaNetworkConfig
{
    /// <summary>Mirror Node REST base URL (no trailing slash), e.g. https://testnet.mirrornode.hedera.com.</summary>
    public required string MirrorRestUrl { get; init; }
}
