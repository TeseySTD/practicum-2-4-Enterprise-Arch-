using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.Endpoints;

public class LogoutResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class Logout : EndpointWithoutRequest<LogoutResponse>
{
    public override void Configure()
    {
        Post("/logout");
        AllowAnonymous();
        Summary(s => { s.Summary = "Logout the current user"; });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Response = new LogoutResponse
        {
            Success = true,
            Message = "Logged out successfully (Client must delete the token)"
        };

        await Send.OkAsync(Response, ct);
    }
}