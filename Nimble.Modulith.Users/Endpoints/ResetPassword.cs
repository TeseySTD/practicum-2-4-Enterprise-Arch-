using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Users.Infrastructure;

namespace Nimble.Modulith.Users.Endpoints;

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class ResetPassword(UserManager<IdentityUser> userManager, IMediator mediator)
    : Endpoint<ResetPasswordRequest, ResetPasswordResponse>
{
    public override void Configure()
    {
        Post("/users/reset-password");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        var successResponse = new ResetPasswordResponse
            { Success = true, Message = "If the email exists, a password reset email has been sent." };
        if (user == null)
        {
            Response = successResponse;
            return;
        }

        var newPassword = PasswordGenerator.GeneratePassword();
        await userManager.RemovePasswordAsync(user);
        var addResult = await userManager.AddPasswordAsync(user, newPassword);

        if (!addResult.Succeeded)
        {
            AddError("Failed to set new password");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var emailBody =
            $"Hello,\n\nYour password has been reset.\nYour new temporary password is: {newPassword}\n\nPlease log in and change it.\n\nBest regards,\nThe Team";
        await mediator.Send(new SendEmailCommand(user.Email!, "Password Reset", emailBody), ct);
        Response = successResponse;
    }
}