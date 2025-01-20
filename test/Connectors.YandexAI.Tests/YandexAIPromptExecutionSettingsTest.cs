using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SemanticKernel.Connectors.YandexAI;
using Xunit;

namespace Connectors.YandexAI.Tests;

// ReSharper disable once InconsistentNaming
/// <summary>
///     Test class for YandexAIPromptExecutionSettings.
/// </summary>
[TestSubject(typeof(YandexAIPromptExecutionSettings))]
public class YandexAIPromptExecutionSettingsTest
{
    /// <summary>
    ///     Represents the default API version used in Yandex AI Prompt execution settings.
    /// </summary>
    private const string DefaultApiVersion = "v1";

    /// <summary>
    ///     Represents the default temperature used in Yandex AI prompt execution settings.
    /// </summary>
    private const double DefaultTemperature = 0.3;

    /// <summary>
    ///     Verifies that setting <see cref="Stop" /> property to null does not throw an exception.
    /// </summary>
    [Fact]
    public void StopPropertyCheck_WhenSetToNull_ShouldNotThrow()
    {
        var settings = new YandexAIPromptExecutionSettings();

        Assert.Null(settings.Stop);
    }

    /// <summary>
    ///     Method to test if the 'Stop' property is correctly set and returns the set value in
    ///     YandexAIPromptExecutionSettings.
    /// </summary>
    /// <remarks>
    ///     This method sets the 'Stop' property to a specified value and then verifies that the property is not null and
    ///     contains only the set value.
    /// </remarks>
    /// <returns>void</returns>
    [Fact]
    public void StopPropertyCheck_WhenSet_ShouldReturnSetValue()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            Stop = new[] { "End" }
        };

        Assert.NotNull(settings.Stop);
        Assert.Single(settings.Stop, "End");
    }

    /// Method to check if the ApiVersion property is set correctly and should return the set value.
    /// /
    [Fact]
    public void ApiVersionPropertyCheck_WhenSet_ShouldReturnSetValue()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            ApiVersion = "v2"
        };

        Assert.Equal("v2", settings.ApiVersion);
    }

    /// <summary>
    ///     Method to check if the ApiVersion property is set, and return the default value if not set.
    /// </summary>
    [Fact]
    public void ApiVersionPropertyCheck_WhenNotSet_ShouldReturnDefault()
    {
        var settings = new YandexAIPromptExecutionSettings();

        Assert.Equal(DefaultApiVersion, settings.ApiVersion);
    }

    /// <summary>
    ///     Checks if the Temperature property is set correctly and returns the set value.
    /// </summary>
    [Fact]
    public void TemperaturePropertyCheck_WhenSet_ShouldReturnSetValue()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            Temperature = 0.5
        };

        Assert.Equal(0.5, settings.Temperature);
    }

    /// Method to check the default value of the Temperature property when not set.
    /// /
    [Fact]
    public void TemperaturePropertyCheck_WhenNotSet_ShouldReturnDefault()
    {
        var settings = new YandexAIPromptExecutionSettings();

        Assert.Equal(DefaultTemperature, settings.Temperature);
    }

    /// <summary>
    ///     Check if the MaxTokens property is set to null and should return null.
    /// </summary>
    [Fact]
    public void MaxTokensPropertyCheck_WhenSetToNull_ShouldReturnNull()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            MaxTokens = null
        };

        Assert.Null(settings.MaxTokens);
    }

    /// <summary>
    ///     Checks if the MaxTokens property is set correctly and returns the set value.
    /// </summary>
    [Fact]
    public void MaxTokensPropertyCheck_WhenSet_ShouldReturnSetValue()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            MaxTokens = 10
        };

        Assert.Equal(10, settings.MaxTokens);
    }

    /// <summary>
    ///     Checks the behavior of the ResponseFormat property when set to null. It should return null.
    /// </summary>
    /// <remarks>
    ///     This method tests the setting of the ResponseFormat property to null and verifies that it returns null as expected.
    /// </remarks>
    [Fact]
    public void ResponseFormatPropertyCheck_WhenSetToNull_ShouldReturnNull()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            ResponseFormat = null
        };

        Assert.Null(settings.ResponseFormat);
    }

    /// Method: ResponseFormatPropertyCheck_WhenSet_ShouldReturnSetValue
    /// Description: This method tests the behavior of setting and getting the 'ResponseFormat' property in the YandexAIPromptExecutionSettings class.
    /// When the 'ResponseFormat' property is set to a specific value, it should return the same value when retrieved.
    /// Precondition: The YandexAIPromptExecutionSettings instance must be initialized.
    /// Postcondition: The 'ResponseFormat' property of the YandexAIPromptExecutionSettings instance is set to a specific value.
    /// /
    [Fact]
    public void ResponseFormatPropertyCheck_WhenSet_ShouldReturnSetValue()
    {
        var settings = new YandexAIPromptExecutionSettings();

        var obj = new { type = "dummy" };
        settings.ResponseFormat = obj;

        Assert.Equal(obj, settings.ResponseFormat);
    }

    /// <summary>
    ///     Sets all properties of the YandexAIPromptExecutionSettings class as immutable when called.
    ///     This method freezes the object, making its properties read-only and ensuring immutability.
    /// </summary>
    [Fact]
    public void Freeze_WhenCalled_ShouldSetAllPropertiesImmutable()
    {
        var settings = new YandexAIPromptExecutionSettings
        {
            Stop = new[] { "End" }
        };

        settings.Freeze();

        Assert.True(settings.Stop is ReadOnlyCollection<string>);
    }

    /// <summary>
    ///     Method to create a copy of the current object by performing a deep clone operation.
    /// </summary>
    [Fact]
    public void Clone_WhenCalled_ShouldCreateACopyOfObject()
    {
        var settingsOriginal = new YandexAIPromptExecutionSettings
        {
            Stop = new[] { "End" },
            ApiVersion = "v2",
            Temperature = 0.2,
            MaxTokens = 15,
            ResponseFormat = new { type = "dummy" }
        };

        var settingsClone = (YandexAIPromptExecutionSettings)settingsOriginal.Clone();

        Assert.Equal(settingsOriginal.Stop, settingsClone.Stop);
        Assert.Equal(settingsOriginal.ApiVersion, settingsClone.ApiVersion);
        Assert.Equal(settingsOriginal.Temperature, settingsClone.Temperature);
        Assert.Equal(settingsOriginal.MaxTokens, settingsClone.MaxTokens);
        Assert.Equal(settingsOriginal.ResponseFormat, settingsClone.ResponseFormat);
    }

    /// Method to create a copy of YandexAIPromptExecutionSettings based on the provided PromptExecutionSettings object.
    /// If the input executionSettings object is null, a new default YandexAIPromptExecutionSettings object is returned.
    /// Otherwise, a new YandexAIPromptExecutionSettings object is created with properties copied from the input object.
    /// @param executionSettings The PromptExecutionSettings object to create a copy from
    /// @return A new YandexAIPromptExecutionSettings object that is a copy of the input executionSettings
    /// /
    [Fact]
    public void FromExecutionSettings_WithNonNullSettings_ShouldReturnCopyOfSettings()
    {
        var settingsOriginal = new YandexAIPromptExecutionSettings
        {
            Stop = new[] { "End" },
            ApiVersion = "v2",
            Temperature = 0.2,
            MaxTokens = 15,
            ResponseFormat = new { type = "json_object" }
        };

        var settingsCopy = YandexAIPromptExecutionSettings.FromExecutionSettings(settingsOriginal);

        Assert.Equal(settingsOriginal.Stop, settingsCopy.Stop);
        Assert.Equal(settingsOriginal.ApiVersion, settingsCopy.ApiVersion);
        Assert.Equal(settingsOriginal.Temperature, settingsCopy.Temperature);
        Assert.Equal(settingsOriginal.MaxTokens, settingsCopy.MaxTokens);
        Assert.Equal(settingsOriginal.ResponseFormat, settingsCopy.ResponseFormat);
    }

    /// Method to create Yandex AI Prompt Execution Settings based on the given input PromptExecutionSettings.
    /// If the input settings are null, default settings will be returned.
    /// @param executionSettings The input PromptExecutionSettings to create Yandex AI Prompt Execution Settings from.
    /// @return YandexAIPromptExecutionSettings object with values based on input settings or default settings if input is null.
    /// /
    [Fact]
    public void FromExecutionSettings_WithNullSettings_ShouldReturnDefaultSettings()
    {
        var settings = YandexAIPromptExecutionSettings.FromExecutionSettings(null!);

        Assert.Empty(settings.Stop ?? Enumerable.Empty<string>());
        Assert.Equal(DefaultApiVersion, settings.ApiVersion);
        Assert.Equal(DefaultTemperature, settings.Temperature);
        Assert.Null(settings.MaxTokens);
        Assert.Null(settings.ResponseFormat);
    }
}