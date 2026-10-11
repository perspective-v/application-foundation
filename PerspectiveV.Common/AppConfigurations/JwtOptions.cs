namespace PerspectiveV.Common.AppConfigurations;
/// <summary>
/// Represents configuration options for Jwt Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of JwtOptions.
/// var type = typeof(JwtOptions);
/// ]]></code>
/// </example>
public partial class JwtOptions {
    public string? Audience { get; set; }
    public string[]? Audiences { get; set; }
    public string? Issuer { get; set; }
    public string? Secret { get; set; }
}
