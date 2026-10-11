namespace PerspectiveV.Common.AppConfigurations.Emailing;
/// <summary>
/// Represents the EmailTemplate type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of EmailTemplate.
/// var type = typeof(EmailTemplate);
/// ]]></code>
/// </example>
public partial class EmailTemplate {
    public string? Key { get; set; }
    public string? CallbackUrl { get; set; }
    public string? Template { get; set; }
    public string? TemplateFilePath { get; set; }
}
