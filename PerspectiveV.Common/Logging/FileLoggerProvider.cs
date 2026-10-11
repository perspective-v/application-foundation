using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PerspectiveV.Common.Logging;
/// <summary>
/// Represents the FileLoggerProvider type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of FileLoggerProvider.
/// var type = typeof(FileLoggerProvider);
/// ]]></code>
/// </example>
[ProviderAlias("FileLogger")]
public class FileLoggerProvider : ILoggerProvider {
    public readonly FileLoggerOptions Options;

    /// <summary>
    /// Initializes a new instance of the FileLoggerProvider class.
    /// </summary>
    /// <param name="_options">The _options value.</param>
    public FileLoggerProvider(IOptions<FileLoggerOptions> _options) {
        Options = _options.Value;

        if (!string.IsNullOrEmpty(Options.FolderPath) && !Directory.Exists(Options.FolderPath)) {
            Directory.CreateDirectory(Options.FolderPath);
        }
    }

    /// <summary>
    /// Creates Create Logger.
    /// </summary>
    /// <param name="categoryName">The categoryName value.</param>
    /// <returns>The operation result.</returns>
    public ILogger CreateLogger(string categoryName) {
        return new FileLogger(this);
    }

    /// <summary>
    /// Executes Dispose.
    /// </summary>
    public void Dispose() {
        GC.SuppressFinalize(this);
    }
}
