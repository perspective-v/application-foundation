using System.ComponentModel.DataAnnotations;

namespace PerspectiveV.Common.Paging;
/// <summary>
/// Represents configuration options for Paging Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of PagingOptions.
/// var type = typeof(PagingOptions);
/// ]]></code>
/// </example>
public class PagingOptions {
    [Range(0, 9999, ErrorMessage = "Offset must be greater than zero")]
    public int? Offset { get; set; }

    [Range(1, 100, ErrorMessage = "Limit must be greater than 0 and less than 100")]
    public int? Limit { get; set; }
}
