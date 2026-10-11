using PerspectiveV.Common.Paging.Providers;

namespace PerspectiveV.Common.Paging.Attributes;
/// <summary>
/// Represents the SearchableContainsAttribute type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchableContainsAttribute.
/// var type = typeof(SearchableContainsAttribute);
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchableContainsAttribute : SearchableAttribute {
    /// <summary>
    /// Initializes a new instance of the SearchableContainsAttribute class.
    /// </summary>
    public SearchableContainsAttribute() {
        ExpressionProvider = new ContainsSearchExpressionProvider();
    }
}
