namespace PerspectiveV.Common.Helpers;

/// <summary>
/// Provides static members in Object Helpers.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by ObjectHelpers.
/// var typeName = nameof(ObjectHelpers);
/// ]]></code>
/// </example>
public static class ObjectHelpers
{
    /// <summary>
    /// Sets Set Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <param name="value">The value value.</param>
    public static void SetPropertyValueIfExists<T>(T m, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property is not null)
        {
            typeof(T).GetProperty(propertyName)?.SetValue(m, value);
        }
    }

    /// <summary>
    /// Gets Get Property Value If Exists.
    /// </summary>
    /// <param name="m">The m value.</param>
    /// <param name="propertyName">The propertyName value.</param>
    /// <returns>The operation result.</returns>
    public static TResult? GetPropertyValueIfExists<T, TResult>(T m, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property is not null)
        {
            var value = property.GetValue(m);
            return value is TResult castedValue ? castedValue : default;
        }
        return default;
    }
}
