namespace Nimble.Modulith.Email.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessage message, CancellationToken ct = default);
}