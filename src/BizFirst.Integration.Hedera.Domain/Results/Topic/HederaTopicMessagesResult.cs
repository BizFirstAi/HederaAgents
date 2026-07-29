namespace BizFirst.Integration.Hedera.Domain;

/// <summary>Result of TOP06 — topic/getMessages (Mirror Node REST, list).</summary>
public sealed record HederaTopicMessagesResult(
    bool Success,
    string TopicId,
    IReadOnlyList<HederaTopicMessage> Messages,
    string ErrorCode,
    string ErrorMessage)
{
    public static HederaTopicMessagesResult Ok(string topicId, IReadOnlyList<HederaTopicMessage> messages) =>
        new(true, topicId, messages, string.Empty, string.Empty);

    public static HederaTopicMessagesResult Fail(string errorCode, string errorMessage) =>
        new(false, string.Empty, Array.Empty<HederaTopicMessage>(), errorCode, errorMessage);
}
