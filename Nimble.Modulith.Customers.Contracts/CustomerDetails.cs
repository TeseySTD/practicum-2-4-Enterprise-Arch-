namespace Nimble.Modulith.Customers.Contracts;

public record CustomerDetails(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    AddressDetails Address
);

public record AddressDetails(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country
);