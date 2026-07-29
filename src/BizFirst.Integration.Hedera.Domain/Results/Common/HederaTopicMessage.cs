namespace BizFirst.Integration.Hedera.Domain;

/// <summary>A single HCS topic message from the mirror node (contents base64-decoded to UTF-8).</summary>
public sealed record HederaTopicMessage(
    long SequenceNumber,
    string ConsensusTimestamp,
    string Message,
    string RunningHash,
    string? PayerAccountId,
    string TopicId);
