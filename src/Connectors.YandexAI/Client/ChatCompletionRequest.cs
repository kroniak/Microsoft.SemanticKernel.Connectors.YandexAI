using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// <summary>
///     Request for Chat Completion containing information for generating chat completions.
/// </summary>
internal sealed record ChatCompletionRequest
{
    /// <summary>
    ///     Construct an instance of <see cref="ChatCompletionRequest" />.
    /// </summary>
    /// <param name="model">ID of the model to use.</param>
    [JsonConstructor]
    internal ChatCompletionRequest(string model)
    {
        Model = model;
    }

    /// <summary>
    ///     Represents the completion options for chat completion requests.
    /// </summary>
    [JsonPropertyName("completionOptions")]
    public ChatCompletionRequestCompletionOptions CompletionOptions { get; set; } = new();

    /// <summary>
    ///     Represents the Model property in a ChatCompletionRequest object.
    /// </summary>
    [JsonPropertyName("modelUri")]
    public string Model { get; set; }

    /// <summary>
    ///     Represents a collection of chat messages used in the ChatCompletionRequest.
    /// </summary>
    [JsonPropertyName("messages")]
    public IList<YandexAIChatMessage> Messages { get; set; } = [];

    /// <summary>
    ///     Represents the stop condition for chat completion.
    /// </summary>
    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? Stop { get; set; }

    /// <summary>
    ///     Add a message to the request.
    /// </summary>
    /// <param name="message">The chat message to add.</param>
    internal void AddMessage(YandexAIChatMessage message)
    {
        Verify.NotNull(message);
        Messages.Add(message);
    }
}