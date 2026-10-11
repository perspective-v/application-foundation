using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Common.Helpers;
using PerspectiveV.Primitives.Types.Models;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides extension methods for Web Host Environment Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by WebHostEnvironmentExtensions.
/// var typeName = nameof(WebHostEnvironmentExtensions);
/// ]]></code>
/// </example>
public static class WebHostEnvironmentExtensions {

    /// <summary>
    /// Gets Get Processed Content Path.
    /// </summary>
    /// <param name="env">The env value.</param>
    /// <param name="appSettingOptions">The appSettingOptions value.</param>
    /// <returns>The operation result.</returns>
    public static string GetProcessedContentPath(this IWebHostEnvironment env, IOptions<AppSettings> appSettingOptions) {
        var _appSettings = appSettingOptions?.Value;
        var ContentOrWebRootPath = env.ContentRootPath;

        if (_appSettings != null) {
            ContentOrWebRootPath = _appSettings.VirtualDirectory?.IsEnabled == true
                ? Path.Combine(env.ContentRootPath, $"..\\{_appSettings.VirtualDirectory?.Name}")
                : env.WebRootPath;
        }

        return ContentOrWebRootPath;
    }

    /// <summary>
    /// Executes Save File And Get Server Path.
    /// </summary>
    /// <param name="env">The env value.</param>
    /// <param name="contentPath">The contentPath value.</param>
    /// <param name="fileDTO">The fileDTO value.</param>
    /// <param name="appSettingOptions">The appSettingOptions value.</param>
    /// <returns>The operation result.</returns>
    public static string SaveFileAndGetServerPath(this IWebHostEnvironment env, string contentPath, FileModel fileDTO, IOptions<AppSettings> appSettingOptions) {
        byte[] bytes = Convert.FromBase64String(fileDTO?.Data!);
        MemoryStream stream = new MemoryStream(bytes);

        IFormFile file = new FormFile(stream, 0, bytes.Length, fileDTO?.Name!, fileDTO?.Name!);
        return file.SaveFileGetServerPath(contentPath, env.GetProcessedContentPath(appSettingOptions));
    }

    /// <summary>
    /// Deletes Delete Server File.
    /// </summary>
    /// <param name="env">The env value.</param>
    /// <param name="fileDynamicString">The fileDynamicString value.</param>
    /// <param name="appSettingOptions">The appSettingOptions value.</param>
    public static void DeleteServerFile(this IWebHostEnvironment env, string fileDynamicString, IOptions<AppSettings> appSettingOptions) {
        FileIOHelper.DeleteStaticFileIfExists(fileDynamicString, env.GetProcessedContentPath(appSettingOptions));
    }
}
