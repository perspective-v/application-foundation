namespace PerspectiveV.Primitives.Types.Models; 
/// <summary>
/// Represents the FileModel type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of FileModel.
/// var type = typeof(FileModel);
/// ]]></code>
/// </example>
public class FileModel {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Data { get; set; }
    public string? Extension { get; set; }
}
