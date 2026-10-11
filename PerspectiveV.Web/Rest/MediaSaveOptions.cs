namespace PerspectiveV.Web.Rest;
/// <summary>
/// Represents configuration options for Media Save Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of MediaSaveOptions.
/// var type = typeof(MediaSaveOptions);
/// ]]></code>
/// </example>
public class MediaSaveOptions {
    public string FolderPath { get; set; } = null!;
    /// <summary>
    /// Model Property Name to which returned values should be bound
    /// </summary>
    public string PropertyName { get; set; } = null!;

    public bool HasImageCollection { get; set; } = false;
}
