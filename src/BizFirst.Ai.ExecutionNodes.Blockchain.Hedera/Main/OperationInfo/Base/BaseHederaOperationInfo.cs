using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>
/// Common per-operation config every Hedera operation shares: which network to run against.
/// Selects an entry from the "Hedera:Networks" application configuration (see
/// <see cref="HederaNetworkOptions"/>) by name (mainnet/testnet/previewnet) — never a raw endpoint,
/// so node instances stay portable across environments.
/// </summary>
internal abstract class BaseHederaOperationInfo : BaseNodeOperationInfo
{
    /// <summary>Network name (mainnet/testnet/previewnet). Falls back to the configured default when omitted.</summary>
    public string? Network { get; private set; }

    public override void LoadFrom(ConfigDataPropertyBag reader)
    {
        Network = reader.ReadConfigByKeyDefaultNull(HederaDomain.InputKeys.Network);
    }
}
