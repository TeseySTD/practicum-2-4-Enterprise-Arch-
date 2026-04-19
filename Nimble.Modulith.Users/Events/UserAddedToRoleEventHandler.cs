using Mediator;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Email.Contracts;

namespace Nimble.Modulith.Users.Events;

public class UserAddedToRoleEventHandler(IMediator mediator, ILogger<UserAddedToRoleEventHandler> logger)
    : INotificationHandler<UserAddedToRoleEvent>
{
    public async ValueTask Handle(UserAddedToRoleEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending email notification for role {RoleName}", notification.RoleName);
        var emailCommand = new SendEmailCommand(notification.UserEmail,
            $"You've been added to the {notification.RoleName} role",
            $"Hello,\n\nYou have been added to the {notification.RoleName} role.\n\nThe Nimble Team");
        await mediator.Send(emailCommand, ct);
    }
}