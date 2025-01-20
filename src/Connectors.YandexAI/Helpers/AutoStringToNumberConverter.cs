using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.YandexAI.Helpers;

/// <summary>
///     Represents a custom JSON converter for converting integer values stored as strings in JSON format.
/// </summary>
internal class StringJsonConverter : JsonConverter<int?>
{
    /// <summary>
    ///     Reads and converts the input data to an integer.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader object.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The JsonSerializerOptions object.</param>
    /// <returns>The integer value converted from the input data.</returns>
    public override int? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return int.Parse(reader.GetString()!);
    }

    /// <summary>
    ///     Writes the provided integer value as a string to the specified Utf8JsonWriter using the provided
    ///     JsonSerializerOptions.
    /// </summary>
    /// <param name="writer">The Utf8JsonWriter instance to write the value to.</param>
    /// <param name="value">The integer value to be written as a string.</param>
    /// <param name="options">The JsonSerializerOptions to be used during the writing process.</param>
    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}