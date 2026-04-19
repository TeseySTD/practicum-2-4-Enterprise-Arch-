using Mediator;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Email.Interfaces;

namespace Nimble.Modulith.Email.Integrations;

public class SendEmailCommandHandler(IQueueService<EmailToSend> queueService) : ICommandHandler<SendEmailCommand>
{
    public async ValueTask<Unit> Handle(SendEmailCommand command, CancellationToken ct)
    {
        await queueService.EnqueueAsync(new EmailToSend(command.To, command.Subject, command.Body, command.From), ct);
        return Unit.Value;
    }
}