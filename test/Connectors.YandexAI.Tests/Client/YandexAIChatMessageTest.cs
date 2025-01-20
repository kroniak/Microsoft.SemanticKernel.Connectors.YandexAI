using System;
using Microsoft.SemanticKernel.Connectors.YandexAI.Client;
using Xunit;

namespace Connectors.YandexAI.Tests.Client;

/// <summary>
///     Test class for testing the YandexAIChatMessage class.
/// </summary>
public class YandexAiChatMessageTest
{
    // Invalid roles
    /// <summary>
    ///     Represents an invalid role that cannot be used in a YandexAI chat message.
    /// </summary>
    public static readonly TheoryData<string> InvalidRoles =
    [
        "admin",
        "test",
        "operation",
        "invalid-role",
        ""
    ];

    // Valid roles
    /// <summary>
    ///     Chat message role for YandexAI. Must be one of: system, user, assistant, tool.
    /// </summary>
    public static readonly TheoryData<string> ValidRoles =
    [
        "system",
        "user",
        "assistant",
        "tool"
    ];

    /// <summary>
    ///     Chat message for YandexAI.
    ///     Construct an instance of <see cref="YandexAIChatMessage" />.
    /// </summary>
    /// <param name="role">If provided must be one of: system, user, assistant</param>
    /// <param name="text">Text of the chat message</param>
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.ChatCompletionResponses" />
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.ContentChunk" />
    /// <seealso cref="Microsoft.SemanticKernel.Connectors.YandexAI.Client.YandexAIClient" />
    [Theory]
    [MemberData(nameof(InvalidRoles))]
    public void Constructor_ShouldThrowForInvalidRoles(string role)
    {
        Assert.Throws<ArgumentException>(() => new YandexAIChatMessage(role, "Test Message"));
    }

    /// <summary>
    ///     Verifies that the constructor does not throw an exception for valid roles.
    /// </summary>
    /// <param name="role">The role of the chat message. Must be one of: system, user, assistant</param>
    [Theory]
    [MemberData(nameof(ValidRoles))]
    public void Constructor_ShouldNotThrowForValidRoles(string role)
    {
        var exception = Record.Exception(() => new YandexAIChatMessage(role, "Test Message"));

        Assert.Null(exception);
    }

    /// <summary>
    ///     Constructor_ShouldNotThrowForNullRole unit test method.
    ///     Validates that the constructor of YandexAIChatMessage does not throw an exception when a null role is provided.
    /// </summary>
    [Fact]
    public void Constructor_ShouldNotThrowForNullRole()
    {
        var exception = Record.Exception(() => new YandexAIChatMessage(null, "Test Message"));

        Assert.Null(exception);
    }

    /// <summary>
    ///     Constructor_ShouldSetPropertiesCorrectly.
    ///     Verifies that the YandexAIChatMessage constructor correctly sets the Role and Text properties based on the input
    ///     parameters.
    /// </summary>
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        const string role = "system";
        const string text = "Test Message";

        var message = new YandexAIChatMessage(role, text);

        Assert.Equal(role, message.Role);
        Assert.Equal(text, message.Text);
    }

    /// <summary>
    ///     Constructor_ShouldNotThrowForNullText - Verifies that the constructor of YandexAIChatMessage does not throw an
    ///     exception when the text parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_ShouldNotThrowForNullText()
    {
        const string role = "system";

        var exception = Record.Exception(() => new YandexAIChatMessage(role, null));

        Assert.Null(exception);
    }
}