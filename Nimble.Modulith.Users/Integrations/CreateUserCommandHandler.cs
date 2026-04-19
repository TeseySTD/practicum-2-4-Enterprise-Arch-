using Ardalis.Result;
using Mediator;
using Nimble.Modulith.Users.UseCases.Commands;

namespace Nimble.Modulith.Users.Integrations;

public class CreateUserCommandHandler(IMediator mediator) : ICommandHandler<CreateUserCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(CreateUserCommand command, CancellationToken ct) =>
        await mediator.Send(new CreateUserInternalCommand(command.Email, command.Password), ct);
}