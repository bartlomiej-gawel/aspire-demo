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
        OrganizationEmployeeRole role) : base(id)
    {
        OrganizationId = organizationId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = null!;
        Status = status;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null!;
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
}