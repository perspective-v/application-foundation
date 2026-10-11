namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the NameValuePair type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of NameValuePair.
/// var type = typeof(NameValuePair<>);
/// ]]></code>
/// </example>
public class NameValuePair<T> {
    public string? Name { get; set; }
    public T? Value { get; set; }
}
