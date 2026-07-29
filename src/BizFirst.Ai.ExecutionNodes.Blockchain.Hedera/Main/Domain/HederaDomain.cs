namespace BizFirst.Ai.ExecutionNodes.Blockchain.Hedera;

/// <summary>Authoritative source for all Hedera config and output key names.</summary>
internal static class HederaDomain
{
    /// <summary>Configuration input property keys read from forms and workflow JSON.</summary>
    public static class InputKeys
    {
        /// <summary>Hedera network selection: mainnet, testnet, previewnet.</summary>
        public const string Network = "network";

        /// <summary>Account identifier in 0.0.x format.</summary>
        public const string AccountID = "accountId";

        /// <summary>Token identifier in 0.0.x format.</summary>
        public const string TokenID = "tokenId";

        /// <summary>Topic identifier in 0.0.x format.</summary>
        public const string TopicID = "topicId";

        /// <summary>Transaction identifier in 0.0.x@seconds.nanos or 0.0.x-seconds-nanos format.</summary>
        public const string TransactionID = "transactionId";

        /// <summary>Maximum number of results to return (1-100).</summary>
        public const string Limit = "limit";

        /// <summary>Starting sequence number for HCS topic message pagination (optional).</summary>
        public const string SequenceFrom = "sequenceFrom";

        /// <summary>Whether to decode message contents from base64 to UTF-8.</summary>
        public const string DecodeUtf8 = "decodeUtf8";

        /// <summary>Numeric amount to convert between units.</summary>
        public const string Amount = "amount";

        /// <summary>Source unit for conversion (hbar or tinybar).</summary>
        public const string FromUnit = "fromUnit";

        /// <summary>Target unit for conversion (hbar or tinybar).</summary>
        public const string ToUnit = "toUnit";

        /// <summary>Address to validate (native 0.0.x or EVM 0x...).</summary>
        public const string Address = "address";
    }

    /// <summary>Output result dictionary keys written to the ExecutionContext result.</summary>
    public static class OutputKeys
    {
        /// <summary>Account identifier (mirrored from input for tracing).</summary>
        public const string AccountID = "accountId";

        /// <summary>Token identifier (mirrored from input for tracing).</summary>
        public const string TokenID = "tokenId";

        /// <summary>Topic identifier (mirrored from input for tracing).</summary>
        public const string TopicID = "topicId";

        /// <summary>HBAR balance formatted with 8 decimal places.</summary>
        public const string HbarBalance = "hbarBalance";

        /// <summary>Balance in tinybars (atomic units, no decimals).</summary>
        public const string TinybarBalance = "tinybarBalance";

        /// <summary>Array of token holdings with ID, balance, and decimals.</summary>
        public const string TokenBalances = "tokenBalances";

        /// <summary>Token name from HTS token metadata.</summary>
        public const string Name = "name";

        /// <summary>Token symbol/ticker from HTS token metadata.</summary>
        public const string Symbol = "symbol";

        /// <summary>Number of decimal places for token display.</summary>
        public const string Decimals = "decimals";

        /// <summary>Total tokens minted (as string to preserve precision).</summary>
        public const string TotalSupply = "totalSupply";

        /// <summary>Token type: FUNGIBLE_COMMON or NON_FUNGIBLE_UNIQUE.</summary>
        public const string TokenType = "tokenType";

        /// <summary>Treasury account holding the initial token supply.</summary>
        public const string TreasuryAccountID = "treasuryAccountId";

        /// <summary>Whether the token or account is deleted.</summary>
        public const string Deleted = "deleted";

        /// <summary>EVM-compatible 20-byte address (when present).</summary>
        public const string EvmAddress = "evmAddress";

        /// <summary>Account memo field.</summary>
        public const string Memo = "memo";

        /// <summary>Auto-renewal period in seconds.</summary>
        public const string AutoRenewPeriodSeconds = "autoRenewPeriodSeconds";

        /// <summary>Account signing key.</summary>
        public const string Key = "key";

        /// <summary>Number of token holdings returned.</summary>
        public const string Count = "count";

        /// <summary>Array of token objects with ID, balance, decimals.</summary>
        public const string Tokens = "tokens";

        /// <summary>Array of HCS topic messages (decoded or base64).</summary>
        public const string Messages = "messages";

        /// <summary>Transaction result status (SUCCESS, FAILURE, etc).</summary>
        public const string Result = "result";

        /// <summary>Consensus timestamp when Hedera agreed on the transaction.</summary>
        public const string ConsensusTimestamp = "consensusTimestamp";

        /// <summary>Actual fee charged for the transaction in tinybars.</summary>
        public const string ChargedTxFee = "chargedTxFee";

        /// <summary>Operation name (if available from Mirror Node).</summary>
        public const string TransactionName = "name";

        /// <summary>Entity ID affected by the transaction.</summary>
        public const string EntityID = "entityId";

        /// <summary>Converted numeric amount (as string to preserve precision).</summary>
        public const string ConvertedAmount = "result";

        /// <summary>Source unit used in conversion.</summary>
        public const string ConversionFromUnit = "fromUnit";

        /// <summary>Target unit used in conversion.</summary>
        public const string ConversionToUnit = "toUnit";

        /// <summary>Whether the address is valid.</summary>
        public const string IsValid = "isValid";

        /// <summary>Address classification: nativeId, evmLongZero, evmAlias, unknown.</summary>
        public const string Format = "format";

        /// <summary>Standardized address representation.</summary>
        public const string Normalized = "normalized";
    }

    /// <summary>Error code constants for operation failures.</summary>
    public static class ErrorCodes
    {
        public const string MirrorTimeout = "MIRROR_TIMEOUT";
        public const string MirrorUnavailable = "MIRROR_UNAVAILABLE";
        public const string MirrorNotFound = "MIRROR_NOT_FOUND";
        public const string MirrorParseError = "MIRROR_PARSE_ERROR";
        public const string ConfigMissingNetwork = "CFG_MISSING_NETWORK";
        public const string ValidationInvalidUnit = "VAL_INVALID_UNIT";
        public const string ValidationInvalidAmount = "VAL_INVALID_AMOUNT";
        public const string ValidationMissingAddress = "VAL_MISSING_ADDRESS";
        public const string ValidationMissingAccountID = "VAL_MISSING_ACCOUNT_ID";
        public const string ValidationMissingTokenID = "VAL_MISSING_TOKEN_ID";
        public const string ValidationMissingTopicID = "VAL_MISSING_TOPIC_ID";
        public const string ValidationMissingTransactionID = "VAL_MISSING_TRANSACTION_ID";
    }
}
