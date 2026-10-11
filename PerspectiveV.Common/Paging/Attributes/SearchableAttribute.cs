using PerspectiveV.Common.Paging.Providers;

namespace PerspectiveV.Common.Paging.Attributes;
/// <summary>
/// Represents the SearchableAttribute type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchableAttribute.
/// var type = typeof(SearchableAttribute);
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchableAttribute : Attribute {
    public ISearchExpressionProvider ExpressionProvider { get; set; } = new DefaultSearchExpressionProvider();
}
