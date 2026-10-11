using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PerspectiveV.Common.Logging; 
/// <summary>
/// Provides extension methods for File Logger Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by FileLoggerExtensions.
/// var typeName = nameof(FileLoggerExtensions);
/// ]]></code>
/// </example>
public static class FileLoggerExtensions {
    /// <summary>
    /// Adds Add File Logger.
    /// </summary>
    /// <param name="builder">The builder value.</param>
    /// <param name="configure">The configure value.</param>
    /// <returns>The operation result.</returns>
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure) {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}
