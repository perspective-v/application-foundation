using PerspectiveV.Common.AppConfigurations.Emailing;
using System.Net;
using System.Net.Mail;

namespace PerspectiveV.Common.Mailing;
/// <summary>
/// Represents the SMTPMailClient type.
/// </summary>
/// <example>
/// <code><![CDATA[
/// // Create and use an instance of SMTPMailClient.
/// var type = typeof(SMTPMailClient);
/// ]]></code>
/// </example>
public class SMTPMailClient {

    private readonly SMTPClientSettings _clientSettings;
    /// <summary>
    /// Initializes a new instance of the SMTPMailClient class.
    /// </summary>
    /// <param name="clientSettings">The clientSettings value.</param>
    public SMTPMailClient(SMTPClientSettings clientSettings) {
        _clientSettings = clientSettings;
    }
    /// <summary>
    /// Executes Send Email.
    /// </summary>
    /// <param name="body">The body value.</param>
    /// <param name="subject">The subject value.</param>
    /// <param name="to">The to value.</param>
    /// <returns>The operation result.</returns>
    public bool SendEmail(string body, string subject, params string[] to) {
        ArgumentNullException.ThrowIfNullOrEmpty(_clientSettings.SenderEmail, "Sender email not configured");

        SmtpClient smtpClient = new(_clientSettings.SmtpClient, _clientSettings.SmtpClientPort) {
            Credentials = new NetworkCredential(_clientSettings.SenderEmail, _clientSettings.SenderEmailPassword),
            EnableSsl = false,
        };

        MailMessage message = new() {
            From = new MailAddress(_clientSettings.SenderEmail, _clientSettings?.AppName),
            IsBodyHtml = true,
            Subject = subject,
            Body = body
        };

        to.ToList().ForEach(recipient => {
            message.To.Add(recipient);
        });

        try {
            smtpClient.Send(message);
            return true;
        }
        catch (Exception) {
            return false;
            throw;
        }
    }
}
