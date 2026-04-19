using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Infrastructure;
using Nimble.Modulith.Customers.UseCases.Customers.Queries;
using Nimble.Modulith.Customers.UseCases.Orders.Queries;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class GetById(IMediator mediator, ICustomerAuthorizationService auth) : EndpointWithoutRequest<OrderResponse>
{
    public override void Configure()
    {
        Get("/orders/{id}");
        Tags("orders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var orderResult = await mediator.Send(new GetOrderByIdQuery(id), ct);
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

        Response = new OrderResponse(orderResult.Value.Id, orderResult.Value.CustomerId, orderResult.Value.OrderNumber,
            orderResult.Value.OrderDate, orderResult.Value.Status, orderResult.Value.TotalAmount,
            orderResult.Value.Items.Select(i =>
                    new OrderItemResponse(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
                .ToList());
    }
}