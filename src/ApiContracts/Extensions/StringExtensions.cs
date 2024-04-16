namespace ApiContracts.Extensions;

/// <summary>
/// The string extensions class that handles the string extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// The to camel case extension method that converts the string to camel case.
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>The converted string</returns>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// The to pascal case extension method that converts the string to pascal case.
    /// </summary>
    /// <param name="value">The string value</param>
    /// <returns>The converted string</returns>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}
