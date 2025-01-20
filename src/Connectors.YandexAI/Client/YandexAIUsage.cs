using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Connectors.YandexAI.Helpers;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// <summary>
///     Represents usage information related to Yandex AI.
/// </summary>
internal sealed record YandexAIUsage
{
    /// <summary>
    ///     The number of tokens processed for the input text.
    /// </summary>
    [JsonPropertyName("inputTextTokens")]
    [JsonConverter(typeof(StringJsonConverter))]
    public int? InputTextTokens { get; set; }

    /// <summary>
    ///     The number of total tokens processed.
    /// </summary>
    [JsonPropertyName("totalTokens")]
    [JsonConverter(typeof(StringJsonConverter))]
    public int? TotalTokens { get; set; }

    /// <summary>
    ///     The number of completion tokens processed for Yandex AI usage.
    /// </summary>
    [JsonPropertyName("completionTokens")]
    [JsonConverter(typeof(StringJsonConverter))]
    public int? CompletionTokens { get; set; }
}