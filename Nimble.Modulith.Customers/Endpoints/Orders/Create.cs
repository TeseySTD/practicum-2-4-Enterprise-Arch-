using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Infrastructure;
using Nimble.Modulith.Customers.UseCases.Customers.Queries;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class Create(IMediator mediator, ICustomerAuthorizationService auth)
    : Endpoint<CreateOrderRequest, OrderResponse>
{
    public override void Configure()
    {
        Post("/orders");
        Tags("orders");
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var custResult = await mediator.Send(new GetCustomerByIdQuery(req.CustomerId), ct);
        if (!custResult.IsSuccess || !auth.IsAdminOrOwner(User, custResult.Value.Email))
        {
            AddError("Access Denied");
            await Send.ForbiddenAsync(ct);
            return;
        }

        var command = new CreateOrderCommand(req.CustomerId, req.OrderDate,
            req.Items.Select(i => new CreateOrderItemDto(i.ProductId, i.Quantity))
                .ToList());
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.CreatedAtAsync<GetById>(new { id = result.Value.Id },
            new OrderResponse(result.Value.Id, result.Value.CustomerId, result.Value.OrderNumber,
                result.Value.OrderDate, result.Value.Status, result.Value.TotalAmount,
                result.Value.Items.Select(i =>
                        new OrderItemResponse(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
                    .ToList()), generateAbsoluteUrl: false, cancellation: ct);
    }
}