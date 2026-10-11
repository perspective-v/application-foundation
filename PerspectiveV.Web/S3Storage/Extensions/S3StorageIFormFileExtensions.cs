using Microsoft.AspNetCore.Http;
using PerspectiveV.Web.S3Storage.Interface;

namespace PerspectiveV.Web.S3Storage.Extensions;

public static class S3StorageIFormFileExtensions
{
    public static async Task<string?> SaveToS3StorageGetKey(this IFormFileCollection files, IS3StorageService s3, string storageFolder, CancellationToken cancellationToken = default)
    {
        string? objectKey = null;
        if (files.Count > 0)
        {
            objectKey = await s3.UploadFileAsync(files[0], storageFolder, cancellationToken);
        }
        return objectKey;
    }

    public static async Task<List<string>?> SaveToS3StorageGetKeys(this IFormFileCollection files, IS3StorageService s3, string storageFolder, CancellationToken cancellationToken = default)
    {
        List<string>? objectKeys = null;
        if (files.Count > 0)
        {
            objectKeys = [];
            foreach (IFormFile file in files)
            {
                if (file != null)
                {
                    var key = await s3.UploadFileAsync(file, storageFolder, cancellationToken);
                    objectKeys.Add(key);
                }
            }
        }
        return objectKeys;
    }

    public static async Task DeleteFileFromS3Storage(this IFormFileCollection files, IS3StorageService s3, string objectKey, CancellationToken cancellationToken = default)
    {
        await s3.DeleteFileAsync(objectKey, cancellationToken);
    }
}
