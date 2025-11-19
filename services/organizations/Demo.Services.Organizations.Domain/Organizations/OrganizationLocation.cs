using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationLocation : Entity<OrganizationLocationId>
{
    private OrganizationLocation()
    {
    }

    private OrganizationLocation(
        OrganizationLocationId id,
        OrganizationId organizationId,
        string name,
        OrganizationLocationOpeningHours openingHours,
        OrganizationLocationStatus status) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        OpeningHours = openingHours;
        Address = null;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public OrganizationId OrganizationId { get; } = null!;
    public string Name { get; private set; } = null!;
    public OrganizationLocationOpeningHours OpeningHours { get; private set; } = null!;
    public OrganizationLocationAddress? Address { get; private set; }
    public OrganizationLocationStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public static OrganizationLocation CreateDefault(
        OrganizationId organizationId,
        string organizationName)
    {
        return new OrganizationLocation(
            OrganizationLocationId.Create(),
            organizationId,
            $"{organizationName} Headquarters",
            OrganizationLocationOpeningHours.CreateDefault(),
            OrganizationLocationStatus.Active);
    }

    public void Archive()
    {
        if (Status is OrganizationLocationStatus.Archived)
            throw new InvalidOperationException("Organization location is already archived");

        Status = OrganizationLocationStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}