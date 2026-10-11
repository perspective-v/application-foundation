namespace PerspectiveV.Common.Paging;

/// <summary>
/// Represents the PagedResult type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of PagedResult.
/// var type = typeof(PagedResult<>);
/// ]]></code>
/// </example>
public class PagedResult<T> {
    public IEnumerable<T>? Items { get; set; }
    public long ItemsCount { get; set; } = 0;
    public long TotalItemsCount { get; set; } = 0;
    public int? Offset { get; set; } = 0;
    public int? Limit { get; set; } = 0;
}
