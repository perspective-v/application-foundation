using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace PerspectiveV.Common.Logging;

/// <summary>
/// Represents the FileLogger type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of FileLogger.
/// var type = typeof(FileLogger);
/// ]]></code>
/// </example>
public class FileLogger : ILogger, IDisposable {
    protected readonly FileLoggerProvider _loggerFileProvider;

    /// <summary>
    /// Initializes a new instance of the FileLogger class.
    /// </summary>
    /// <param name="loggerFileProvider">The loggerFileProvider value.</param>
    public FileLogger([NotNull] FileLoggerProvider loggerFileProvider) {
        _loggerFileProvider = loggerFileProvider ?? throw new ArgumentNullException(nameof(loggerFileProvider));
    }

    /// <summary>
    /// Represents the Disposable type.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Create and use an instance of Disposable.
    /// var type = typeof(Disposable);
    /// ]]></code>
    /// </example>
    private class Disposable : IDisposable {
        public static readonly IDisposable Empty = new Disposable();
        /// <summary>
        /// Executes Dispose.
        /// </summary>
        public void Dispose() { }
    }

    IDisposable ILogger.BeginScope<TState>(TState state) {
        return Disposable.Empty;
    }

    /// <summary>
    /// Executes Dispose.
    /// </summary>
    public void Dispose() {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Determines whether Is Enabled.
    /// </summary>
    /// <param name="logLevel">The logLevel value.</param>
    /// <returns>The operation result.</returns>
    public bool IsEnabled(LogLevel logLevel) {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Executes Log.
    /// </summary>
    /// <param name="logLevel">The logLevel value.</param>
    /// <param name="eventId">The eventId value.</param>
    /// <param name="state">The state value.</param>
    /// <param name="exception">The exception value.</param>
    /// <param name="formatter">The formatter value.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel) || _loggerFileProvider?.Options == null || formatter == null) {
            return;
        }

        // Ensure that FolderPath and FilePath are not null
        var folderPath = _loggerFileProvider.Options.FolderPath ?? "logs";
        var filePath = _loggerFileProvider.Options.FilePath ?? "log-{date}.txt";

        // Replace {date} placeholder safely
        var fullFilePath = Path.Combine(folderPath, filePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd")));

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath) ?? "logs");

        // Create log record
        var logRecord = string.Format("{0} [{1}] {2} {3}",
            "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]",
            logLevel.ToString(),
            formatter(state, exception),
            exception?.StackTrace ?? "");

        try {
            using var streamWriter = new StreamWriter(fullFilePath, true);
            streamWriter.WriteLine(logRecord);
        }
        catch (Exception ex) {
            Console.Error.WriteLine($"Logging failed: {ex.Message}");
        }
    }
}
