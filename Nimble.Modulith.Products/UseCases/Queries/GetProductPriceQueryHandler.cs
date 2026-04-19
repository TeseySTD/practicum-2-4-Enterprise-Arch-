using Mediator;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Products.Contracts;
using Nimble.Modulith.Products.Data;

namespace Nimble.Modulith.Products.UseCases.Queries;

public class GetProductPriceQueryHandler(ProductsDbContext dbContext) : IQueryHandler<GetProductPriceQuery, decimal>
{
    public async ValueTask<decimal> Handle(GetProductPriceQuery query, CancellationToken ct)
    {
        var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == query.ProductId, ct);
        return product?.Price ?? throw new InvalidOperationException($"Product {query.ProductId} not found");
    }
}