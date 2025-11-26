using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationEmployee : Entity<OrganizationEmployeeId>
{
    private readonly List<OrganizationLocationId> _locationIds = [];

    private OrganizationEmployee()
    {
    }

    private OrganizationEmployee(
        OrganizationEmployeeId id,
        OrganizationId organizationId,
        string firstName,
        string lastName,
        string email,
        OrganizationEmployeeStatus status,
        OrganizationEmployeeRole role,
        string? phone = null) : base(id)
    {
        OrganizationId = organizationId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Status = status;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public OrganizationId OrganizationId { get; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public OrganizationEmployeeStatus Status { get; private set; }
    public OrganizationEmployeeRole Role { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyList<OrganizationLocationId> LocationIds => _locationIds.AsReadOnly();

    public static OrganizationEmployee Create(
        OrganizationId organizationId,
        OrganizationEmployeeRole role,
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        return new OrganizationEmployee(
            OrganizationEmployeeId.Create(),
            organizationId,
            firstName,
            lastName,
            email,
            OrganizationEmployeeStatus.Inactive,
            role,
            phone);
    }

    public void Archive()
    {
        if (Status is OrganizationEmployeeStatus.Archived)
            throw new InvalidOperationException("Employee is already archived");

        Status = OrganizationEmployeeStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToLocation(OrganizationLocationId locationId)
    {
        if (Status is OrganizationEmployeeStatus.Archived)
            throw new InvalidOperationException("Cannot assign archived employee to location");

        _locationIds.Add(locationId);

        UpdatedAt = DateTime.UtcNow;
    }
}