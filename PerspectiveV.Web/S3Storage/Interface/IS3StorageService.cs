using PerspectiveV.Web.S3Storage.Models;

namespace PerspectiveV.Web.S3Storage.Interface;

using Microsoft.AspNetCore.Http;

public interface IS3StorageService
{
    // =========================
    // BASIC OBJECT OPERATIONS
    // =========================

    Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);

    Task<string> UploadFileAsync(
        IFormFile file,
        string folderName,
        IUploadProgress? progress = null, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadStreamAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task CopyFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default);


    // =========================
    // METADATA
    // =========================

    Task<string> GetContentTypeAsync(string key, CancellationToken cancellationToken = default);

    Task<long> GetFileSizeAsync(string key, CancellationToken cancellationToken = default);

    Task<IDictionary<string, string>> GetMetadataAsync(string key, CancellationToken cancellationToken = default);


    // =========================
    // URL ACCESS
    // =========================

    Task<string> GetPreSignedUrlAsync(string key, int expiresInMinutes = 30, CancellationToken cancellationToken = default);

    Task<string> GetPreSignedUploadUrlAsync(string key, int expiresInMinutes = 30, CancellationToken cancellationToken = default);

    string GetPublicUrl(string key, CancellationToken cancellationToken = default);


    // =========================
    // LISTING / QUERY
    // =========================

    Task<IEnumerable<string>> ListFilesAsync(string prefix, CancellationToken cancellationToken = default);

    Task<IEnumerable<S3ObjectInfo>> ListFilesDetailedAsync(string prefix, CancellationToken cancellationToken = default);


    // =========================
    // MULTIPART UPLOAD (BIG FILES)
    // =========================

    Task<string> InitiateMultipartUploadAsync(string key, CancellationToken cancellationToken = default);

    Task UploadPartAsync(string key, string uploadId, int partNumber, Stream partStream, CancellationToken cancellationToken = default);

    Task CompleteMultipartUploadAsync(
        string key,
        string uploadId,
        IEnumerable<string> partETags, CancellationToken cancellationToken = default);

    Task AbortMultipartUploadAsync(string key, string uploadId, CancellationToken cancellationToken = default);


    // =========================
    // BUCKET OPERATIONS
    // =========================

    Task CreateBucketIfNotExistsAsync(CancellationToken cancellationToken = default);

    Task<bool> BucketExistsAsync(CancellationToken cancellationToken = default);
}
