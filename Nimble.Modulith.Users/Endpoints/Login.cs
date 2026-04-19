using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Nimble.Modulith.Users.Endpoints;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Token { get; set; }
}

public class Login(SignInManager<IdentityUser> signInManager) : Endpoint<LoginRequest, LoginResponse>
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Login with email and password";
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {       
        var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, false, false);
        
        if (result.Succeeded)
        {
            Response = new LoginResponse { Success = true, Message = "Login successful", Token = "TODO: Generate JWT token" };
            await Send.OkAsync(Response, ct);
        }
        else
        {
            await Send.UnauthorizedAsync(ct);
        }
    }
}
