using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// <summary>
///     Represents the result of a chat completion response.
/// </summary>
internal sealed record ChatCompletionResponseResult
{
    /// <summary>
    ///     Represents the result of a chat completion response.
    /// </summary>
    [JsonPropertyName("result")]
    public ChatCompletionResponses? Result { get; set; }
}

/// <summary>
///     Represents a set of responses for chat message completion.
/// </summary>
internal sealed record ChatCompletionResponses
{
    /// <summary>
    ///     Represents the model version used for chat completion responses in YandexAI.
    /// </summary>
    [JsonPropertyName("modelVersion")]
    public string? ModelVersion { get; set; }

    /// <summary>
    ///     Represents information about the usage of YandexAI for a chat completion response.
    /// </summary>
    [JsonPropertyName("usage")]
    public YandexAIUsage? Usage { get; set; }

    /// <summary>
    ///     Represents a list of alternative options for chat message completion.
    /// </summary>
    [JsonPropertyName("alternatives")]
    public IReadOnlyCollection<ChatCompletionAlternative>? Alternatives { get; set; }
}

/// <summary>
///     Represents an alternative option for chat message completion.
/// </summary>
internal sealed record ChatCompletionAlternative
{
    /// <summary>
    ///     Represents a chat message for YandexAI.
    /// </summary>
    [JsonPropertyName("message")]
    public YandexAIChatMessage? Message { get; set; }

    /// <summary>
    ///     The reason the chat completion was finished.
    ///     The generation status of the alternative.
    ///     ALTERNATIVE_STATUS_UNSPECIFIED: Unspecified generation status.
    ///     ALTERNATIVE_STATUS_PARTIAL: Partially generated alternative.
    ///     ALTERNATIVE_STATUS_TRUNCATED_FINAL: Incomplete final alternative resulting from reaching the maximum allowed number
    ///     of tokens.
    ///     ALTERNATIVE_STATUS_FINAL: Final alternative generated without running into any limits.
    ///     ALTERNATIVE_STATUS_CONTENT_FILTER: Generation was stopped due to the discovery of potentially sensitive content in
    ///     the prompt or generated response.
    ///     To fix, modify the prompt and restart generation.
    ///     ALTERNATIVE_STATUS_TOOL_CALLS: Tools were invoked during the completion generation.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}