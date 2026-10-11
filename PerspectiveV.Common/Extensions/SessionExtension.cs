
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides static members in Session Extension.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by SessionExtension.
/// var typeName = nameof(SessionExtension);
/// ]]></code>
/// </example>
public static class SessionExtension {
    private static readonly JsonSerializerOptions SessionSerializationOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Sets Set Object As Json.
    /// </summary>
    /// <param name="session">The session value.</param>
    /// <param name="key">The key value.</param>
    /// <param name="value">The value value.</param>
    public static void SetObjectAsJson(this ISession session, string key, object value) {
        session.SetString(key, JsonSerializer.Serialize(value, SessionSerializationOptions));
    }

    /// <summary>
    /// Gets Get Object From Json.
    /// </summary>
    /// <param name="session">The session value.</param>
    /// <param name="key">The key value.</param>
    /// <returns>The operation result.</returns>
    public static TModel? GetObjectFromJson<TModel>(this ISession session, string key) {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<TModel>(value, SessionSerializationOptions);
    }
}
