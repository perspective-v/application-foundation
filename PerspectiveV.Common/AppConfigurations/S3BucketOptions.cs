namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents configuration options for Virtual Directory Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of S3BucketOptions.
/// var type = typeof(S3BucketOptions);
/// ]]></code>
/// </example>
public partial class S3BucketOptions
{
    public bool IsEnabled { get; set; }
    public string? ServiceHost { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
    public string? Bucket { get; set; }
}
