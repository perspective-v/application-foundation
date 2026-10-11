namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents configuration options for Service Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of ServiceOptions.
/// var type = typeof(ServiceOptions);
/// ]]></code>
/// </example>
public partial class ServiceOptions {
    public string GatewayUrl { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public JwtOptions? Jwt { get; set; }
}
