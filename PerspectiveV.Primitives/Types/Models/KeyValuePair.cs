namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the KeyValuePair type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of KeyValuePair.
/// var type = typeof(KeyValuePair<>);
/// ]]></code>
/// </example>
public class KeyValuePair<T> {
    public string? Key { get; set; }
    public T? Value { get; set; }
}
