using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class Organization : AggregateRoot<OrganizationId>
{
    private readonly List<OrganizationLocation> _locations = [];
    private readonly List<OrganizationEmployee> _employees = [];

    private Organization()
    {
    }

    private Organization(
        OrganizationId id,
        OrganizationSubscription subscription,
        string name) : base(id)
    {
        Name = name;
        Subscription = subscription;
        Status = OrganizationStatus.Inactive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public string Name { get; private set; } = null!;
    public OrganizationSubscription Subscription { get; private set; } = null!;
    public OrganizationStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyList<OrganizationLocation> Locations => _locations.AsReadOnly();
    public IReadOnlyList<OrganizationEmployee> Employees => _employees.AsReadOnly();

    public static Organization CreateFromBackoffice(
        string organizationName,
        List<OrganizationEmployeeData> organizationAdmins)
    {
        if (organizationAdmins.Count == 0)
            throw new ArgumentException("At least one admin is required");

        var organizationId = OrganizationId.Create();

        var organizationSubscription = OrganizationSubscription.CreateBackofficeTrial(organizationAdmins.Count);

        var organization = new Organization(
            organizationId,
            organizationSubscription,
            organizationName);

        var defaultLocation = OrganizationLocation.CreateDefault(
            organizationId,
            organizationName);

        organization._locations.Add(defaultLocation);

        foreach (var organizationAdmin in organizationAdmins)
        {
            var admin = OrganizationEmployee.CreateAdmin(
                organizationId,
                defaultLocation.Id,
                organizationAdmin.FirstName,
                organizationAdmin.LastName,
                organizationAdmin.Email,
                organizationAdmin.Phone);

            organization._employees.Add(admin);
        }

        return organization;
    }

    public void Archive()
    {
        if (Status is OrganizationStatus.Archived)
            throw new InvalidOperationException("Organization is already archived");

        if (Subscription.Status is not OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot archive organization while subscription is still valid");

        foreach (var location in _locations)
            location.Archive();

        var employeesToArchive = _employees
            .Where(employee => employee.Role is not OrganizationEmployeeRole.Admin)
            .ToList();

        foreach (var employee in employeesToArchive)
            employee.Archive();

        Status = OrganizationStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void OnSubscriptionCanceled()
    {
        if (Subscription.Status is OrganizationSubscriptionStatus.Canceled or OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Subscription is already canceled or expired");

        var canceledSubscription = OrganizationSubscription.CreateCanceled(
            Subscription.Plan,
            Subscription.ExpiresAt,
            Subscription.TotalSeats,
            Subscription.AvailableSeats);

        Subscription = canceledSubscription;
        UpdatedAt = DateTime.UtcNow;
    }
}