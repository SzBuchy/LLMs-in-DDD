namespace Core.Entities;

public record Address(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
