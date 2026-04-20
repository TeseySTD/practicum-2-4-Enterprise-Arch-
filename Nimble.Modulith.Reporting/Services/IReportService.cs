namespace Nimble.Modulith.Reporting.Services;

public interface IReportService
{
    Task<IEnumerable<OrderReportItem>> GetOrdersReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ProductSalesReportItem>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate);
}