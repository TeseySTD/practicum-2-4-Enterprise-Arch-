using System.Security.Claims;

namespace Nimble.Modulith.Customers.Infrastructure;

public interface ICustomerAuthorizationService
{
    bool IsAdminOrOwner(ClaimsPrincipal user, string customerEmail);
}