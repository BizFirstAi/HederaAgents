using BizFirst.Ai.ProcessEngine.Service;
namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>
/// Maps (resource, operation) config values to the concrete operation DTO, then loads it from the
/// raw config via <see cref="ConfigDataPropertyBag"/>. See the design docs
/// (010_NodeDesign-Engineer/ExecutionNodes/Hedera/44_Features) for the full 51-entry catalogue this
/// mirrors; the operations wired here are the read/local slice implemented so far.
/// </summary>
internal static class HederaOperationInfoFactory
{
    internal static BaseHederaOperationInfo? Create(string? resource, string? operation, ConfigDataPropertyBag reader)
    {
        BaseHederaOperationInfo? info = (resource, operation) switch
        {
            // ── account (mirror reads) ───────────────────────────────────────
            ("account", "getBalance") => new AccountGetBalanceInfo(),
            ("account", "getInfo")    => new AccountGetInfoInfo(),

            // ── token / HTS (mirror reads) ───────────────────────────────────
            ("token", "getInfo")     => new TokenGetInfoInfo(),
            ("token", "getBalances") => new TokenGetBalancesInfo(),

            // ── topic / HCS (mirror reads) ───────────────────────────────────
            ("topic", "getMessages") => new TopicGetMessagesInfo(),

            // ── transaction (mirror reads) ───────────────────────────────────
            ("transaction", "get") => new TransactionGetInfo(),

            // ── utility (local) ──────────────────────────────────────────────
            ("utility", "convertUnits")    => new UtilityConvertUnitsInfo(),
            ("utility", "validateAddress") => new UtilityValidateAddressInfo(),

            _ => null,
        };

        info?.LoadFrom(reader);
        return info;
    }
}
