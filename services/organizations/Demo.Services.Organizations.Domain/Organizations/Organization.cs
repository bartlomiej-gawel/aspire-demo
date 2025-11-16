using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class Organization : AggregateRoot<OrganizationId>
{
    private readonly List<OrganizationLocation> _locations = [];
    private readonly List<OrganizationEmployee> _employees = [];

    private Organization()
    {
    }

    private Organization(OrganizationId id, string name) : base(id)
    {
        Name = name;
        Status = OrganizationStatus.Inactive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public string Name { get; private set; } = null!;
    public OrganizationStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyList<OrganizationLocation> Locations => _locations.AsReadOnly();
    public IReadOnlyList<OrganizationEmployee> Employees => _employees.AsReadOnly();

    public static Organization Create(string name)
    {
        return new Organization(OrganizationId.Create(), name);
    }
}