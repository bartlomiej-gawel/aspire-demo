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
        OrganizationLocationStatus status,
        OrganizationLocationOpeningHours openingHours,
        OrganizationLocationAddress? address = null) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Status = status;
        OpeningHours = openingHours;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public OrganizationId OrganizationId { get; } = null!;
    public string Name { get; private set; } = null!;
    public OrganizationLocationStatus Status { get; private set; }
    public OrganizationLocationOpeningHours OpeningHours { get; private set; } = null!;
    public OrganizationLocationAddress? Address { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public static OrganizationLocation CreateDefault(
        OrganizationId organizationId,
        string organizationName)
    {
        return new OrganizationLocation(
            OrganizationLocationId.Create(),
            organizationId,
            $"{organizationName} HQ",
            OrganizationLocationStatus.Active,
            OrganizationLocationOpeningHours.CreateDefault());
    }

    public static OrganizationLocation Create(
        OrganizationId organizationId,
        string name,
        OrganizationLocationOpeningHours openingHours,
        OrganizationLocationAddress address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty");

        return new OrganizationLocation(
            OrganizationLocationId.Create(),
            organizationId,
            name,
            OrganizationLocationStatus.Active,
            openingHours,
            address);
    }

    public void Update(
        string name,
        OrganizationLocationOpeningHours openingHours,
        OrganizationLocationAddress address)
    {
        if (Status is OrganizationLocationStatus.Archived)
            throw new InvalidOperationException("Cannot update archived location");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty");

        Name = name;
        OpeningHours = openingHours;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status is OrganizationLocationStatus.Active)
            throw new InvalidOperationException("Location is already active");

        Status = OrganizationLocationStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status is OrganizationLocationStatus.Archived)
            throw new InvalidOperationException("Location is already archived");

        Status = OrganizationLocationStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}