namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents configuration options for Virtual Directory Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of VirtualDirectoryOptions.
/// var type = typeof(VirtualDirectoryOptions);
/// ]]></code>
/// </example>
public partial class VirtualDirectoryOptions {
    public bool IsEnabled { get; set; }
    public string? Name { get; set; }
}
