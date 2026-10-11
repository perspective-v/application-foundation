using Microsoft.AspNetCore.Http;

using PerspectiveV.Common.Helpers;

namespace PerspectiveV.Common.Extensions;
/// <summary>
/// Provides extension methods for Form File Extensions.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Use static members exposed by FormFileExtensions.
/// var typeName = nameof(FormFileExtensions);
/// ]]></code>
/// </example>
public static class FormFileExtensions
{
    /// <summary>
    /// Executes Save File Get Server Path.
    /// </summary>
    /// <param name="file">The file value.</param>
    /// <param name="serverFolder">The serverFolder value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <returns>The operation result.</returns>
    public static string SaveFileGetServerPath(this IFormFile file, string serverFolder, string webRootPath)
    {
        string url = "";
        if (file != null)
        {
            url = FileIOHelper.SaveFileGetDbPath(serverFolder, webRootPath, file);
        }
        return url;
    }

    /// <summary>
    /// Saves a single file asynchronously and returns its stored server path.
    /// </summary>
    /// <param name="file">Uploaded file.</param>
    /// <param name="serverFolder">Target server folder.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stored server path.</returns>
    public static async Task<string> SaveFileGetServerPathAsync(this IFormFile file, string serverFolder, string webRootPath, CancellationToken cancellationToken = default)
    {
        string url = "";
        if (file != null)
        {
            url = await FileIOHelper.SaveFileGetDbPathAsync(serverFolder, webRootPath, file, cancellationToken);
        }
        return url;
    }

    /// <summary>
    /// Executes Save File Get Server Path.
    /// </summary>
    /// <param name="files">The files value.</param>
    /// <param name="serverFolder">The serverFolder value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <returns>The operation result.</returns>
    public static string SaveFileGetServerPath(this IFormFileCollection files, string serverFolder, string webRootPath)
    {
        string url = "";
        if (files.Count > 0)
        {
            url = SaveFileGetServerPath(files[0], serverFolder, webRootPath);
        }
        return url;
    }

    /// <summary>
    /// Saves the first file in a collection asynchronously and returns its stored server path.
    /// </summary>
    /// <param name="files">Uploaded file collection.</param>
    /// <param name="serverFolder">Target server folder.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stored server path.</returns>
    public static async Task<string> SaveFileGetServerPathAsync(this IFormFileCollection files, string serverFolder, string webRootPath, CancellationToken cancellationToken = default)
    {
        string url = "";
        if (files.Count > 0)
        {
            url = await SaveFileGetServerPathAsync(files[0], serverFolder, webRootPath, cancellationToken);
        }
        return url;
    }

    /// <summary>
    /// Executes Save Files Get Server Paths.
    /// </summary>
    /// <param name="files">The files value.</param>
    /// <param name="serverFolder">The serverFolder value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <returns>The operation result.</returns>
    public static List<string>? SaveFilesGetServerPaths(this IFormFileCollection files, string serverFolder, string webRootPath)
    {
        List<string>? urls = null;
        if (files.Count > 0)
        {
            urls = [];
            foreach (IFormFile file in files)
            {
                if (file != null)
                {
                    urls.Add(FileIOHelper.SaveFileGetDbPath(serverFolder, webRootPath, file));
                }
            }
        }
        return urls;
    }

    /// <summary>
    /// Saves all files in a collection asynchronously and returns their stored server paths.
    /// </summary>
    /// <param name="files">Uploaded file collection.</param>
    /// <param name="serverFolder">Target server folder.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stored server paths.</returns>
    public static async Task<List<string>?> SaveFilesGetServerPathsAsync(this IFormFileCollection files, string serverFolder, string webRootPath, CancellationToken cancellationToken = default)
    {
        List<string>? urls = null;
        if (files.Count > 0)
        {
            urls = [];
            foreach (IFormFile file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (file != null)
                {
                    urls.Add(await FileIOHelper.SaveFileGetDbPathAsync(serverFolder, webRootPath, file, cancellationToken));
                }
            }
        }
        return urls;
    }

    /// <summary>
    /// Deletes Delete File From Server.
    /// </summary>
    /// <param name="files">The files value.</param>
    /// <param name="filePath">The filePath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    public static void DeleteFileFromServer(this IFormFileCollection files, string filePath, string webRootPath)
    {
        FileIOHelper.DeleteStaticFileIfExists($"~/{filePath}", webRootPath);
    }

    /// <summary>
    /// Deletes a file asynchronously from server storage.
    /// </summary>
    /// <param name="files">Uploaded file collection context.</param>
    /// <param name="filePath">Db-relative file path.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task DeleteFileFromServerAsync(this IFormFileCollection files, string filePath, string webRootPath, CancellationToken cancellationToken = default)
    {
        await FileIOHelper.DeleteStaticFileIfExistsAsync($"~/{filePath}", webRootPath, cancellationToken);
    }

    /// <summary>
    /// Deletes Delete Files From Server.
    /// </summary>
    /// <param name="files">The files value.</param>
    /// <param name="filePaths">The filePaths value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    public static void DeleteFilesFromServer(this IFormFileCollection files, List<string> filePaths, string webRootPath)
    {
        foreach (var path in filePaths)
        {
            FileIOHelper.DeleteStaticFileIfExists($"~/{path}", webRootPath);
        }
    }

    /// <summary>
    /// Deletes multiple files asynchronously from server storage.
    /// </summary>
    /// <param name="files">Uploaded file collection context.</param>
    /// <param name="filePaths">Db-relative file paths.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task DeleteFilesFromServerAsync(this IFormFileCollection files, List<string> filePaths, string webRootPath, CancellationToken cancellationToken = default)
    {
        foreach (var path in filePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await FileIOHelper.DeleteStaticFileIfExistsAsync($"~/{path}", webRootPath, cancellationToken);
        }
    }
    /// <summary>
    /// Deletes Delete File From Server.
    /// </summary>
    /// <param name="file">The file value.</param>
    /// <param name="filePath">The filePath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    public static void DeleteFileFromServer(this IFormFile file, string filePath, string webRootPath)
    {
        FileIOHelper.DeleteStaticFileIfExists($"~/{filePath}", webRootPath);
    }

    /// <summary>
    /// Deletes a file asynchronously from server storage.
    /// </summary>
    /// <param name="file">Uploaded file context.</param>
    /// <param name="filePath">Db-relative file path.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task DeleteFileFromServerAsync(this IFormFile file, string filePath, string webRootPath, CancellationToken cancellationToken = default)
    {
        await FileIOHelper.DeleteStaticFileIfExistsAsync($"~/{filePath}", webRootPath, cancellationToken);
    }
}
