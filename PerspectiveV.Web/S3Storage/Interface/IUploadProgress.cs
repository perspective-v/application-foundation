namespace PerspectiveV.Web.S3Storage.Interface;

public interface IUploadProgress
{
    /// <summary>
    /// Reports upload progress in bytes.
    /// </summary>
    /// <param name="bytesTransferred">Total bytes uploaded so far</param>
    /// <param name="totalBytes">Total file size (if known)</param>
    void Report(long bytesTransferred, long? totalBytes);
}
