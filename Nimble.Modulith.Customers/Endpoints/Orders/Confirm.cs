using FastEndpoints;
using Mediator;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class Confirm(
    IMediator mediator,
    Infrastructure.ICustomerAuthorizationService authService)
    : EndpointWithoutRequest<OrderResponse>
{
    public override void Configure()
    {
        Post("/orders/{id}/confirm");
        Tags("orders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orderId = Route<int>("id");
        var orderResult =
            await mediator.Send(new Nimble.Modulith.Customers.UseCases.Orders.Queries.GetOrderByIdQuery(orderId), ct);
        if (!orderResult.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var custResult =
            await mediator.Send(
                new Nimble.Modulith.Customers.UseCases.Customers.Queries.GetCustomerByIdQuery(orderResult.Value
                    .CustomerId), ct);
        if (!custResult.IsSuccess || !authService.IsAdminOrOwner(User, custResult.Value.Email))
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        var result =
            await mediator.Send(new Nimble.Modulith.Customers.UseCases.Orders.Commands.ConfirmOrderCommand(orderId),
                ct);
        if (!result.IsSuccess)
        {
            AddError("Failed to confirm");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var itemsStr = string.Join("\n",
            result.Value.Items.Select(i =>
                $"- {i.ProductName} x {i.Quantity} @ ${i.UnitPrice:F2} = ${i.TotalPrice:F2}"));
        var emailBody =
            $"Your order is confirmed!\n\nOrder: {result.Value.OrderNumber}\nTotal: ${result.Value.TotalAmount:F2}\n\nItems:\n{itemsStr}";

        await mediator.Send(
            new Nimble.Modulith.Email.Contracts.SendEmailCommand(custResult.Value.Email,
                $"Order Confirmed - {result.Value.OrderNumber}", emailBody), ct);

        Response = new OrderResponse(result.Value.Id, result.Value.CustomerId, result.Value.OrderNumber,
            result.Value.OrderDate, result.Value.Status, result.Value.TotalAmount,
            result.Value.Items.Select(i =>
                    new OrderItemResponse(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
                .ToList());
    }
}