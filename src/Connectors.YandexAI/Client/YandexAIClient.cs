using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// <summary>
///     Represents a client for interacting with Yandex AI services.
/// </summary>
// ReSharper disable once InconsistentNaming
internal sealed class YandexAIClient
{
    /// Internal class for interacting with Yandex AI service.
    /// </summary>
    internal YandexAIClient(string modelId,
        HttpClient httpClient,
        string apiKey,
        string folderId,
        Uri? endpoint = null,
        ILogger? logger = null)
    {
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);
        Verify.NotNull(httpClient);

        _endpoint = endpoint;
        _modelId = modelId;
        _apiKey = apiKey;
        _httpClient = httpClient;
        _folderId = folderId;
        _logger = logger ?? NullLogger.Instance;
        _streamJsonParser = new StreamJsonParser();
    }

    /// <summary>
    ///     Retrieves chat message contents asynchronously based on the provided chat history.
    /// </summary>
    /// <param name="chatHistory">The chat history to retrieve message contents from.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <param name="kernel">Optional kernel to use for the operation.</param>
    /// <returns>
    ///     An asynchronous task that returns a list of chat message contents.
    /// </returns>
    internal async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory,
        CancellationToken cancellationToken, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null)
    {
        ValidateChatHistory(chatHistory);

        var modelId = executionSettings?.ModelId ?? _modelId;
        var yandexAiPromptExecutionSettings = YandexAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
        var endpoint = GetEndpoint(yandexAiPromptExecutionSettings, "completion");

        var chatRequest = CreateChatCompletionRequest(modelId, _folderId, false, chatHistory,
            yandexAiPromptExecutionSettings);

        using var httpRequestMessage = CreatePost(chatRequest, endpoint, _apiKey, false, _folderId);
        var responseData = (await SendRequestAsync<ChatCompletionResponseResult>(httpRequestMessage, cancellationToken)
            .ConfigureAwait(false)).Result;

        if (responseData is null) throw new KernelException("Chat completions not found");

        LogUsage(responseData.Usage);

        var responseContent = ToChatMessageContent(modelId, responseData);

        // If we don't want to attempt to invoke any functions, just return the result.
        // Or if we are auto-invoking, but we somehow end up with other than 1 choice even though only 1 was requested, similarly bail.
        if (responseData.Alternatives?.Count != 1) return responseContent;

        var alternative = responseData.Alternatives.FirstOrDefault(); // TODO Handle multiple choices

        if (alternative is null) return responseContent;

        // Add the result message to the caller's chat history;
        // this is required for the service to understand the tool call responses.
        var chatMessageContent = ToChatMessageContent(modelId, responseData, alternative);
        chatHistory.Add(chatMessageContent);

        return responseContent;
    }

    #region private

    /// <summary>
    ///     Converts the chat completion responses into a list of ChatMessageContent objects.
    /// </summary>
    /// <param name="modelId">
    ///     The name of the YandexAI modelId. e.g. "yandexgpt/rc" See
    ///     https://yandex.cloud/ru/docs/foundation-models/concepts/yandexgpt/models
    /// </param>
    /// <param name="response">The chat completion responses to be converted.</param>
    /// <returns>A list of ChatMessageContent objects generated from the provided responses.</returns>
    private static List<ChatMessageContent> ToChatMessageContent(string modelId, ChatCompletionResponses response)
    {
        if (response.Alternatives is null) throw new InvalidOperationException("Cannot get alternatives from json.");

        return response.Alternatives.Select(alternative => ToChatMessageContent(modelId, response, alternative))
            .ToList();
    }

    /// <summary>
    ///     Convert a ChatCompletionAlternative response into a ChatMessageContent object.
    /// </summary>
    /// <param name="modelId">
    ///     The name of the YandexAI modelId. e.g. "yandexgpt/rc" See
    ///     https://yandex.cloud/ru/docs/foundation-models/concepts/yandexgpt/models
    /// </param>
    /// <param name="response">The ChatCompletionResponses object containing alternatives.</param>
    /// <param name="alternative">The specific ChatCompletionAlternative to convert.</param>
    /// <returns>A ChatMessageContent object representing the converted alternative.</returns>
    private static ChatMessageContent ToChatMessageContent(string modelId, ChatCompletionResponses response,
        ChatCompletionAlternative alternative)
    {
        var message = new ChatMessageContent(new AuthorRole(alternative.Message!.Role!),
            alternative.Message!.Text?.ToString(), modelId, alternative, Encoding.UTF8,
            GetChatChoiceMetadata(response, alternative));

        return message;
    }

    /// <summary>
    ///     Retrieves the metadata associated with chat choice for completion response and alternative.
    /// </summary>
    /// <param name="completionResponse">The completion response information.</param>
    /// <param name="alternative">The alternative option for chat message completion.</param>
    /// <returns>
    ///     A dictionary containing metadata for the chat choice, including usage and status.
    /// </returns>
    private static Dictionary<string, object?> GetChatChoiceMetadata(ChatCompletionResponses completionResponse,
        ChatCompletionAlternative alternative)
    {
        return new Dictionary<string, object?>(6)
        {
            { nameof(completionResponse.Usage), completionResponse.Usage },
            { nameof(alternative.Status), alternative.Status }
        };
    }

    /// <summary>
    ///     Creates a HttpRequestMessage with POST method to the specified endpoint using the provided requestData.
    /// </summary>
    /// <param name="requestData">The data to be sent in the POST request.</param>
    /// <param name="endpoint">The URI endpoint to which the POST request will be sent.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="stream">Indicates if the request should be streamed.</param>
    /// <param name="folderId">The ID of the folder.</param>
    /// <returns>A HttpRequestMessage configured for a POST request.</returns>
    private HttpRequestMessage CreatePost(object requestData, Uri endpoint, string apiKey, bool stream, string folderId)
    {
        var httpRequestMessage = HttpRequest.CreatePostRequest(endpoint, requestData);
        SetRequestHeaders(httpRequestMessage, apiKey, stream, folderId);

        return httpRequestMessage;
    }

    /// <summary>
    ///     Sets the necessary headers for the HTTP request including User-Agent, Semantic Kernel Version, Accept header,
    ///     Authorization, x-folder-id, and Content-Type.
    /// </summary>
    /// <param name="request">The HttpRequestMessage to set headers for.</param>
    /// <param name="apiKey">The API key to be included in the Authorization header.</param>
    /// <param name="stream">Specifies if the request expects a stream response.</param>
    /// <param name="folderId">The folder ID to include in the x-folder-id header.</param>
    private void SetRequestHeaders(HttpRequestMessage request, string apiKey, bool stream, string folderId)
    {
        request.Headers.Add("User-Agent", HttpHeaderConstant.Values.UserAgent);
        request.Headers.Add(HttpHeaderConstant.Names.SemanticKernelVersion,
            HttpHeaderConstant.Values.GetAssemblyVersion(GetType()));
        request.Headers.Add("Accept", stream ? "text/event-stream" : "application/json");
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Headers.Add("x-folder-id", folderId);
        request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    }

    /// <summary>
    ///     The identifier of the model used for AI processing.
    /// </summary>
    private readonly string _modelId;

    /// <summary>
    ///     The API key used for authentication and authorization.
    /// </summary>
    private readonly string _apiKey;

    /// <summary>
    ///     Represents the base endpoint URL used for making API requests. If not provided, a default endpoint will be used.
    /// </summary>
    private readonly Uri? _endpoint;

    /// <summary>
    ///     Represents an instance of HttpClient used for making HTTP requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Logger used for logging in the YandexAIClient class.
    /// </summary>
    private readonly ILogger _logger;

    // TODO FR streaming
    /// <summary>
    ///     Internal class for parsing a stream of text which contains a series of discrete JSON strings into an enumerable
    ///     containing each separate JSON string.
    /// </summary>
    /// <remarks>
    ///     This class provides a universal parser for parsing a stream of text containing discrete JSON strings.
    ///     If you need a specialized SSE parser, use Microsoft.SemanticKernel.Text.SseJsonParser instead.
    ///     The StreamJsonParser class is thread-safe.
    /// </remarks>
    private readonly StreamJsonParser _streamJsonParser;

    /// <summary>
    ///     The identifier of the folder in which the chat messages are stored for retrieval.
    /// </summary>
    private readonly string _folderId;

    /// <summary>
    ///     Provider name used for diagnostics.
    /// </summary>
    // TODO FR diagnostics
    private const string ModelProvider = "yandex_ai";

    /// <summary>
    ///     Messages are required and the first prompt role should be user or system.
    /// </summary>
    private static void ValidateChatHistory(ChatHistory chatHistory)
    {
        Verify.NotNull(chatHistory);

        if (chatHistory.Count == 0)
            throw new ArgumentException("Chat history must contain at least one message", nameof(chatHistory));

        var firstRole = chatHistory[0].Role.ToString();
        if (firstRole is not "system" and not "user")
            throw new ArgumentException("The first message in chat history must have either the system or user role",
                nameof(chatHistory));
    }

    /// <summary>
    ///     Log token usage to the logger and metrics.
    /// </summary>
    /// <param name="usage">The Yandex AI usage information containing token usage details.</param>
    private void LogUsage(YandexAIUsage? usage)
    {
        if (usage?.InputTextTokens is null || usage.CompletionTokens is null ||
            usage.TotalTokens is null)
        {
            _logger.LogDebug("Usage information unavailable");
            return;
        }

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(
                "Prompt tokens: {PromptTokens}. Completion tokens: {CompletionTokens}. Total tokens: {TotalTokens}",
                usage.InputTextTokens,
                usage.CompletionTokens,
                usage.TotalTokens);
    }

    /// <summary>
    ///     Retrieves the endpoint based on the provided YandexAIPromptExecutionSettings and path.
    /// </summary>
    /// <param name="executionSettings">The Yandex AI prompt execution settings to determine the API version for the endpoint.</param>
    /// <param name="path">The path to be appended to the generated endpoint URL.</param>
    /// <returns>
    ///     The Uri representing the endpoint for the API request.
    /// </returns>
    private Uri GetEndpoint(YandexAIPromptExecutionSettings executionSettings, string path)
    {
        var endpoint = _endpoint ??
                       new Uri($"https://llm.api.cloud.yandex.net/foundationModels/{executionSettings.ApiVersion}");
        var separator = endpoint.AbsolutePath.EndsWith("/", StringComparison.InvariantCulture) ? string.Empty : "/";
        return new Uri($"{endpoint}{separator}{path}");
    }

    /// <summary>
    ///     Sends an asynchronous HTTP request and returns the deserialized response of type T.
    /// </summary>
    /// <param name="httpRequestMessage">The HTTP request message to be sent.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>The deserialized response of type T from the HTTP request.</returns>
    private async Task<T> SendRequestAsync<T>(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendWithSuccessCheckAsync(httpRequestMessage, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringWithExceptionMappingAsync(cancellationToken)
            .ConfigureAwait(false);

        return DeserializeResponse<T>(body);
    }

    /// <summary>
    ///     Deserializes the JSON response body into the specified type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response body to.</typeparam>
    /// <param name="body">The JSON response body as a string.</param>
    /// <returns>The deserialized object of type <typeparamref name="T" />.</returns>
    private static T DeserializeResponse<T>(string body)
    {
        try
        {
            var deserializedResponse = JsonSerializer.Deserialize<T>(body);
            return deserializedResponse ?? throw new JsonException("Response is null");
        }
        catch (JsonException exc)
        {
            throw new KernelException("Unexpected response from model", exc)
            {
                Data = { { "ResponseData", body } }
            };
        }
    }

    /// <summary>
    ///     Sends an HTTP request asynchronously using the provided <see cref="HttpClient" /> instance and
    ///     returns the <see cref="HttpResponseMessage" /> representing the response.
    /// </summary>
    /// <param name="httpRequestMessage">The <see cref="HttpRequestMessage" /> to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> for canceling the request.</param>
    /// <returns>The <see cref="HttpResponseMessage" /> representing the response.</returns>
    private async Task<HttpResponseMessage> SendStreamingRequestAsync(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        return await _httpClient
            .SendWithSuccessCheckAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates a chat completion request based on the provided parameters.
    /// </summary>
    /// <param name="modelId">
    ///     The name of the YandexAI modelId. e.g. "yandexgpt/rc" See
    ///     https://yandex.cloud/ru/docs/foundation-models/concepts/yandexgpt/models
    /// </param>
    /// <param name="folderId">The folder id required for accessing the YandexAI service</param>
    /// <param name="stream">Indicates if the request should be processed as a stream.</param>
    /// <param name="chatHistory">The chat history to generate completion for.</param>
    /// <param name="executionSettings">Execution settings for the prompt generation.</param>
    /// <returns>A new instance of ChatCompletionRequest based on the provided parameters.</returns>
    private ChatCompletionRequest CreateChatCompletionRequest(string modelId, string folderId, bool stream,
        ChatHistory chatHistory,
        YandexAIPromptExecutionSettings executionSettings)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace("ChatHistory: {ChatHistory}, Settings: {Settings}",
                JsonSerializer.Serialize(chatHistory),
                JsonSerializer.Serialize(executionSettings));

        var request = new ChatCompletionRequest($"gpt://{folderId}/{modelId}")
        {
            Messages = chatHistory.SelectMany(ToYandexChatMessages).ToList(),
            CompletionOptions =
            {
                Temperature = executionSettings.Temperature,
                MaxTokens = executionSettings.MaxTokens,
                ResponseFormat = executionSettings.ResponseFormat,
                Stream = stream
            },
            Stop = executionSettings.Stop
        };

        return request;
    }

    /// <summary>
    ///     Converts given ChatMessageContent to YandexAIChatMessage format. Supports text messages and image URLs. Throws
    ///     NotSupportedException for invalid message content types.
    /// </summary>
    /// <param name="chatMessage">The ChatMessageContent to be converted.</param>
    /// <returns>A list of YandexAIChatMessage objects representing the chat message content.</returns>
    private static List<YandexAIChatMessage> ToYandexChatMessages(ChatMessageContent chatMessage)
    {
        if (chatMessage.Role == AuthorRole.Assistant)
        {
            // Handling function calls supplied via ChatMessageContent.Items collection elements of the FunctionCallContent type.
            var message = new YandexAIChatMessage(chatMessage.Role.ToString(), chatMessage.Content ?? string.Empty);
            return [message];
        }

        if (chatMessage.Items is [TextContent text])
            return [new YandexAIChatMessage(chatMessage.Role.ToString(), text.Text)];

        List<ContentChunk> content = [];
        foreach (var item in chatMessage.Items)
            if (item is TextContent textContent && !string.IsNullOrEmpty(textContent.Text))
                content.Add(new TextChunk(textContent.Text!));
            else if (item is ImageContent imageContent && imageContent.Uri is not null)
                content.Add(new ImageUrlChunk(imageContent.Uri));
            else
                throw new NotSupportedException("Invalid message content, only text and image url are supported.");

        return [new YandexAIChatMessage(chatMessage.Role.ToString(), content)];
    }

    #endregion
}