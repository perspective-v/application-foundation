namespace PerspectiveV.Common.Paging;
/// <summary>
/// Represents the SortTerm type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SortTerm.
/// var type = typeof(SortTerm);
/// ]]></code>
/// </example>
public class SortTerm {
    public string? Name { get; set; }
    public bool Descending { get; set; }
    public bool Default { get; set; }
}
