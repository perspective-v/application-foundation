using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PerspectiveV.Common.AppConfigurations;
using PerspectiveV.Web.S3Storage.Interface;
using PerspectiveV.Web.S3Storage.Models;

namespace PerspectiveV.Web.S3Storage.Implementation;
public class S3StorageService(IAmazonS3 s3Client, IOptions<S3BucketOptions> options) : IS3StorageService
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly S3BucketOptions _config = options.Value;

    // =====================================================
    // BASIC OBJECT OPERATIONS
    // =====================================================

    // 🔥 ORIGINAL (no progress)
    public Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        => UploadInternalAsync(file, folderName, null, cancellationToken);

    // 🔥 NEW (with progress tracking)
    public Task<string> UploadFileAsync(IFormFile file, string folderName, IUploadProgress? progress = null, CancellationToken cancellationToken = default)
        => UploadInternalAsync(file, folderName, progress, cancellationToken);

    private async Task<string> UploadInternalAsync(
        IFormFile file,
        string folderName,
        IUploadProgress? progress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (string.IsNullOrWhiteSpace(folderName))
            throw new ArgumentException("Folder required");

        var key = $"{folderName.TrimEnd('/')}/{Guid.NewGuid()}_{file.FileName}";

        await using var fileStream = file.OpenReadStream();

        Stream inputStream = fileStream;

        // 🔥 Wrap with progress stream if needed
        if (progress != null)
        {
            inputStream = new ProgressStream(fileStream, progress, file.Length);
        }

        var request = new PutObjectRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            InputStream = inputStream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return key;
    }

    // =====================================================
    // 🔥 TRUE STREAMING DOWNLOAD (NO BUFFERING)
    // =====================================================

    public async Task<Stream> OpenReadStreamAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key)
        }, cancellationToken);

        // IMPORTANT: This is a live network stream (no buffering)
        return response.ResponseStream;
    }

    // =====================================================
    // DELETE
    // =====================================================

    public async Task DeleteFileAsync(string key, CancellationToken cancellationToken = default)
    {
        await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key)
        }, cancellationToken);
    }

    // =====================================================
    // EXISTS
    // =====================================================

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = GetBucket(),
                Key = Normalize(key)
            }, cancellationToken);

            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    // =====================================================
    // COPY
    // =====================================================

    public async Task CopyFileAsync(string sourceKey, string destinationKey, CancellationToken cancellationToken = default)
    {
        await _s3Client.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = GetBucket(),
            SourceKey = Normalize(sourceKey),
            DestinationBucket = GetBucket(),
            DestinationKey = Normalize(destinationKey)
        }, cancellationToken);
    }

    // =====================================================
    // METADATA
    // =====================================================

    public async Task<string> GetContentTypeAsync(string key, CancellationToken cancellationToken = default)
    {
        var meta = await _s3Client.GetObjectMetadataAsync(GetBucket(), Normalize(key), cancellationToken);
        return meta.Headers.ContentType;
    }

    public async Task<long> GetFileSizeAsync(string key, CancellationToken cancellationToken = default)
    {
        var meta = await _s3Client.GetObjectMetadataAsync(GetBucket(), Normalize(key), cancellationToken);
        return meta.ContentLength;
    }

    public async Task<IDictionary<string, string>> GetMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key)
        }, cancellationToken);

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (response.Metadata != null)
        {
            foreach (var k in response.Metadata.Keys)
            {
                result[k] = response.Metadata[k];
            }
        }

        return result;
    }

    // =====================================================
    // URL ACCESS
    // =====================================================

    public Task<string> GetPreSignedUrlAsync(string key, int expiresInMinutes = 30, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            Verb = HttpVerb.GET
        };

        return Task.FromResult(_s3Client.GetPreSignedURL(request));
    }

    public Task<string> GetPreSignedUploadUrlAsync(string key, int expiresInMinutes = 30, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            Verb = HttpVerb.PUT
        };

        return Task.FromResult(_s3Client.GetPreSignedURL(request));
    }

    public string GetPublicUrl(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_config.ServiceHost))
            throw new InvalidOperationException("ServiceHost not configured.");

        return $"{_config.ServiceHost.TrimEnd('/')}/{GetBucket()}/{Normalize(key)}";
    }

    // =====================================================
    // LISTING
    // =====================================================

    public async Task<IEnumerable<string>> ListFilesAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var result = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = GetBucket(),
            Prefix = prefix
        }, cancellationToken);

        return result.S3Objects.Select(x => x.Key);
    }

    public async Task<IEnumerable<S3ObjectInfo>> ListFilesDetailedAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var result = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = GetBucket(),
            Prefix = prefix
        }, cancellationToken);

        return result.S3Objects.Select(x => new S3ObjectInfo
        {
            Key = x.Key,
            Size = x.Size,
            LastModified = x.LastModified
        });
    }

    // =====================================================
    // MULTIPART UPLOAD
    // =====================================================

    public async Task<string> InitiateMultipartUploadAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key)
        }, cancellationToken);

        return response.UploadId;
    }

    public async Task UploadPartAsync(string key, string uploadId, int partNumber, Stream partStream, CancellationToken cancellationToken = default)
    {
        await _s3Client.UploadPartAsync(new UploadPartRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            UploadId = uploadId,
            PartNumber = partNumber,
            InputStream = partStream
        }, cancellationToken);
    }

    public async Task CompleteMultipartUploadAsync(string key, string uploadId, IEnumerable<string> partETags, CancellationToken cancellationToken = default)
    {
        var request = new CompleteMultipartUploadRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            UploadId = uploadId,
            PartETags = [.. partETags.Select((etag, i) =>
                new PartETag(i + 1, etag))]
        };

        await _s3Client.CompleteMultipartUploadAsync(request, cancellationToken);
    }

    public async Task AbortMultipartUploadAsync(string key, string uploadId, CancellationToken cancellationToken = default)
    {
        await _s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
        {
            BucketName = GetBucket(),
            Key = Normalize(key),
            UploadId = uploadId
        }, cancellationToken);
    }

    // =====================================================
    // BUCKET OPERATIONS
    // =====================================================

    public async Task CreateBucketIfNotExistsAsync(CancellationToken cancellationToken = default)
    {
        if (!await BucketExistsAsync(cancellationToken))
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = GetBucket()
            }, cancellationToken);
        }
    }

    public async Task<bool> BucketExistsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.ListBucketsAsync(cancellationToken);
        return response.Buckets.Any(b => b.BucketName == GetBucket());
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private string GetBucket()
    {
        return _config.Bucket
            ?? throw new InvalidOperationException("Bucket not configured.");
    }

    private static string Normalize(string key) => key.TrimStart('/');
}