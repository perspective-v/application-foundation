namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents the AppSettings type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of AppSettings.
/// var type = typeof(AppSettings);
/// ]]></code>
/// </example>
public partial class AppSettings {
    public string? AppName { get; set; }
    public string? GatewayUrl { get; set; }
    public string? AppCode { get; set; }
    public string? Secret { get; set; }
    public VirtualDirectoryOptions? VirtualDirectory { get; set; }
    public S3BucketOptions? S3Storage { get; set; }
    public CorsOptions? CORS { get; set; }
    public JwtOptions? Jwt { get; set; }
}
