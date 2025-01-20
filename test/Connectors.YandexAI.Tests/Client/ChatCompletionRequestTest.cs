using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.SemanticKernel.Connectors.YandexAI.Client;
using Xunit;

namespace Connectors.YandexAI.Tests.Client;

/// <summary>
/// Request for Chat Completion containing information for generating chat completions.
/// </summary>
[TestSubject(typeof(ChatCompletionRequest))]
public class ChatCompletionRequestTest
{
    /// <summary>
    /// Construct a new instance of <see cref="ChatCompletionRequest"/> with the specified model ID.
    /// </summary>
    /// <param name="model">The ID of the model to use for chat completion.</param>
    [Fact]
    public void ChatCompletionRequestConstructorShouldSetModelCorrectly()
    {
        var modelUri = "testURI";
        var chatCompletion = new ChatCompletionRequest(modelUri);
        Assert.Equal(modelUri, chatCompletion.Model);
    }

    /// <summary>
    /// Method to test the behavior of the CompletionOptions property in the ChatCompletionRequest class.
    /// It verifies that the property is not null after initialization,
    /// and that it can be set to a new instance of ChatCompletionRequestCompletionOptions successfully.
    /// </summary>
    /// <remarks>
    /// This test is part of the ChatCompletionRequestTest class for unit testing purposes.
    /// </remarks>
    /// <seealso cref="ChatCompletionRequestTest"/>
    [Fact]
    public void ChatCompletionRequestCompletionOptionsPropertyShouldBehaveCorrectly()
    {
        var chatCompletion = new ChatCompletionRequest("testURI");
        Assert.NotNull(chatCompletion.CompletionOptions);

        var newOptions = new ChatCompletionRequestCompletionOptions { Temperature = 0.5 };
        chatCompletion.CompletionOptions = newOptions;
        Assert.Equal(0.5, chatCompletion.CompletionOptions.Temperature);
    }

    /// <summary>
    /// Ensures the stop property of the chat completion request behaves correctly.
    /// The stop property enables users to specify phrases where generation should end.
    /// </summary>
    /// <remarks>
    /// This property should be null by default.
    /// Users can set stop phrases as a list of strings to define points where generation should stop.
    /// </remarks>
    [Fact]
    public void ChatCompletionRequestStopPropertyShouldBehaveCorrectly()
    {
        var chatCompletion = new ChatCompletionRequest("testURI");
        Assert.Null(chatCompletion.Stop);

        var stopStrings = new List<string> { "testStop" };
        chatCompletion.Stop = stopStrings;
        Assert.Single(chatCompletion.Stop, "testStop");
    }

    /// <summary>
    /// Ensure that the "Messages" property behaves correctly in the ChatCompletionRequest class.
    /// </summary>
    [Fact]
    public void ChatCompletionRequestMessagesPropertyShouldBehaveCorrectly()
    {
        var chatCompletion = new ChatCompletionRequest("testURI");
        Assert.Empty(chatCompletion.Messages);
    }

    /// <summary>
    /// Add a message to the list of messages in the Chat Completion request.
    /// </summary>
    /// <param name="message">The message to add to the request.</param>
    [Fact]
    public void ChatCompletionRequestAddMessageShouldAddAMessage()
    {
        var chatCompletion = new ChatCompletionRequest("testURI");
        Assert.Empty(chatCompletion.Messages);

        var chatMessage = new YandexAIChatMessage("user", "hello");
        chatCompletion.AddMessage(chatMessage);
        Assert.Single(chatCompletion.Messages, message => message.Role == "user" && message.Text.ToString() == "hello");
    }

    /// <summary>
    /// Ensures that adding a null message to the ChatCompletionRequest throws an ArgumentNullException.
    /// </summary>
    [Fact]
    public void ChatCompletionRequestShouldThrowOnAddNullMessage()
    {
        var chatCompletion = new ChatCompletionRequest("testURI");
        Assert.Throws<ArgumentNullException>(() => chatCompletion.AddMessage(null!));
    }
}