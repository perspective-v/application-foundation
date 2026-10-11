using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace PerspectiveV.Web.Extensions;

/// <summary>
/// Provides extension methods for List Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ListExtensions.
/// var typeName = nameof(ListExtensions);
/// ]]></code>
/// </example>
public static class ListExtensions {
    /// <summary>
    /// Gets Get Item Type.
    /// </summary>
    /// <param name="enumerable">The enumerable value.</param>
    /// <returns>The operation result.</returns>
    public static Type GetItemType<T>(this IEnumerable<T> enumerable) {
        return typeof(T);
    }

    /// <summary>
    /// Executes To Select Item List.
    /// </summary>
    /// <param name="values">The values value.</param>
    /// <returns>The operation result.</returns>
    public static List<SelectListItem> ToSelectItemList(dynamic values) {
        List<SelectListItem> tempList = [];
        if (values != null) {
            tempList = new List<SelectListItem>();
            foreach (var v in values) {
                tempList.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.Id) });
            }
            tempList.TrimExcess();
        }
        return tempList;
    }

    /// <summary>
    /// Executes To Select List.
    /// </summary>
    /// <param name="values">The values value.</param>
    /// <param name="value">The value value.</param>
    /// <param name="displayText">The displayText value.</param>
    /// <returns>The operation result.</returns>
    public static List<SelectListItem> ToSelectList<T>(this ICollection<T> values, string? value = null, string? displayText = null) {
        if (values == null || values.Count == 0)
            return new List<SelectListItem>();

        var tempList = new List<SelectListItem>(values.Count);
        foreach (var v in values) {
            if (v == null) continue;

            // Define the property name predicates
            var valuePred = new Func<PropertyInfo, bool>(x =>
                x.Name.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                (string.IsNullOrEmpty(value) && (x.Name.Equals("id", StringComparison.OrdinalIgnoreCase) || x.Name.Equals("value", StringComparison.OrdinalIgnoreCase)))
            );

            var textPred = new Func<PropertyInfo, bool>(x =>
                x.Name.Equals(displayText, StringComparison.OrdinalIgnoreCase) ||
                (string.IsNullOrEmpty(displayText) && (x.Name.Equals("name", StringComparison.OrdinalIgnoreCase) || x.Name.Equals("description", StringComparison.OrdinalIgnoreCase)))
            );

            // Get the properties based on predicates
            var valueProp = v.GetType().GetProperties().FirstOrDefault(valuePred);
            var textProp = v.GetType().GetProperties().FirstOrDefault(textPred);

            // Fetch the values and handle nulls gracefully
            var id = valueProp?.GetValue(v)?.ToString() ?? string.Empty;
            var text = textProp?.GetValue(v)?.ToString() ?? id;

            tempList.Add(new SelectListItem { Value = id, Text = text });
        }

        return tempList;
    }

    // Convert List of Strings to comma separated string
    /// <summary>
    /// Executes To CSL.
    /// </summary>
    /// <param name="list">The list value.</param>
    /// <returns>The operation result.</returns>
    public static string ToCSL(this IEnumerable<string> list) {
        if (list == null || !list.Any()) return string.Empty;
        return string.Join(",", list);
    }
}
