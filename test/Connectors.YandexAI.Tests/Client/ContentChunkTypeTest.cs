using System;
using JetBrains.Annotations;
using Microsoft.SemanticKernel.Connectors.YandexAI.Client;
using Xunit;

namespace Connectors.YandexAI.Tests.Client;

/// <summary>
///     Represents a test class for ContentChunkType.
/// </summary>
[TestSubject(typeof(ContentChunkType))]
public class ContentChunkTypeTest
{
    /// <summary>
    ///     Throws an exception when the type parameter is null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsException_WhenTypeIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ContentChunkType(null));
    }

    /// <summary>
    ///     Constructor that sets the type of the ContentChunkType object.
    /// </summary>
    /// <param name="type">The type of the content chunk (e.g., text or image URL).</param>
    /// <remarks>
    ///     This constructor initializes a new instance of the ContentChunkType struct with the specified type.
    /// </remarks>
    /// <seealso cref="ContentChunkType.Text" />
    /// <seealso cref="ContentChunkType.ImageUrl" />
    /// <seealso cref="operator ==(ContentChunkType, ContentChunkType)" />
    /// <seealso cref="operator !=(ContentChunkType, ContentChunkType)" />
    /// <seealso cref="Equals(object?)" />
    /// <seealso cref="Equals(ContentChunkType)" />
    /// <seealso cref="GetHashCode()" />
    /// <seealso cref="ToString()" />
    [Fact]
    public void Constructor_SetsType()
    {
        var contentChunkType = new ContentChunkType("text");
        Assert.Equal("text", contentChunkType.Type);
    }

    /// Determines if the equality operator returns true when both instances are the same ContentChunkType.
    /// @param type The type of the ContentChunkType.
    /// @returns true if the equality operator returns true when both instances are the same ContentChunkType, false otherwise.
    /// /
    [Fact]
    public void EqualityOperator_ReturnsTrue_WhenBothAreSame()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("text");
        Assert.True(contentChunkType1 == contentChunkType2);
    }

    /// <summary>
    ///     Determines if the equality operator returns false when comparing two instances of ContentChunkType that have
    ///     different values.
    /// </summary>
    [Fact]
    public void EqualityOperator_ReturnsFalse_WhenBothAreDifferent()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("image_url");
        Assert.False(contentChunkType1 == contentChunkType2);
    }

    /// <summary>
    ///     Determines if the inequality operator returns false when both ContentChunkType instances are the same.
    /// </summary>
    /// <remarks>
    ///     This test verifies that when two ContentChunkType instances have the same type,
    ///     the inequality operator (!=) returns false.
    /// </remarks>
    [Fact]
    public void InequalityOperator_ReturnsFalse_WhenBothAreSame()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("text");
        Assert.False(contentChunkType1 != contentChunkType2);
    }

    /// <summary>
    ///     Determines whether two ContentChunkType instances are different.
    /// </summary>
    /// <returns>
    ///     True if the two instances are different, false otherwise.
    /// </returns>
    [Fact]
    public void InequalityOperator_ReturnsTrue_WhenBothAreDifferent()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("image_url");
        Assert.True(contentChunkType1 != contentChunkType2);
    }

    /// <summary>
    ///     Determines whether the current ContentChunkType instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
    [Fact]
    public void Equals_Object_ReturnsTrue_WhenSame()
    {
        var contentChunkType1 = new ContentChunkType("text");
        object contentChunkType2 = new ContentChunkType("text");
        Assert.True(contentChunkType1.Equals(contentChunkType2));
    }

    /// <summary>
    ///     Determines whether the current <see cref="ContentChunkType" /> object is equal to another
    ///     <see cref="ContentChunkType" /> object.
    ///     Returns true if the type of both content chunk types is the same; otherwise, false.
    /// </summary>
    /// <remarks>
    ///     Equality is determined by comparing the <see cref="Type" /> property of the content chunk types.
    /// </remarks>
    /// <returns>A boolean value indicating equality between the two content chunk types.</returns>
    [Fact]
    public void Equals_ContentChunkType_ReturnsTrue_WhenSame()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("text");
        Assert.True(contentChunkType1.Equals(contentChunkType2));
    }

    /// <summary>
    ///     Determines whether the GetHashCode method returns the same value for instances of the same type.
    /// </summary>
    /// <remarks>
    ///     It verifies that for two instances of the same ContentChunkType with the same type value,
    ///     the GetHashCode method returns the same hash code value.
    /// </remarks>
    [Fact]
    public void GetHashCode_ReturnsSame_WhenSameType()
    {
        var contentChunkType1 = new ContentChunkType("text");
        var contentChunkType2 = new ContentChunkType("text");
        Assert.Equal(contentChunkType1.GetHashCode(), contentChunkType2.GetHashCode());
    }

    /// <summary>
    ///     Returns the string representation of the ContentChunkType instance, which is the type of content chunk.
    /// </summary>
    /// <returns>A string representing the type of the content chunk.</returns>
    [Fact]
    public void ToString_ReturnsCorrectType()
    {
        var contentChunkType = new ContentChunkType("text");
        Assert.Equal("text", contentChunkType.ToString());
    }
}