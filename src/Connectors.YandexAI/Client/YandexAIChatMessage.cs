using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// <summary>
///     Represents a chat message for YandexAI.
/// </summary>
// ReSharper disable once InconsistentNaming
internal sealed record YandexAIChatMessage
{
    /// <summary>
    ///     Chat message for YandexAI.
    ///     Construct an instance of <see cref="YandexAIChatMessage" />.
    /// </summary>
    /// <param name="role">If provided must be one of: system, user, assistant</param>
    /// <param name="text">Text of the chat message</param>
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.ChatCompletionResponses" />
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.ContentChunk" />
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.YandexAIClient" />
    [JsonConstructor]
    internal YandexAIChatMessage(string? role, object? text)
    {
        if (role is not null and not "system" and not "user" and not "assistant" and not "tool")
            throw new ArgumentException(
                $"Role must be one of: system, user, assistant or tool. {role} is an invalid role.", nameof(role));

        Role = role;
        Text = text;
    }

    /// <summary>
    ///     Represents the role of a chat message in YandexAI. The role must be one of the following values: system, user,
    ///     assistant, tool.
    /// </summary>
    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; }

    /// <summary>
    ///     Text of the chat message.
    /// </summary>
    /// <remarks>
    ///     This property represents the main content of a chat message in the YandexAIChatMessage class.
    /// </remarks>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Text { get; set; }
}