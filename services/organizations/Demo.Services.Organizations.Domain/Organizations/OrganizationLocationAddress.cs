using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationLocationAddress : ValueObject
{
    private OrganizationLocationAddress()
    {
    }

    private OrganizationLocationAddress(
        string country,
        string city,
        string street,
        string postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }

    public string Country { get; } = null!;
    public string City { get; } = null!;
    public string Street { get; } = null!;
    public string PostalCode { get; } = null!;

    public static OrganizationLocationAddress Create(
        string country,
        string city,
        string street,
        string postalCode)
    {
        return new OrganizationLocationAddress(
            country,
            city,
            street,
            postalCode);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Street;
        yield return PostalCode;
    }
}