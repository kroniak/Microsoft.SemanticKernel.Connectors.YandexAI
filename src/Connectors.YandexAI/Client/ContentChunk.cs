using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Client;

/// Represents a content chunk that can be either text or an image URL.
/// /
[JsonDerivedType(typeof(TextChunk))]
[JsonDerivedType(typeof(ImageUrlChunk))]
internal abstract class ContentChunk(ContentChunkType type)
{
    /// <summary>
    ///     Represents the type of content chunk.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = type.ToString();
}

/// <summary>
///     Represents a chunk of text content for chat messages.
/// </summary>
/// <remarks>
///     This class is used to encapsulate text content within chat messages.
/// </remarks>
/// <seealso cref="ContentChunk" />
/// <seealso cref="YandexAIClient" />
/// <seealso cref="YandexAIChatMessage" />
internal class TextChunk(string text) : ContentChunk(ContentChunkType.Text)
{
    /// <summary>
    ///     Represents a content chunk of type Text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = text;
}

/// <summary>
///     Represents a content chunk containing an image URL.
/// </summary>
internal class ImageUrlChunk(Uri imageUrl) : ContentChunk(ContentChunkType.ImageUrl)
{
    /// <summary>
    ///     Represents a ContentChunk that contains an image URL.
    /// </summary>
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = imageUrl.ToString();
}

/// <summary>
///     Represents the type of content chunk, which can be either text or an image URL.
/// </summary>
internal readonly struct ContentChunkType : IEquatable<ContentChunkType>
{
    /// <summary>
    ///     Represents a text chunk within a content, typically used for storing textual information.
    /// </summary>
    public static ContentChunkType Text { get; } = new("text");

    /// <summary>
    ///     Represents an ImageUrlChunk which is a type of ContentChunk containing an image URL.
    /// </summary>
    public static ContentChunkType ImageUrl { get; } = new("image_url");

    /// <summary>
    ///     Represents a specific type for a content chunk, such as text or image URL.
    /// </summary>
    public string Type { get; }

    /// Represents the type of content chunk, such as Text or Image Url.
    /// </summary>
    /// <remarks>
    ///     This is a readonly struct that implements <see cref="IEquatable{ContentChunkType}" />.
    /// </remarks>
    /// <seealso cref="ContentChunkType.Text" />
    /// <seealso cref="ContentChunkType.ImageUrl" />
    /// <seealso cref="ContentChunkType(ContentChunkType)" />
    /// <seealso cref="operator ==(ContentChunkType, ContentChunkType)" />
    /// <seealso cref="operator !=(ContentChunkType, ContentChunkType)" />
    /// <seealso cref="Equals(object?)" />
    /// <seealso cref="Equals(ContentChunkType)" />
    /// <seealso cref="GetHashCode()" />
    /// <seealso cref="ToString()" />
    [JsonConstructor]
    public ContentChunkType(string type)
    {
        Verify.NotNullOrWhiteSpace(type, nameof(type));
        Type = type;
    }

    /// <summary>
    ///     Returns a value indicating whether two <see cref="ContentChunkType" /> instances are equivalent, as determined by a
    ///     case-insensitive comparison of their labels.
    /// </summary>
    /// <param name="left"> the first <see cref="ContentChunkType" /> instance to compare </param>
    /// <param name="right"> the second <see cref="ContentChunkType" /> instance to compare </param>
    /// <returns> true if left and right are both null or have equivalent labels; false otherwise </returns>
    /// <summary>
    ///     Returns a value indicating whether two <see cref="ContentChunkType" /> instances are not equivalent, as determined
    ///     by a
    ///     case-insensitive comparison of their labels.
    /// </summary>
    /// <param name="left"> the first <see cref="ContentChunkType" /> instance to compare </param>
    /// <param name="right"> the second <see cref="ContentChunkType" /> instance to compare </param>
    /// <returns> false if left and right are both null or have equivalent labels; true otherwise </returns>
    public static bool operator ==(ContentChunkType left, ContentChunkType right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Returns a value indicating whether two <see cref="ContentChunkType" /> instances are not equivalent, as determined
    ///     by a case-insensitive comparison of their labels.
    /// </summary>
    /// <param name="left"> The first <see cref="ContentChunkType" /> instance to compare </param>
    /// <param name="right"> The second <see cref="ContentChunkType" /> instance to compare </param>
    /// <returns> False if left and right are both null or have equivalent labels; true otherwise </returns>
    public static bool operator !=(ContentChunkType left, ContentChunkType right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Determines whether the specified object is equal to this <see cref="ContentChunkType" /> instance.
    /// </summary>
    /// <param name="obj">The object to compare to this <see cref="ContentChunkType" />.</param>
    /// <returns>
    ///     True if the specified object is also a <see cref="ContentChunkType" /> and its label is equivalent to this
    ///     instance's label (case-insensitive comparison); otherwise, false.
    /// </returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ContentChunkType otherRole && this == otherRole;
    }

    /// <summary>
    ///     Returns a value indicating whether the current <see cref="ContentChunkType" /> instance is equal to another
    ///     <see cref="ContentChunkType" /> instance,
    ///     as determined by a case-insensitive comparison of their labels.
    /// </summary>
    /// <param name="other">The <see cref="ContentChunkType" /> to compare with the current instance.</param>
    /// <returns>
    ///     True if the current instance and the provided <see cref="ContentChunkType" /> have equivalent labels;
    ///     otherwise, false.
    /// </returns>
    public bool Equals(ContentChunkType other)
    {
        return string.Equals(Type, other.Type, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a
    ///     hash table.
    /// </summary>
    /// <returns> A hash code for the current <see cref="ContentChunkType" /> object. </returns>
    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Type);
    }

    /// <summary>
    ///     Returns the string representation of the ContentChunkType instance.
    /// </summary>
    /// <returns>The type label associated with this ContentChunkType instance.</returns>
    public override string ToString()
    {
        return Type;
    }
}