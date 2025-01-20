using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.YandexAI;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
// ReSharper disable once InconsistentNaming
internal sealed class YandexAIPromptExecutionSettings : PromptExecutionSettings
{
    private string _apiVersion = "v1";
    private int? _maxTokens;
    private object? _responseFormat;
    private IList<string>? _stop;

    private double _temperature = 0.7;

    /// <summary>
    ///     Gets or sets the stop sequences to use for the completion.
    /// </summary>
    /// <remarks>
    ///     Stop generation if this token is detected. Or if one of these tokens is detected when providing an array
    /// </remarks>
    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? Stop
    {
        get => _stop;

        set
        {
            ThrowIfFrozen();
            _stop = value;
        }
    }

    /// <summary>
    ///     The API version to use.
    /// </summary>
    [JsonPropertyName("api_version")]
    public string ApiVersion
    {
        get => _apiVersion;

        set
        {
            ThrowIfFrozen();
            _apiVersion = value;
        }
    }

    /// <summary>
    ///     Default: 0.7
    ///     What sampling temperature to use, between 0.0 and 1.0. Higher values like 0.8 will make the output more random,
    ///     while lower values like 0.2 will make it more focused and deterministic.
    /// </summary>
    /// <remarks>
    ///     We generally recommend altering this or top_p but not both.
    /// </remarks>
    [JsonPropertyName("temperature")]
    public double Temperature
    {
        get => _temperature;

        set
        {
            ThrowIfFrozen();
            _temperature = value;
        }
    }

    /// <summary>
    ///     Default: null
    ///     The maximum number of tokens to generate in the completion.
    /// </summary>
    /// <remarks>
    ///     The token count of your prompt plus max_tokens cannot exceed the model's context length.
    /// </remarks>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens
    {
        get => _maxTokens;

        set
        {
            ThrowIfFrozen();
            _maxTokens = value;
        }
    }

    /// <summary>
    ///     Gets or sets the response format to use for the completion.
    /// </summary>
    /// <remarks>
    ///     An object specifying the format that the model must output.
    ///     Setting to { "type": "json_object" } enables JSON mode, which guarantees the message the model generates is in
    ///     JSON.
    ///     When using JSON mode you MUST also instruct the model to produce JSON yourself with a system or a user message.
    /// </remarks>
    [JsonPropertyName("response_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ResponseFormat
    {
        get => _responseFormat;

        set
        {
            ThrowIfFrozen();
            _responseFormat = value;
        }
    }

    /// <inheritdoc />
    public override void Freeze()
    {
        if (IsFrozen) return;

        if (_stop is not null) _stop = new ReadOnlyCollection<string>(_stop);

        base.Freeze();
    }

    /// <inheritdoc />
    public override PromptExecutionSettings Clone()
    {
        return new YandexAIPromptExecutionSettings
        {
            ModelId = ModelId,
            ExtensionData = ExtensionData is not null ? new Dictionary<string, object>(ExtensionData) : null,
            Temperature = Temperature,
            MaxTokens = MaxTokens,
            ApiVersion = ApiVersion,
            ResponseFormat = ResponseFormat,
            Stop = Stop is not null ? new List<string>(Stop) : null
        };
    }

    /// <summary>
    ///     Create a new settings object with the values from another settings object.
    /// </summary>
    /// <param name="executionSettings">Template configuration</param>
    /// <returns>An instance of MistralAIPromptExecutionSettings</returns>
    public static YandexAIPromptExecutionSettings FromExecutionSettings(PromptExecutionSettings? executionSettings)
    {
        if (executionSettings is null) return new YandexAIPromptExecutionSettings();

        if (executionSettings is YandexAIPromptExecutionSettings settings) return settings;

        var json = JsonSerializer.Serialize(executionSettings);

        var mistralExecutionSettings =
            JsonSerializer.Deserialize<YandexAIPromptExecutionSettings>(json, JsonOptionsCache.ReadPermissive);
        return mistralExecutionSettings!;
    }
}