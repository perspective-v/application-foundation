using System.Text.RegularExpressions;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides extension methods for String Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by StringExtensions.
/// var typeName = nameof(StringExtensions);
/// ]]></code>
/// </example>
public static partial class StringExtensions {
    public enum TextCase {
        AutoDetect,
        CamelCase,
        PascalCase,
        SnakeCase,
        ScreamingSnakeCase,
        KebabCase,
        TitleCase,
        SentenceCase
    }

    /// <summary>
    /// Executes To Case.
    /// </summary>
    /// <param name="str">The str value.</param>
    /// <param name="textCase">The textCase value.</param>
    /// <returns>The operation result.</returns>
    public static string ToCase(this string str, TextCase textCase) {
        if (string.IsNullOrWhiteSpace(str))
            return str;

        // Auto-detect source format
        if (textCase == TextCase.AutoDetect) {
            if (str.All(c => char.IsUpper(c) || c == '_'))
                textCase = TextCase.ScreamingSnakeCase;
            else if (str.Contains('_'))
                textCase = TextCase.SnakeCase;
            else if (str.Contains('-'))
                textCase = TextCase.KebabCase;
            else if (LowerUpperCaseRegex().IsMatch(str))
                textCase = TextCase.CamelCase;
            else
                textCase = TextCase.SentenceCase;
        }

        // Normalize input: split into words based on case transitions or delimiters
        var words = CamelCaseWordBoundaryRegex()
            .Replace(str, "$1 $2") // camelCase to spaced
            .Replace("_", " ")
            .Replace("-", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToArray();

        switch (textCase) {
            case TextCase.CamelCase:
                return words[0] + string.Concat(words.Skip(1).Select(Capitalize));

            case TextCase.PascalCase:
                return string.Concat(words.Select(Capitalize));

            case TextCase.SnakeCase:
                return string.Join("_", words);

            case TextCase.ScreamingSnakeCase:
                return string.Join("_", words).ToUpperInvariant();

            case TextCase.KebabCase:
                return string.Join("-", words);

            case TextCase.TitleCase:
                return string.Join(" ", words.Select(Capitalize));

            case TextCase.SentenceCase:
                return Capitalize(words[0]) + " " + string.Join(" ", words.Skip(1));

            default:
                return str;
        }

        static string Capitalize(string word) =>
            string.IsNullOrEmpty(word)
                ? word
                : char.ToUpperInvariant(word[0]) + word.Substring(1);
    }

    /// <summary>
    /// Executes To Sentence Case.
    /// </summary>
    /// <param name="str">The str value.</param>
    /// <returns>The operation result.</returns>
    public static string ToSentenceCase(this string str) => str.ToCase(TextCase.SentenceCase);

    /// <summary>
    /// Executes To Camel Case.
    /// </summary>
    /// <param name="str">The str value.</param>
    /// <returns>The operation result.</returns>
    public static string ToCamelCase(this string str) => str.ToCase(TextCase.CamelCase);

    /// <summary>
    /// Executes To Pascal Case.
    /// </summary>
    /// <param name="str">The str value.</param>
    /// <returns>The operation result.</returns>
    public static string ToPascalCase(this string str) => str.ToCase(TextCase.PascalCase);

    /// <summary>
    /// Executes Capitalize.
    /// </summary>
    /// <param name="word">The word value.</param>
    /// <returns>The operation result.</returns>
    private static string Capitalize(string word) => string.IsNullOrEmpty(word) ? word : char.ToUpperInvariant(word[0]) + word.Substring(1);

    [GeneratedRegex(@"[a-z][A-Z]")]
    private static partial Regex LowerUpperCaseRegex();

    [GeneratedRegex(@"([a-z])([A-Z])")]
    private static partial Regex CamelCaseWordBoundaryRegex();
}
