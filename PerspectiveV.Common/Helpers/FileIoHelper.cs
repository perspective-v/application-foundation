using Microsoft.AspNetCore.Http;

namespace PerspectiveV.Common.Helpers;
/// <summary>
/// Represents the FileIOHelper type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of FileIOHelper.
/// var type = typeof(FileIOHelper);
/// ]]></code>
/// </example>
public class FileIOHelper {

    public static string GetFileNameWithTimestamp(IFormFile file) {
        var timestamp = DateTime.Now.Ticks;
        var extension = Path.GetExtension(file.FileName);
        return $"{timestamp}-{extension}";
    }

    /// <summary>
    /// Executes Save File Get Db Path.
    /// </summary>
    /// <param name="dynamicDirectoryPath">The dynamicDirectoryPath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <param name="file">The file value.</param>
    /// <returns>The operation result.</returns>
    public static string SaveFileGetDbPath(string dynamicDirectoryPath, string webRootPath, IFormFile file) {
        var uno = DateTime.Now.Ticks;
        // create a new file name
        var fileName = $"{uno}{file.FileName[file.FileName.LastIndexOf('.')..]}";
        // create path to store and retrieve in db
        var dbPath = $"{dynamicDirectoryPath}{fileName}";
        // create physical path to store file on file system
        var phyPath = $"{webRootPath}{dbPath.Replace('/', Path.DirectorySeparatorChar).Replace("~", "")}";
        // directory path 
        var dirPath = $"{webRootPath}{dynamicDirectoryPath.Replace('/', Path.DirectorySeparatorChar).Replace("~", "")}";

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        using (var stream = new FileStream(phyPath, FileMode.Create)) {
            file.CopyTo(stream);
        }
        return dbPath;
    }

    /// <summary>
    /// Saves an uploaded file to disk asynchronously and returns the stored db-relative path.
    /// </summary>
    /// <param name="dynamicDirectoryPath">Target virtual directory path.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="file">Uploaded file to store.</param>
    /// <param name="cancellationToken">Cancellation token for I/O operations.</param>
    /// <returns>Stored db-relative path.</returns>
    public static async Task<string> SaveFileGetDbPathAsync(string dynamicDirectoryPath, string webRootPath, IFormFile file, CancellationToken cancellationToken = default) {
        var uno = DateTime.Now.Ticks;
        var fileName = $"{uno}{file.FileName[file.FileName.LastIndexOf('.')..]}";
        var dbPath = $"{dynamicDirectoryPath}{fileName}";
        var phyPath = $"{webRootPath}{dbPath.Replace('/', Path.DirectorySeparatorChar).Replace("~", "")}";
        var dirPath = $"{webRootPath}{dynamicDirectoryPath.Replace('/', Path.DirectorySeparatorChar).Replace("~", "")}";

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        await using (var stream = new FileStream(phyPath, FileMode.Create)) {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return dbPath;
    }

    /// <summary>
    /// Deletes Delete Static File If Exists.
    /// </summary>
    /// <param name="dbPath">The dbPath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    public static void DeleteStaticFileIfExists(string dbPath, string webRootPath) {
        // make dbpath into physical path formating
        var ftd = $"{dbPath.Replace("~/", $"{Path.DirectorySeparatorChar}").Replace('/', Path.DirectorySeparatorChar)}";
        // get physical path
        var ptd = $"{webRootPath}{ftd}";

        FileInfo finfo = new(ptd);
        if (finfo.Exists) {
            File.Delete(finfo.FullName);
        }
    }

    /// <summary>
    /// Deletes a static file when it exists.
    /// </summary>
    /// <param name="dbPath">Db-relative file path.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public static Task DeleteStaticFileIfExistsAsync(string dbPath, string webRootPath, CancellationToken cancellationToken = default) {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteStaticFileIfExists(dbPath, webRootPath);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets Get Static File If Exists.
    /// </summary>
    /// <param name="dbPath">The dbPath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <returns>The operation result.</returns>
    public static FileInfo? GetStaticFileIfExists(string dbPath, string webRootPath) {
        // make dbpath into physical path formating
        var ftd = $"{dbPath.Replace("~/", $"{Path.DirectorySeparatorChar}").Replace('/', Path.DirectorySeparatorChar)}";
        // get physical path
        var ptd = $"{webRootPath}{ftd}";

        var finfo = new FileInfo(ptd);
        return finfo.Exists ? finfo : null;
    }

    /// <summary>
    /// Gets Get Static File Content If Exists.
    /// </summary>
    /// <param name="dbPath">The dbPath value.</param>
    /// <param name="webRootPath">The webRootPath value.</param>
    /// <returns>The operation result.</returns>
    public static string GetStaticFileContentIfExists(string dbPath, string webRootPath) {
        // make dbpath into physical path formating
        var ftd = $"{dbPath.Replace("~/", $"{Path.DirectorySeparatorChar}").Replace('/', Path.DirectorySeparatorChar)}";
        // get physical path
        var ptd = $"{webRootPath}{ftd}";

        FileInfo finfo = new(ptd);
        var res = "";
        if (finfo.Exists) {
            res = File.ReadAllText(finfo.FullName);
        }
        return res;
    }

    /// <summary>
    /// Reads static file content when it exists.
    /// </summary>
    /// <param name="dbPath">Db-relative file path.</param>
    /// <param name="webRootPath">Physical web root path.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>File content or an empty string when missing.</returns>
    public static async Task<string> GetStaticFileContentIfExistsAsync(string dbPath, string webRootPath, CancellationToken cancellationToken = default) {
        // make dbpath into physical path formating
        var ftd = $"{dbPath.Replace("~/", $"{Path.DirectorySeparatorChar}").Replace('/', Path.DirectorySeparatorChar)}";
        // get physical path
        var ptd = $"{webRootPath}{ftd}";

        FileInfo finfo = new(ptd);
        if (finfo.Exists) {
            return await File.ReadAllTextAsync(finfo.FullName, cancellationToken);
        }

        return "";
    }
}
