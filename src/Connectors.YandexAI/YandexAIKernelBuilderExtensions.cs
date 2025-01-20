using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.YandexAI;
using Microsoft.SemanticKernel.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.SemanticKernel;

// ReSharper disable once InconsistentNaming
[ExcludeFromCodeCoverage]
public static class YandexAIKernelBuilderExtensions
{
    #region Chat Completion

    /// <summary>
    ///     Adds an YandexAI chat completion service with the specified configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IKernelBuilder" /> instance to augment.</param>
    /// <param name="modelId">
    ///     The name of the YandexAI modelId. e.g. "yandexgpt/rc" See
    ///     https://yandex.cloud/ru/docs/foundation-models/concepts/yandexgpt/models
    /// </param>
    /// <param name="apiKey">The API key required for accessing the YandexAI service.</param>
    /// <param name="folderId">The folder id required for accessing the YandexAI service</param>
    /// <param name="endpoint">
    ///     Optional  uri endpoint including the port where YandexAI server is hosted. Default is
    ///     https://llm.api.cloud.yandex.net/foundationModels/.
    /// </param>
    /// <param name="serviceId">A local identifier for the given AI service.</param>
    /// <param name="httpClient">The HttpClient to use with this service.</param>
    /// <returns>The same instance as <paramref name="builder" />.</returns>
    // ReSharper disable once InconsistentNaming
    public static IKernelBuilder AddYandexAIChatCompletion(
        this IKernelBuilder builder,
        string modelId,
        string apiKey,
        string folderId,
        Uri? endpoint = null,
        string? serviceId = null,
        HttpClient? httpClient = null)
    {
        Verify.NotNull(builder);
        Verify.NotNullOrWhiteSpace(modelId);
        Verify.NotNullOrWhiteSpace(apiKey);

        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (serviceProvider, _) =>
            new YandexAIChatCompletionService(modelId, apiKey, folderId, endpoint,
                HttpClientProvider.GetHttpClient(httpClient, serviceProvider),
                serviceProvider.GetService<ILoggerFactory>()));

        return builder;
    }

    #endregion
}