namespace PerspectiveV.Common.AppConfigurations.Emailing;
/// <summary>
/// Represents the SMTPClientSettings type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SMTPClientSettings.
/// var type = typeof(SMTPClientSettings);
/// ]]></code>
/// </example>
public partial class SMTPClientSettings {
    public string? AppName { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderEmailPassword { get; set; }
    public string? SmtpClient { get; set; }
    public int SmtpClientPort { get; set; }
}
