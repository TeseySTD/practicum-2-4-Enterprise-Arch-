using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Infrastructure;
using Nimble.Modulith.Customers.UseCases.Customers.Queries;

namespace Nimble.Modulith.Customers.Endpoints.Customers;

public class GetById(IMediator mediator, ICustomerAuthorizationService auth) : EndpointWithoutRequest<CustomerResponse>
{
    public override void Configure()
    {
        Get("/customers/{id}");
        Tags("customers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var query = new GetCustomerByIdQuery(id);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!auth.IsAdminOrOwner(User, result.Value.Email))
        {
            AddError("Access Denied");
            await Send.ForbiddenAsync(ct);
            return;
        }

        Response = new CustomerResponse(result.Value.Id, result.Value.FirstName, result.Value.LastName,
            result.Value.Email, result.Value.PhoneNumber,
            new AddressResponse(result.Value.Address.Street, result.Value.Address.City, result.Value.Address.State,
                result.Value.Address.PostalCode, result.Value.Address.Country));
    }
}