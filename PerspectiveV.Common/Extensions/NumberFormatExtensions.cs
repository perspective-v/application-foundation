namespace PerspectiveV.Common.Extensions;
using System.Linq;

/// <summary>
/// Provides extension methods for Number Format Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by NumberFormatExtensions.
/// var typeName = nameof(NumberFormatExtensions);
/// ]]></code>
/// </example>
public static class NumberFormatExtensions
{
    public enum NumberFormatType
    {
        Auto,
        Full,
        CompactK,
        CompactM,
        CompactB,
        CompactT,
        CompactQ,
        CompactQQ,
        Scientific,
        Currency,
        CurrencyCompact,
        CurrencyFullWithCompact,
        CurrencyFullWithSuffix
    }

    /// <summary>
    /// Executes To Formatted String.
    /// </summary>
    /// <param name="num">The num value.</param>
    /// <param name="formatType">The formatType value.</param>
    /// <param name="currencySymbolOrCode">The currencySymbolOrCode value.</param>
    /// <param name="minDecimals">The minDecimals value.</param>
    /// <param name="maxDecimals">The maxDecimals value.</param>
    /// <returns>The operation result.</returns>
    public static string ToFormattedString(
        this decimal num,
        NumberFormatType formatType = NumberFormatType.Auto,
        string currencySymbolOrCode = "$",
        int minDecimals = 0,
        int maxDecimals = 2)
    {
        switch (formatType)
        {
            case NumberFormatType.Full:
                return num.ToString($"N{maxDecimals}");

            case NumberFormatType.CompactK:
                return num >= 1_000 ? FormatNumber(num / 1_000, minDecimals, maxDecimals) + "K" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.CompactM:
                return num >= 1_000_000 ? FormatNumber(num / 1_000_000, minDecimals, maxDecimals) + "M" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.CompactB:
                return num >= 1_000_000_000 ? FormatNumber(num / 1_000_000_000, minDecimals, maxDecimals) + "B" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.CompactT:
                return num >= 1_000_000_000_000 ? FormatNumber(num / 1_000_000_000_000, minDecimals, maxDecimals) + "T" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.CompactQ:
                return num >= 1_000_000_000_000_000 ? FormatNumber(num / 1_000_000_000_000_000, minDecimals, maxDecimals) + "Q" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.CompactQQ:
                return num >= 1_000_000_000_000_000_000 ? FormatNumber(num / 1_000_000_000_000_000_000, minDecimals, maxDecimals) + "QQ" : FormatNumber(num, minDecimals, maxDecimals);

            case NumberFormatType.Scientific:
                return num.ToString($"E{maxDecimals}");

            case NumberFormatType.Currency:
                return FormatCurrency(num, currencySymbolOrCode, minDecimals, maxDecimals);

            case NumberFormatType.CurrencyCompact:
                return FormatCurrencyPrefixOnly(ToFormattedString(num, NumberFormatType.Auto, currencySymbolOrCode, minDecimals, maxDecimals), currencySymbolOrCode);

            case NumberFormatType.CurrencyFullWithCompact:
                return $"{FormatCurrency(num, currencySymbolOrCode, minDecimals, maxDecimals)} ({ToFormattedString(num, NumberFormatType.Auto, currencySymbolOrCode, minDecimals, maxDecimals)})";

            case NumberFormatType.CurrencyFullWithSuffix:
                return $"{FormatCurrency(num, currencySymbolOrCode, minDecimals, maxDecimals)} {GetSuffix(num)}".Trim();

            case NumberFormatType.Auto:
                return GetAutoFormatted(num, minDecimals, maxDecimals);

            default:
                return num.ToString();
        }
    }

    /// <summary>
    /// Gets Get Auto Formatted.
    /// </summary>
    /// <param name="num">The num value.</param>
    /// <param name="minDecimals">The minDecimals value.</param>
    /// <param name="maxDecimals">The maxDecimals value.</param>
    /// <returns>The operation result.</returns>
    private static string GetAutoFormatted(decimal num, int minDecimals, int maxDecimals)
    {
        if (num >= 1_000_000_000_000_000_000) return FormatNumber(num / 1_000_000_000_000_000_000, minDecimals, maxDecimals) + "QQ";
        if (num >= 1_000_000_000_000_000) return FormatNumber(num / 1_000_000_000_000_000, minDecimals, maxDecimals) + "Q";
        if (num >= 1_000_000_000_000) return FormatNumber(num / 1_000_000_000_000, minDecimals, maxDecimals) + "T";
        if (num >= 1_000_000_000) return FormatNumber(num / 1_000_000_000, minDecimals, maxDecimals) + "B";
        if (num >= 1_000_000) return FormatNumber(num / 1_000_000, minDecimals, maxDecimals) + "M";
        if (num >= 1_000) return FormatNumber(num / 1_000, minDecimals, maxDecimals) + "K";
        return FormatNumber(num, minDecimals, maxDecimals);
    }

    /// <summary>
    /// Gets Get Suffix.
    /// </summary>
    /// <param name="num">The num value.</param>
    /// <returns>The operation result.</returns>
    private static string GetSuffix(decimal num)
    {
        if (num >= 1_000_000_000_000_000_000) return "QQ";
        if (num >= 1_000_000_000_000_000) return "Q";
        if (num >= 1_000_000_000_000) return "T";
        if (num >= 1_000_000_000) return "B";
        if (num >= 1_000_000) return "M";
        if (num >= 1_000) return "K";
        return string.Empty;
    }

    /// <summary>
    /// Executes Format Number.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <param name="minDecimals">The minDecimals value.</param>
    /// <param name="maxDecimals">The maxDecimals value.</param>
    /// <returns>The operation result.</returns>
    private static string FormatNumber(decimal value, int minDecimals, int maxDecimals)
    {
        return value.ToString($"N{maxDecimals}").TrimEnd('0').TrimEnd('.');
    }

    /// <summary>
    /// Executes Format Currency.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <param name="symbolOrCode">The symbolOrCode value.</param>
    /// <param name="minDecimals">The minDecimals value.</param>
    /// <param name="maxDecimals">The maxDecimals value.</param>
    /// <returns>The operation result.</returns>
    private static string FormatCurrency(decimal value, string symbolOrCode, int minDecimals, int maxDecimals)
    {
        if (string.IsNullOrEmpty(symbolOrCode))
            return value.ToString($"C{maxDecimals}");

        if (symbolOrCode.Length == 3 && symbolOrCode.All(char.IsLetter))
            return value.ToString($"C{maxDecimals}", new System.Globalization.CultureInfo("en-US")).Replace("$", symbolOrCode + " ");

        return $"{symbolOrCode}{FormatNumber(value, minDecimals, maxDecimals)}";
    }

    /// <summary>
    /// Executes Format Currency Prefix Only.
    /// </summary>
    /// <param name="formattedNumber">The formattedNumber value.</param>
    /// <param name="symbolOrCode">The symbolOrCode value.</param>
    /// <returns>The operation result.</returns>
    private static string FormatCurrencyPrefixOnly(string formattedNumber, string symbolOrCode)
    {
        if (string.IsNullOrEmpty(symbolOrCode)) return $"${formattedNumber}";
        if (symbolOrCode.Length == 3 && symbolOrCode.All(char.IsLetter)) return $"{symbolOrCode} {formattedNumber}";
        return $"{symbolOrCode}{formattedNumber}";
    }
}
