using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>
/// DI registration for the Hedera ExecutionNode plugin. Registers all Hedera services, the executor,
/// and the executor-registry entry.
///
/// IMPORTANT — this alone is not sufficient to make the node discoverable at runtime. Per this
/// codebase's plugin-loading mechanism, an assembly that is only ProjectReference'd but never
/// force-loaded is not guaranteed to be present in AppDomain.CurrentDomain.GetAssemblies() when the
/// assembly-scanning RegisterNodeExecutors() runs. Add an explicit
/// "new HederaDependency().RegisterDefaults(services);" line to Plugins_RegisterAllNodes(...) in
/// BizFirst.Ai.Platform.Web.Server.Core/DependencyInjection/Ai/ServiceCollectionExtensionsForAI.cs
/// (mirroring the existing Ethereum registration) so this node registers.
/// </summary>
public sealed class HederaDependency : INodeExecutorDependency
{
    public void RegisterDefaults(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.AddDependenciesHederaServices();
        services.AddScoped<HederaNodeExecutor>();
        ExecutorRegistry.Register<HederaNodeExecutor>(HederaNodeExecutor.NodeTypeName);
    }
}
