using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.YandexAI.Client;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.Services;

// ReSharper disable once CheckNamespace
namespace Microsoft.SemanticKernel.Connectors.YandexAI;

// ReSharper disable once InconsistentNaming
/// <summary>
///     Service for chat completion using Yandex AI.
/// </summary>
public sealed class YandexAIChatCompletionService : IChatCompletionService
{
    /// <summary>
    ///     Represents the Yandex AI client associated with YandexAIChatCompletionService for interacting with Yandex AI
    ///     services.
    /// </summary>
    private readonly YandexAIClient _client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="YandexAIChatCompletionService" /> class.
    /// </summary>
    /// <param name="apiKey">API key for accessing the YandexAI service.</param>
    /// <param name="modelId">
    ///     The name of the YandexAI modelId. e.g. "yandexgpt/rc" See
    ///     https://yandex.cloud/ru/docs/foundation-models/concepts/yandexgpt/models
    /// </param>
    /// <param name="folderId">The folder id required for accessing the YandexAI service</param>
    /// <param name="endpoint">
    ///     Optional  uri endpoint including the port where YandexAI server is hosted. Default is
    ///     https://api.mistral.ai.
    /// </param>
    /// <param name="httpClient">Optional HTTP client to be used for communication with the YandexAI API.</param>
    /// <param name="loggerFactory">Optional logger factory to be used for logging.</param>
    public YandexAIChatCompletionService(string modelId, string apiKey, string folderId, Uri? endpoint = null,
        HttpClient? httpClient = null, ILoggerFactory? loggerFactory = null)
    {
        _client = new YandexAIClient(
            modelId,
            endpoint: endpoint ?? httpClient?.BaseAddress,
            apiKey: apiKey,
            folderId: folderId,
            httpClient: HttpClientProvider.GetHttpClient(httpClient),
            logger: loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance
        );

        AttributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }

    /// <summary>
    ///     Represents the internal attributes associated with the YandexAIChatCompletionService.
    /// </summary>
    private Dictionary<string, object?> AttributesInternal { get; } = new();

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => AttributesInternal;

    /// <inheritdoc />
    public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        return _client.GetChatMessageContentsAsync(chatHistory, cancellationToken, executionSettings, kernel);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        // TODO FR streaming
        // => _client.GetStreamingChatMessageContentsAsync(chatHistory, cancellationToken, executionSettings, kernel);
    }
}