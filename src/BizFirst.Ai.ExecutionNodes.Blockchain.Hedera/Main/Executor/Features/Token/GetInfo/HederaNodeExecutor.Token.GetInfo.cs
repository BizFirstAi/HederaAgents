using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

public sealed partial class HederaNodeExecutor
{
    private async Task<NodeExecutionResult> _Hedera_Token_GetInfo_Async(
        NodeExecutionContext nodeExecutionContext,
        CancellationToken cancellationToken = default)
    {
        //code-step: 1.1 - Validate settings exist and cast to TokenGetInfoInfo
        if (mySettings?.ActiveInfo is not TokenGetInfoInfo info)
            return SimpleErrorOperationUnfound();

        //code-step: 1.2 - Create result manager for output handling
        var resultManager = NodeResultOperateManager.CreateInstance(nodeExecutionContext);

        var error = info.Validate();
        if (error is not null)
            return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, error.Value.Message, this);

        try
        {
            //code-step: 1.3 - Call Hedera token service to fetch token info (Mirror Node REST)
            var r = await _tokenService.GetTokenInfoAsync(info.Network, info.TokenId!, cancellationToken);

            if (!r.Success)
                return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, r.ErrorMessage, this);

            //code-step: 1.4 - Report progress milestone to execution context
            await ReportNodeProgress_ResourceOperation(nodeExecutionContext, "IntegrationCallCompleted");

            //code-step: 1.5 - Extract token info record from result
            var record = new Dictionary<string, object>
            {
                { "tokenId", r.TokenId },
                { "name", r.Name },
                { "symbol", r.Symbol },
                { "decimals", r.Decimals },
                { "totalSupply", r.TotalSupply },
                { "tokenType", r.TokenType },
                { "treasuryAccountId", r.TreasuryAccountId ?? string.Empty },
                { "deleted", r.Deleted },
            };

            //code-step: 1.6 - Build output metadata dictionary
            var outputData = resultManager.GetOrCreateOutputData();
            outputData["status"] = "success";
            outputData["resource"] = "token";
            outputData["operation"] = "getInfo";

            //code-step: 1.7 - Convert token info record to standard items array
            outputData.TryGetValue(ExecutionConstants.OutputFieldNameConstants.CONST_items, out var existingItemsValue);
            outputData[ExecutionConstants.OutputFieldNameConstants.CONST_items] = ApplyOutputItemsMerge(existingItemsValue, WrapJsonIntoItems(record, nodeExecutionContext));

            //code-step: 1.8 - Write output (handles TargetDataPath writes + items downstream)
            return await WriteOutputData(ExecutionConstants.OutputPorts.Success, outputData, record, nodeExecutionContext, cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            //code-step: 1.9 - Catch exceptions and return error with context
            return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, $"token/getInfo failed for {info.TokenId}: {ex.Message}", this);
        }
    }
}
