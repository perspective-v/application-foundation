using PerspectiveV.Common.Paging.Providers;

namespace PerspectiveV.Common.Paging.Attributes;
/// <summary>
/// Represents the SearchableDecimalAttribute type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchableDecimalAttribute.
/// var type = typeof(SearchableDecimalAttribute);
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchableDecimalAttribute : SearchableAttribute {
    /// <summary>
    /// Initializes a new instance of the SearchableDecimalAttribute class.
    /// </summary>
    public SearchableDecimalAttribute() {
        ExpressionProvider = new DecimalToIntSearchExpressionProvider();
    }
}
