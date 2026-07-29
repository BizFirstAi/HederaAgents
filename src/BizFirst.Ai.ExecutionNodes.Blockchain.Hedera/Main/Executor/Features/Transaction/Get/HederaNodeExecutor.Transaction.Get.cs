using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

public sealed partial class HederaNodeExecutor
{
    private async Task<NodeExecutionResult> _Hedera_Transaction_Get_Async(
        NodeExecutionContext nodeExecutionContext,
        CancellationToken cancellationToken = default)
    {
        //code-step: 1.1 - Validate settings exist and cast to TransactionGetInfo
        if (mySettings?.ActiveInfo is not TransactionGetInfo info)
            return SimpleErrorOperationUnfound();

        //code-step: 1.2 - Create result manager for output handling
        var resultManager = NodeResultOperateManager.CreateInstance(nodeExecutionContext);

        var error = info.Validate();
        if (error is not null)
            return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, error.Value.Message, this);

        try
        {
            //code-step: 1.3 - Call Hedera transaction service to fetch the transaction (Mirror Node REST)
            var r = await _transactionService.GetTransactionAsync(info.Network, info.TransactionId!, cancellationToken);

            if (!r.Success)
                return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, r.ErrorMessage, this);

            //code-step: 1.4 - Report progress milestone to execution context
            await ReportNodeProgress_ResourceOperation(nodeExecutionContext, "IntegrationCallCompleted");

            //code-step: 1.5 - Extract transaction record from result
            var record = new Dictionary<string, object>
            {
                { "transactionId", r.TransactionId },
                { "result", r.Result },
                { "consensusTimestamp", r.ConsensusTimestamp },
                { "chargedTxFee", r.ChargedTxFee },
                { "name", r.Name ?? string.Empty },
                { "entityId", r.EntityId ?? string.Empty },
            };

            //code-step: 1.6 - Build output metadata dictionary
            var outputData = resultManager.GetOrCreateOutputData();
            outputData["status"] = "success";
            outputData["resource"] = "transaction";
            outputData["operation"] = "get";

            //code-step: 1.7 - Convert transaction record to standard items array
            outputData.TryGetValue(ExecutionConstants.OutputFieldNameConstants.CONST_items, out var existingItemsValue);
            outputData[ExecutionConstants.OutputFieldNameConstants.CONST_items] = ApplyOutputItemsMerge(existingItemsValue, WrapJsonIntoItems(record, nodeExecutionContext));

            //code-step: 1.8 - Write output (handles TargetDataPath writes + items downstream)
            return await WriteOutputData(ExecutionConstants.OutputPorts.Success, outputData, record, nodeExecutionContext, cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            //code-step: 1.9 - Catch exceptions and return error with context
            return resultManager.SetResultAsError(ExecutionConstants.OutputPorts.Error, $"transaction/get failed for {info.TransactionId}: {ex.Message}", this);
        }
    }
}
