using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>Configuration management partial for <see cref="HederaNodeExecutor"/>.</summary>
public sealed partial class HederaNodeExecutor
{
    /// <summary>Returns a new <see cref="HederaNodeExecutorSettings"/> so LoadConfigAsync gets typed access instead of DictionaryBasedSettings.</summary>
    public override BaseNodeExecutorSettings CreateExecutorSettings() => new HederaNodeExecutorSettings();

    /// <summary>Typed settings accessor — casts this.settings to <see cref="HederaNodeExecutorSettings"/>.</summary>
    private HederaNodeExecutorSettings? mySettings => (HederaNodeExecutorSettings?)this.settings;

    private NodeResultOperateManager? _executionResultManager;
    private NodeResultOperateManager resultManager => _executionResultManager!;

    /// <summary>Initialises the standard success ("main") / error output ports on this node.</summary>
    public override void ValidateExecutorSettings()
    {
        if (mySettings?.OutputMapping is null) return;
        mySettings.OutputMapping.GetOrCreatePortSuccessAndError();
    }

    protected override NodeExecutorManifest? GetNodeExecutorManifest()
        => NodeExecutorManifest.From(
            ProcessElementTypeCode,
            [],
            new SuspensionPolicy { AllowAdminForceComplete = true });
}
