using Ardalis.Result;
using Mediator;

namespace Nimble.Modulith.Users;

public record CreateUserCommand(string Email, string Password) : ICommand<Result<string>>;