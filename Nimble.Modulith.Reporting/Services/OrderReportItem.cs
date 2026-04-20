namespace Nimble.Modulith.Reporting.Services;

public record OrderReportItem(int OrderId, string Email, DateTime OrderDate, int TotalQuantity, decimal TotalAmount);

public record ProductSalesReportItem(string ProductName, int TotalQuantity, decimal TotalRevenue, int OrderCount);