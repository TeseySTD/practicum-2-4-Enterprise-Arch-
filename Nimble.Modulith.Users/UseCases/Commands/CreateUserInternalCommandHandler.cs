using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.UseCases.Commands;

public class CreateUserInternalCommandHandler(UserManager<IdentityUser> userManager)
    : ICommandHandler<CreateUserInternalCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(CreateUserInternalCommand command, CancellationToken ct)
    {
        var user = new IdentityUser { UserName = command.Email, Email = command.Email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
            return Result<string>.Error($"Failed: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        return Result<string>.Success(user.Id);
    }
}