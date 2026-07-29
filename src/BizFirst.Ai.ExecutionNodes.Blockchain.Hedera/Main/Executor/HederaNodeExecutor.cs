using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>
/// Hedera Hashgraph ExecutionNode — routes (resource, operation) to feature partials. See the design
/// (010_NodeDesign-Engineer/ExecutionNodes/Hedera/44_Features) for the full 51-entry catalogue.
///
/// Design:
///   HederaNodeExecutor.cs         — routing switch + constructor (this file)
///   HederaNodeExecutor.Config.cs  — settings accessor + port initialisation + manifest
///   Main/Executor/Features/{Resource}/{Operation}/ — one feature partial per operation
///
/// The operations wired here are the mirror-read + local slice (no signing SDK dependency); write
/// operations (signed via the Hashgraph SDK) are added on the same pattern — see the design's
/// INFRASTRUCTURE_WalletManagement + the SDK-spike note (ISSUE-01) for the credential/signing model.
///
/// Registration: <see cref="HederaDependency"/> (Support/).
/// </summary>
public sealed partial class HederaNodeExecutor : ResourceBasedNodeExecutor, IActionNodeExecution
{
    public const string NodeTypeName = "hedera";
    public override string ProcessElementTypeCode => NodeTypeName;

    private readonly HederaAccountService _accountService;
    private readonly HederaTokenService _tokenService;
    private readonly HederaTopicService _topicService;
    private readonly HederaTransactionService _transactionService;
    private readonly HederaUtilityService _utilityService;

    public HederaNodeExecutor(
        ILogger<HederaNodeExecutor> logger,
        HederaAccountService accountService,
        HederaTokenService tokenService,
        HederaTopicService topicService,
        HederaTransactionService transactionService,
        HederaUtilityService utilityService,
        INodeCredentialsFactory? credentialsFactory = null,
        INodeConfigurationParser? configParser = null,
        NodeFormResolver? formResolver = null)
        : base(logger, credentialsFactory, configParser, formResolver)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        _utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
    }

    protected override async Task<NodeExecutionResult> _ExecuteInternal_Route_Async(
        NodeExecutionContext nodeExecutionContext,
        CancellationToken cancellationToken = default)
    {
        _executionResultManager = NodeResultOperateManager.CreateInstance(nodeExecutionContext);

        var result = (mySettings!.Resource, mySettings!.Operation) switch
        {
            // ── account (mirror reads) ───────────────────────────────────────
            ("account", "getBalance") => await _Hedera_Account_GetBalance_Async(nodeExecutionContext, cancellationToken),
            ("account", "getInfo")    => await _Hedera_Account_GetInfo_Async(nodeExecutionContext, cancellationToken),

            // ── token / HTS (mirror reads) ───────────────────────────────────
            ("token", "getInfo")     => await _Hedera_Token_GetInfo_Async(nodeExecutionContext, cancellationToken),
            ("token", "getBalances") => await _Hedera_Token_GetBalances_Async(nodeExecutionContext, cancellationToken),

            // ── topic / HCS (mirror reads) ───────────────────────────────────
            ("topic", "getMessages") => await _Hedera_Topic_GetMessages_Async(nodeExecutionContext, cancellationToken),

            // ── transaction (mirror reads) ───────────────────────────────────
            ("transaction", "get") => await _Hedera_Transaction_Get_Async(nodeExecutionContext, cancellationToken),

            // ── utility (local) ──────────────────────────────────────────────
            ("utility", "convertUnits")    => await _Hedera_Utility_ConvertUnits_Async(nodeExecutionContext, cancellationToken),
            ("utility", "validateAddress") => await _Hedera_Utility_ValidateAddress_Async(nodeExecutionContext, cancellationToken),

            _ => await base._ExecuteInternal_Route_Async(nodeExecutionContext, cancellationToken)
        };

        LogActivity_Status($"{NodeTypeName} operation executed", result.IsSuccess);
        return result;
    }
}
