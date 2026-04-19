namespace Nimble.Modulith.Email.Contracts;

using Mediator;

public record SendEmailCommand(string To, string Subject, string Body, string? From = null) : ICommand;