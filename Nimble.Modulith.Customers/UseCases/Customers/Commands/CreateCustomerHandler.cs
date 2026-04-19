using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Users;

namespace Nimble.Modulith.Customers.UseCases.Customers.Commands;

public class CreateCustomerHandler(
    IRepository<Customer> repository,
    IMediator mediator,
    UserManager<IdentityUser> userManager) : ICommandHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    public async ValueTask<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        string? tempPassword = null;

        if (existingUser == null)
        {
            tempPassword = Guid.NewGuid().ToString("N")[..12];
            var userResult = await mediator.Send(new CreateUserCommand(command.Email, tempPassword), ct);
            if (!userResult.IsSuccess) return Result<CustomerDto>.Error("Failed to create user: " + userResult.Errors.First());
        }

        var customer = new Customer
        {
            FirstName = command.FirstName, LastName = command.LastName, Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            Address = new Address
            {
                Street = command.Street, City = command.City, State = command.State, PostalCode = command.PostalCode,
                Country = command.Country
            }
        };
        await repository.AddAsync(customer, ct);
        await repository.SaveChangesAsync(ct);

        var emailBody = tempPassword != null
            ? $"Welcome!\n\nEmail: {command.Email}\nTemporary Password: {tempPassword}\n\nPlease log in and change your password."
            : $"Welcome back!\n\nA customer profile has been created for {command.Email}. You can use your existing password.";

        await mediator.Send(
            new SendEmailCommand(command.Email, tempPassword != null ? "Welcome" : "Profile Created", emailBody), ct);

        return Result<CustomerDto>.Success(new CustomerDto(customer.Id, customer.FirstName, customer.LastName,
            customer.Email, customer.PhoneNumber,
            new AddressDto(customer.Address.Street, customer.Address.City, customer.Address.State,
                customer.Address.PostalCode, customer.Address.Country), customer.CreatedAt, customer.UpdatedAt));
    }
}