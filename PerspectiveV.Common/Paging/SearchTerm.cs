
using PerspectiveV.Common.Paging.Providers;

namespace PerspectiveV.Common.Paging;
/// <summary>
/// Represents the SearchTerm type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SearchTerm.
/// var type = typeof(SearchTerm);
/// ]]></code>
/// </example>
public class SearchTerm {
    public string? Name { get; set; }

    public string? Operator { get; set; }

    public string? Value { get; set; }

    public bool ValidSyntax { get; set; }

    public ISearchExpressionProvider ExpressionProvider { get; set; } = null!;
}
