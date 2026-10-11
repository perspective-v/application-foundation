namespace PerspectiveV.Common.AppConfigurations.Emailing;
/// <summary>
/// Represents configuration options for Email Template Options.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of EmailTemplateOptions.
/// var type = typeof(EmailTemplateOptions);
/// ]]></code>
/// </example>
public partial class EmailTemplateOptions {
    /// <summary>
    /// Defines the base behavior for Constants.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// // Derive from Constants and implement required members.
    /// var type = typeof(Constants);
    /// ]]></code>
    /// </example>
    public abstract class Constants {
        public const string FORGOT_PASSWORD = "ForgotPassword";
        public const string RESET_PASSWORD = "ResetPassword";
        public const string EMAIL_VERIFICATION = "EmailVerification";
    }

    public EmailTemplate[]? EmailTemplates { get; set; }
    /// <summary>
    /// Gets Get Template.
    /// </summary>
    /// <param name="key">The key value.</param>
    /// <returns>The operation result.</returns>
    public EmailTemplate? GetTemplate(string key) {
        return EmailTemplates?.ToList()?.Find(x => x.Key!.Equals(key, StringComparison.Ordinal));
    }
}
