using Ardalis.GuardClauses;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

// Value Object: equality is based on all properties, not on identity (no Id, immutable).
public record PickupPoint
{
    public string Name { get; }
    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    public PickupPoint(string name, string street, string city, string zipCode)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.NullOrEmpty(street, nameof(street));
        Guard.Against.NullOrEmpty(city, nameof(city));
        Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));

        Name = name;
        Street = street;
        City = city;
        ZipCode = zipCode;
    }
}
