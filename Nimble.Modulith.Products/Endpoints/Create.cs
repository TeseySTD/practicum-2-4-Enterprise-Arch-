using System.Security.Claims;
using FastEndpoints;
using Nimble.Modulith.Products.Data;
using Nimble.Modulith.Products.Models;

namespace Nimble.Modulith.Products.Endpoints;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime DateCreated { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
}

public class Create(ProductsDbContext dbContext) : Endpoint<CreateProductRequest, CreateProductResponse>
{
    public override void Configure()
    {
        Post("/products");
        Tags("products");
        Roles("Admin");
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.Identity?.Name
                        ?? "Anonymous";
        var product = new Product
        {
            Name = req.Name, Description = req.Description, Price = req.Price, DateCreated = DateTime.UtcNow,
            CreatedByUser = userEmail
        };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);
        Response = new CreateProductResponse
        {
            Id = product.Id, Name = product.Name, Description = product.Description, Price = product.Price,
            DateCreated = product.DateCreated, CreatedByUser = product.CreatedByUser
        };
    }
}