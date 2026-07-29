using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>Typed settings for <see cref="HederaNodeExecutor"/> — resolves the active operation DTO from (resource, operation).</summary>
internal sealed class HederaNodeExecutorSettings : ResourceBasedNodeExecutorSettings
{
    public override BaseNodeOperationInfo? CreateActiveInfo() =>
        HederaOperationInfoFactory.Create(Resource, Operation, ConfigReader);
}
