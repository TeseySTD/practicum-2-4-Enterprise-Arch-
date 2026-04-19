using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Products.Data;

namespace Nimble.Modulith.Products.Endpoints;

public class DeleteProductRequest { public int Id { get; set; } }

public class Delete(ProductsDbContext dbContext) : Endpoint<DeleteProductRequest>
{
    public override void Configure() { Delete("/products/{id}"); Tags("products"); Roles("Admin");}
    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null) { await Send.NotFoundAsync(ct); return; }
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
