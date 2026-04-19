namespace Nimble.Modulith.Customers.Contracts;

public record OrderDetails(
    int Id,
    int CustomerId,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<OrderItemDetails> Items
);

public record OrderItemDetails(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);