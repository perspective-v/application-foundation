namespace PerspectiveV.Common.Logging; 
/// <summary>
/// Represents configuration options for File Logger Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of FileLoggerOptions.
/// var type = typeof(FileLoggerOptions);
/// ]]></code>
/// </example>
public class FileLoggerOptions {
    public virtual string? FilePath { get; set; }

    public virtual string? FolderPath { get; set; }
}
