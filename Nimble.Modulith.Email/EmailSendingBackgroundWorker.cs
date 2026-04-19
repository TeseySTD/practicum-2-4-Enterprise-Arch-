using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Email.Interfaces;

namespace Nimble.Modulith.Email;

public class EmailSendingBackgroundWorker(
    IQueueService<Integrations.EmailToSend> queueService,
    IEmailSender emailSender,
    ILogger<EmailSendingBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Email worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var emailToSend = await queueService.DequeueAsync(stoppingToken);
                await emailSender.SendEmailAsync(
                    new EmailMessage(emailToSend.To, emailToSend.Subject, emailToSend.Body, emailToSend.From),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing email queue");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}