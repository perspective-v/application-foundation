namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents configuration options for Cors Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of CorsOptions.
/// var type = typeof(CorsOptions);
/// ]]></code>
/// </example>
public partial class CorsOptions {
    public List<string>? AllowedOrigins { get; set; }
    public List<string>? AllowedMethods { get; set; }
    public List<string>? AllowedHeaders { get; set; }
}
