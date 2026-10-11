namespace PerspectiveV.Common.Paging.Attributes;

/// <summary>
/// Represents the SortableAttribute type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SortableAttribute.
/// var type = typeof(SortableAttribute);
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SortableAttribute : Attribute {
    public string[]? OrderBy { get; set; }
    public bool Default { get; set; }
}
