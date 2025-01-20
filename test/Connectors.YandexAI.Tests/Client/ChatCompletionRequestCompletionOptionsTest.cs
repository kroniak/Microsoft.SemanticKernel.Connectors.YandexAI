using JetBrains.Annotations;
using Microsoft.SemanticKernel.Connectors.YandexAI.Client;
using Xunit;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Connectors.YandexAI.Tests.Client;

/// <summary>
/// Represents options for completing chat requests.
/// </summary>
/// <remarks>
/// This class provides properties to set various options for completing chat requests, such as temperature, max tokens,
/// streaming behavior, response format, and reasoning options.
/// </remarks>
[TestSubject(typeof(ChatCompletionRequestCompletionOptions))]
public class ChatCompletionRequestCompletionOptionsTest
{
    /// <summary>
    /// Checks if the given temperature value is between 0 and 1.
    /// </summary>
    /// <param name="value">The temperature value to check.</param>
    [Theory]
    [InlineData(0.1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Temperature_Should_Be_Between_0_and_1(double value)
    {
        var options = new ChatCompletionRequestCompletionOptions { Temperature = value };
        Assert.Equal(value, options.Temperature);
    }

    /// <summary>
    /// Set the default temperature value for completing chat requests to 0.3.
    /// </summary>
    [Fact]
    public void Temperature_Should_Default_To_0_3()
    {
        var options = new ChatCompletionRequestCompletionOptions();
        Assert.Equal(0.3, options.Temperature);
    }

    /// Represents tests for the default behavior of the MaxTokens property in ChatCompletionRequestCompletionOptions.
    /// /
    [Fact]
    public void MaxTokens_Should_Be_Null_By_Default()
    {
        var options = new ChatCompletionRequestCompletionOptions();
        Assert.Null(options.MaxTokens);
    }

    /// <summary>
    /// <param name="value">The integer value to set as the max tokens for completing chat requests.</param>
    /// </summary>
    [Theory]
    [InlineData(10)]
    [InlineData(null)]
    public void MaxTokens_Should_Accept_Integer_Value(int? value)
    {
        var options = new ChatCompletionRequestCompletionOptions { MaxTokens = value };
        Assert.Equal(value, options.MaxTokens);
    }

    /// <summary>
    /// Specifies if the stream should default to false for completing chat requests.
    /// </summary>
    [Fact]
    public void Stream_Should_Default_To_False()
    {
        var options = new ChatCompletionRequestCompletionOptions();
        Assert.False(options.Stream);
    }

    /// <summary>
    /// Sets the value indicating whether stream should be accepted in the chat completion request options.
    /// </summary>
    /// <param name="value">A boolean value indicating if stream should be accepted.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Stream_Should_Accept_Boolean_Value(bool value)
    {
        var options = new ChatCompletionRequestCompletionOptions { Stream = value };
        Assert.Equal(value, options.Stream);
    }

    /// <summary>
    /// Ensures that the ResponseFormat property is null by default in the ChatCompletionRequestCompletionOptions class.
    /// </summary>
    [Fact]
    public void ResponseFormat_Should_Be_Null_By_Default()
    {
        var options = new ChatCompletionRequestCompletionOptions();
        Assert.Null(options.ResponseFormat);
    }

    /// <summary>
    /// Specifies that the ReasoningOptions property should be null by default.
    /// </summary>
    [Fact]
    public void ReasoningOptions_Should_Be_Null_By_Default()
    {
        var options = new ChatCompletionRequestCompletionOptions();
        Assert.Null(options.ReasoningOptions);
    }

    /// <summary>
    /// Accepts a string value for reasoning options.
    /// </summary>
    /// <param name="value">The reasoning options string to set.</param>
    [Theory]
    [InlineData("REASONING_MODE_UNSPECIFIED")]
    [InlineData("DISABLED")]
    [InlineData("ENABLED_HIDDEN")]
    [InlineData(null)]
    public void ReasoningOptions_Should_Accept_String_Value(string value)
    {
        var options = new ChatCompletionRequestCompletionOptions { ReasoningOptions = value };
        Assert.Equal(value, options.ReasoningOptions);
    }
}