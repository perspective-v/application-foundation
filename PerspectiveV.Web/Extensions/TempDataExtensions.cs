using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace PerspectiveV.Web.Extensions;

/// <summary>
/// Adds typed read/write helpers and common message shortcuts for ASP.NET Core TempData.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Store and retrieve typed values from TempData.
/// TempData.Set("SelectedTab", "users");
/// var selectedTab = TempData.Get&lt;string&gt;("SelectedTab");
/// ]]></code>
/// </example>
public static class TempDataExtensions {
    private const string MSG = "MSG";
    private const string Error = "ERROR";

    /// <summary>
    /// Stores a value in TempData under the specified key.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <param name="key">Key used to store the value.</param>
    /// <param name="itm">Value to store.</param>
    public static void Set<T>(this ITempDataDictionary dic, string key, T itm) {
        ArgumentNullException.ThrowIfNull(dic);
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty", nameof(key));

        dic[key] = itm is null ? null : itm;
    }

    /// <summary>
    /// Retrieves and converts a value from TempData by key.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <param name="key">Key used to lookup the value.</param>
    /// <returns>The typed value when found and convertible; otherwise <see langword="default"/>.</returns>
    public static T? Get<T>(this ITempDataDictionary dic, string key) {
        ArgumentNullException.ThrowIfNull(dic);
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (!dic.TryGetValue(key, out object? value)) return default;

        if (value is T typedValue) return typedValue; // Type-safe return

        try {
            return (T?)Convert.ChangeType(value, typeof(T)); // Handles value types
        }
        catch {
            return default; // Avoids runtime exceptions
        }
    }

    /// <summary>
    /// Stores an error message in TempData.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <param name="msg">Error message text.</param>
    public static void SetError(this ITempDataDictionary dic, string msg) => dic["Error"] = msg ?? string.Empty;

    /// <summary>
    /// Retrieves the current error message from TempData.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <returns>Error message value, or an empty string when none is present.</returns>
    public static string GetError(this ITempDataDictionary dic) => dic["Error"] as string ?? string.Empty;

    /// <summary>
    /// Stores a general message in TempData.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <param name="msg">Message text.</param>
    public static void SetMSG(this ITempDataDictionary dic, string msg) => dic["MSG"] = msg ?? string.Empty;

    /// <summary>
    /// Retrieves the current general message from TempData.
    /// </summary>
    /// <param name="dic">TempData dictionary instance.</param>
    /// <returns>Message value, or an empty string when none is present.</returns>
    public static string GetMSG(this ITempDataDictionary dic) => dic["MSG"] as string ?? string.Empty;
}
