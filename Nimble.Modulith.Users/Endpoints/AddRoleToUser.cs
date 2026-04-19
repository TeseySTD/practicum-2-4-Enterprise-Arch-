using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Users.Events;

namespace Nimble.Modulith.Users.Endpoints;

public class AddRoleToUser(UserManager<IdentityUser> userManager, IMediator mediator)
    : Endpoint<AddRoleToUserRequest, AddRoleToUserResponse>
{
    public override void Configure()
    {
        Post("/users/{id}/roles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddRoleToUserRequest req, CancellationToken ct)
    {
        var userId = Route<string>("id")!;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            AddError("User not found");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var roleName = char.ToUpper(req.RoleName[0]) + req.RoleName.Substring(1).ToLower();
        if (roleName != "Admin" && roleName != "Customer")
        {
            AddError("Invalid role");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (await userManager.IsInRoleAsync(user, roleName))
        {
            AddError("Already in role");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors) AddError(err.Description);
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        await mediator.Publish(new UserAddedToRoleEvent(user.Id, user.Email!, roleName), ct);
        Response = new AddRoleToUserResponse { Message = "Success" };
    }
}