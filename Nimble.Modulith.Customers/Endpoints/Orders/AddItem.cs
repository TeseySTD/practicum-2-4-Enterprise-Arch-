using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Infrastructure;
using Nimble.Modulith.Customers.UseCases.Customers.Queries;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;
using Nimble.Modulith.Customers.UseCases.Orders.Queries;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class AddItem(IMediator mediator, ICustomerAuthorizationService auth)
    : Endpoint<AddOrderItemRequest, OrderResponse>
{
    public override void Configure()
    {
        Post("/orders/{id}/items");
        Tags("orders");
    }

    public override async Task HandleAsync(AddOrderItemRequest req, CancellationToken ct)
    {
        var orderId = Route<int>("id");
        var orderResult = await mediator.Send(new GetOrderByIdQuery(orderId), ct);
        if (!orderResult.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var custResult = await mediator.Send(new GetCustomerByIdQuery(orderResult.Value.CustomerId), ct);
        if (!custResult.IsSuccess || !auth.IsAdminOrOwner(User, custResult.Value.Email))
        {
            AddError("Access Denied");
            await Send.ForbiddenAsync(ct);
            return;
        }

        var command = new AddOrderItemCommand(orderId, req.ProductId, req.Quantity);
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        Response = new OrderResponse(result.Value.Id, result.Value.CustomerId, result.Value.OrderNumber,
            result.Value.OrderDate, result.Value.Status, result.Value.TotalAmount,
            result.Value.Items.Select(i =>
                    new OrderItemResponse(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
                .ToList());
    }
}