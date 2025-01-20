using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel.Text;

// ReSharper disable once CheckNamespace
namespace Microsoft.SemanticKernel;

/// <summary>
///     Class to handle creation of HttpRequest messages for various HTTP methods.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class HttpRequest
{
    /// <summary>
    ///     Represents a custom PATCH method for HTTP requests.
    ///     Used in creating HttpRequestMessage for PATCH requests.
    /// </summary>
    private static readonly HttpMethod SPatchMethod = new("PATCH");

    /// <summary>
    ///     Creates a GET request message with optional payload for the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <param name="payload">Optional payload data to include in the request.</param>
    /// <returns>A new instance of <see cref="HttpRequestMessage" /> configured with the specified URL and payload.</returns>
    public static HttpRequestMessage CreateGetRequest(string url, object? payload = null)
    {
        return CreateRequest(HttpMethod.Get, url, payload);
    }

    /// <summary>
    ///     Creates an HTTP POST request with the specified URL and payload.
    /// </summary>
    /// <param name="url">The URL of the request.</param>
    /// <param name="payload">The payload data to be sent with the request.</param>
    /// <returns>An instance of HttpRequestMessage for the POST request with the specified URL and payload.</returns>
    public static HttpRequestMessage CreatePostRequest(string url, object? payload = null)
    {
        return CreateRequest(HttpMethod.Post, url, payload);
    }

    /// <summary>
    ///     Creates an HttpRequestMessage with POST method to the specified endpoint using the provided data.
    /// </summary>
    /// <param name="url">The URL for the request.</param>
    /// <param name="payload">The payload object to be included in the request content (optional).</param>
    /// <returns>An HttpRequestMessage object with POST method, specified URL, and optional payload content.</returns>
    public static HttpRequestMessage CreatePostRequest(Uri url, object? payload = null)
    {
        return CreateRequest(HttpMethod.Post, url, payload);
    }

    /// <summary>
    ///     Creates a PUT HttpRequestMessage with the specified URL and optional payload.
    /// </summary>
    /// <param name="url">The URL to send the PUT request to.</param>
    /// <param name="payload">The optional payload to include in the request body.</param>
    /// <returns>
    ///     A new instance of HttpRequestMessage configured with HttpMethod.Put and content set to JSON representation of
    ///     the payload if provided.
    /// </returns>
    public static HttpRequestMessage CreatePutRequest(string url, object? payload = null)
    {
        return CreateRequest(HttpMethod.Put, url, payload);
    }

    /// <summary>
    ///     Creates a PATCH HTTP request message with the specified URL and payload.
    /// </summary>
    /// <param name="url">The URL of the request.</param>
    /// <param name="payload">Optional payload data to include in the request.</param>
    /// <returns>An HttpRequestMessage instance representing the PATCH request.</returns>
    public static HttpRequestMessage CreatePatchRequest(string url, object? payload = null)
    {
        return CreateRequest(SPatchMethod, url, payload);
    }

    /// <summary>
    ///     Creates a HTTP DELETE request message with the specified URL and optional payload.
    /// </summary>
    /// <param name="url">The URL to send the DELETE request to.</param>
    /// <param name="payload">Optional payload to include in the request. If provided, it will be serialized into JSON format.</param>
    /// <returns>
    ///     Returns a new instance of HttpRequestMessage configured for a HTTP DELETE request with the specified URL and
    ///     payload.
    /// </returns>
    public static HttpRequestMessage CreateDeleteRequest(string url, object? payload = null)
    {
        return CreateRequest(HttpMethod.Delete, url, payload);
    }

    /// <summary>
    ///     Creates an HttpRequestMessage object with the specified HTTP method, URL, and payload content.
    /// </summary>
    /// <param name="method">The HTTP method to be used for the request.</param>
    /// <param name="url">The URL to which the request will be sent.</param>
    /// <param name="payload">The optional payload object to be included in the request content.</param>
    /// <returns>Returns an instance of HttpRequestMessage object with the specified method, URL, and payload.</returns>
    private static HttpRequestMessage CreateRequest(HttpMethod method, string url, object? payload)
    {
        return new HttpRequestMessage(method, url) { Content = CreateJsonContent(payload) };
    }

    /// <summary>
    ///     Creates an HttpRequestMessage with the specified <paramref name="method" />, <paramref name="url" />, and optional
    ///     <paramref name="payload" />.
    ///     The payload will be serialized to JSON and included in the request content if provided.
    /// </summary>
    /// <param name="method">The HTTP method for the request (e.g., HttpMethod.Get, HttpMethod.Post).</param>
    /// <param name="url">The URL for the request.</param>
    /// <param name="payload">The payload object to be included in the request content (optional).</param>
    /// <returns>An HttpRequestMessage object with the specified method, URL, and payload content.</returns>
    private static HttpRequestMessage CreateRequest(HttpMethod method, Uri url, object? payload)
    {
        return new HttpRequestMessage(method, url) { Content = CreateJsonContent(payload) };
    }

    /// <summary>
    ///     Creates a JSON content for an HTTP request payload.
    /// </summary>
    /// <param name="payload">The payload object to be serialized into JSON content. Can be null.</param>
    /// <returns>The HTTP content with JSON serialization of the provided payload object.</returns>
    private static HttpContent? CreateJsonContent(object? payload)
    {
        HttpContent? content = null;
        if (payload is not null)
        {
            var utf8Bytes = payload is string s
                ? Encoding.UTF8.GetBytes(s)
                : JsonSerializer.SerializeToUtf8Bytes(payload, JsonOptionsCache.Default);

            content = new ByteArrayContent(utf8Bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
        }

        return content;
    }
}