using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.SemanticKernel;

/// <summary>
///     Verify class contains static methods for performing various input validation checks.
/// </summary>
[ExcludeFromCodeCoverage]
internal static partial class Verify
{
#if NET
    /// <summary>
    ///     Retrieves the regular expression for matching ASCII letters, digits, and underscores.
    /// </summary>
    /// <returns>The regular expression for matching ASCII letters, digits, and underscores.</returns>
    [GeneratedRegex("^[0-9A-Za-z_]*$")]
    private static partial Regex AsciiLettersDigitsUnderscoresRegex();

    /// <summary>
    ///     Retrieves the Regex pattern for validating filenames.
    /// </summary>
    /// <returns>The Regex pattern object for validating filenames.</returns>
    [GeneratedRegex("^[^.]+\\.[^.]+$")]
    private static partial Regex FilenameRegex();
#else
    private static Regex AsciiLettersDigitsUnderscoresRegex() => s_asciiLettersDigitsUnderscoresRegex;
    private static readonly Regex s_asciiLettersDigitsUnderscoresRegex = new("^[0-9A-Za-z_]*$", RegexOptions.Compiled);

    private static Regex FilenameRegex() => s_filenameRegex;
    private static readonly Regex s_filenameRegex = new("^[^.]+\\.[^.]+$", RegexOptions.Compiled);
#endif

    /// <summary>
    ///     Equivalent of ArgumentNullException.ThrowIfNull
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NotNull([NotNull] object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? paramName = null)
    {
#if NET
        ArgumentNullException.ThrowIfNull(obj, paramName);
#else
        if (obj is null)
        {
            ThrowArgumentNullException(paramName);
        }
#endif
    }

    /// <summary>
    ///     Throws an ArgumentException if the provided string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="str">The string to be checked for null or white-space characters.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NotNullOrWhiteSpace([NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
        string? paramName = null)
    {
#if NET
        ArgumentException.ThrowIfNullOrWhiteSpace(str, paramName);
#else
        NotNull(str, paramName);
        if (string.IsNullOrWhiteSpace(str))
        {
            ThrowArgumentWhiteSpaceException(paramName);
        }
#endif
    }

    /// <summary>
    ///     Check if the given list is not null and not empty. Throws an ArgumentException if the list is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to be checked.</param>
    /// <param name="paramName">The name of the parameter.</param>
    internal static void NotNullOrEmpty<T>(IList<T> list,
        [CallerArgumentExpression(nameof(list))]
        string? paramName = null)
    {
        NotNull(list, paramName);
        if (list.Count == 0) throw new ArgumentException("The value cannot be empty.", paramName);
    }

    /// <summary>
    ///     Throws an ArgumentException if the provided condition is false with a custom message.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">The error message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter associated with the condition (automatically provided if not
    ///     specified).
    /// </param>
    public static void True(bool condition, string message,
        [CallerArgumentExpression(nameof(condition))]
        string? paramName = null)
    {
        if (!condition) throw new ArgumentException(message, paramName);
    }

    /// <summary>
    ///     Validates a plugin name to ensure it only contains ASCII letters, digits, and underscores.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to be validated.</param>
    /// <param name="plugins">An optional collection of existing plugins to check for name uniqueness.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    internal static void ValidPluginName([NotNull] string? pluginName, IReadOnlyKernelPluginCollection? plugins = null,
        [CallerArgumentExpression(nameof(pluginName))]
        string? paramName = null)
    {
        NotNullOrWhiteSpace(pluginName);
        if (!AsciiLettersDigitsUnderscoresRegex().IsMatch(pluginName))
            ThrowArgumentInvalidName("plugin name", pluginName, paramName);

        if (plugins is not null && plugins.Contains(pluginName))
            throw new ArgumentException($"A plugin with the name '{pluginName}' already exists.");
    }

    /// <summary>
    ///     Validates the given function name to ensure it consists only of ASCII letters, digits, and underscores.
    ///     Throws an ArgumentException if the name is null, empty, or contains invalid characters.
    /// </summary>
    /// <param name="functionName">The function name to validate. Must not be null or whitespace.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    internal static void ValidFunctionName([NotNull] string? functionName,
        [CallerArgumentExpression(nameof(functionName))]
        string? paramName = null)
    {
        NotNullOrWhiteSpace(functionName);
        if (!AsciiLettersDigitsUnderscoresRegex().IsMatch(functionName))
            ThrowArgumentInvalidName("function name", functionName, paramName);
    }

    /// <summary>
    ///     Validates the provided filename by checking if it is not null or white space and matches the expected file naming
    ///     convention.
    /// </summary>
    /// <param name="filename">The filename to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    internal static void ValidFilename([NotNull] string? filename,
        [CallerArgumentExpression(nameof(filename))]
        string? paramName = null)
    {
        NotNullOrWhiteSpace(filename);
        if (!FilenameRegex().IsMatch(filename))
            throw new ArgumentException(
                $"Invalid filename format: '{filename}'. Filename should consist of an actual name and a file extension.",
                paramName);
    }

    /// <summary>
    ///     Validates the provided URL to ensure it is a valid absolute URL and meets certain criteria.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <param name="allowQuery">Indicates whether query parameters are allowed in the URL.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    public static void ValidateUrl(string url, bool allowQuery = false,
        [CallerArgumentExpression(nameof(url))]
        string? paramName = null)
    {
        NotNullOrWhiteSpace(url, paramName);

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(uri.Host))
            throw new ArgumentException($"The `{url}` is not valid.", paramName);

        if (!allowQuery && !string.IsNullOrEmpty(uri.Query))
            throw new ArgumentException($"The `{url}` is not valid: it cannot contain query parameters.", paramName);

        if (!string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException($"The `{url}` is not valid: it cannot contain URL fragments.", paramName);
    }

    /// <summary>
    ///     Checks if a given <paramref name="text" /> string starts with a specified <paramref name="prefix" /> string in a
    ///     case-insensitive manner.
    /// </summary>
    /// <param name="text">The input string to check.</param>
    /// <param name="prefix">The prefix to check for at the beginning of the <paramref name="text" />.</param>
    /// <param name="message">The error message to include in the exception if the prefix check fails.</param>
    /// <param name="textParamName">The name of the <paramref name="text" /> parameter.</param>
    internal static void StartsWith([NotNull] string? text, string prefix, string message,
        [CallerArgumentExpression(nameof(text))]
        string? textParamName = null)
    {
        Debug.Assert(prefix is not null);

        NotNullOrWhiteSpace(text, textParamName);
        if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(textParamName, message);
    }

    /// <summary>
    ///     Checks if a directory exists at the specified path.
    ///     Throws a DirectoryNotFoundException if the directory does not exist.
    /// </summary>
    /// <param name="path">The full path of the directory to check for existence.</param>
    internal static void DirectoryExists(string path)
    {
        if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Directory '{path}' could not be found.");
    }

    /// <summary>
    ///     Make sure every function parameter name in the list is unique
    /// </summary>
    /// <param name="parameters">List of KernelParameterMetadata objects representing function parameters</param>
    internal static void ParametersUniqueness(IReadOnlyList<KernelParameterMetadata> parameters)
    {
        var count = parameters.Count;
        if (count > 0)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < count; i++)
            {
                var p = parameters[i];
                if (string.IsNullOrWhiteSpace(p.Name))
                {
                    var paramName = $"{nameof(parameters)}[{i}].{p.Name}";
                    if (p.Name is null)
                        ThrowArgumentNullException(paramName);
                    else
                        ThrowArgumentWhiteSpaceException(paramName);
                }

                if (!seen.Add(p.Name))
                    throw new ArgumentException(
                        $"The function has two or more parameters with the same name '{p.Name}'");
            }
        }
    }

    /// <summary>
    ///     Throws an ArgumentException indicating that a certain kind of argument name is invalid.
    /// </summary>
    /// <param name="kind">The kind of argument name that is considered invalid.</param>
    /// <param name="name">The name of the invalid argument.</param>
    /// <param name="paramName">The name of the parameter associated with the argument (optional).</param>
    [DoesNotReturn]
    private static void ThrowArgumentInvalidName(string kind, string name, string? paramName)
    {
        throw new ArgumentException(
            $"A {kind} can contain only ASCII letters, digits, and underscores: '{name}' is not a valid name.",
            paramName);
    }

    /// <summary>
    ///     Throws an ArgumentNullException with the specified parameter name.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    [DoesNotReturn]
    internal static void ThrowArgumentNullException(string? paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    /// <summary>
    ///     Throws an ArgumentException with a message stating that the value cannot be an empty string or composed entirely of
    ///     whitespace.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    [DoesNotReturn]
    internal static void ThrowArgumentWhiteSpaceException(string? paramName)
    {
        throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.",
            paramName);
    }

    /// <summary>
    ///     Throws an ArgumentOutOfRangeException with a specified error message and the name of the parameter that causes the
    ///     exception,
    ///     as well as the actual value that was out of range.
    /// </summary>
    /// <typeparam name="T">The type of the actual value causing the exception.</typeparam>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <param name="actualValue">The actual value that is out of range.</param>
    /// <param name="message">The message that describes the error.</param>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException<T>(string? paramName, T actualValue, string message)
    {
        throw new ArgumentOutOfRangeException(paramName, actualValue, message);
    }
}