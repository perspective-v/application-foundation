namespace PerspectiveV.Web.S3Storage.Models;

public class S3ObjectInfo {
    public string Key { get; set; } = string.Empty;
    public long? Size { get; set; }
    public DateTime? LastModified { get; set; }
    public string? ContentType { get; set; }
}