using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nimble.Modulith.Email.Interfaces;
using MimeKit;

namespace Nimble.Modulith.Email;

public class SmtpEmailSender(ILogger<SmtpEmailSender> logger, IOptions<EmailSettings> settings) : IEmailSender
{
    public async Task SendEmailAsync(EmailMessage message, CancellationToken ct = default)
    {
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(message.From ?? settings.Value.DefaultFromAddress));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = message.Body };
            using var client = new SmtpClient();
            await client.ConnectAsync(settings.Value.SmtpServer, settings.Value.SmtpPort, settings.Value.EnableSsl, ct);
            if (!string.IsNullOrEmpty(settings.Value.Username))
                await client.AuthenticateAsync(settings.Value.Username, settings.Value.Password, ct);
            await client.SendAsync(mimeMessage, ct);
            await client.DisconnectAsync(false, ct);
            logger.LogInformation("Email sent successfully to {To}", message.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
    }
}