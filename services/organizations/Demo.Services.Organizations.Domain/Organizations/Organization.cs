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
        if (Subscription.Status is not OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot archive organization while subscription is still valid");

        if (Status is OrganizationStatus.Archived)
            throw new InvalidOperationException("Organization is already archived");

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

    public void AddLocation(
        string locationName,
        OrganizationLocationOpeningHours locationOpeningHours,
        OrganizationLocationAddress locationAddress)
    {
        if (Subscription.Status is OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot add location to organization with expired subscription");

        if (Status is OrganizationStatus.Archived)
            throw new InvalidOperationException("Cannot add location to archived organization");

        var location = OrganizationLocation.Create(
            Id,
            locationName,
            locationOpeningHours,
            locationAddress);

        _locations.Add(location);

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(
        OrganizationLocationId locationId,
        string locationName,
        OrganizationLocationOpeningHours locationOpeningHours,
        OrganizationLocationAddress locationAddress)
    {
        if (Subscription.Status is OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot update location of organization with expired subscription");

        if (Status is OrganizationStatus.Archived)
            throw new InvalidOperationException("Cannot update location of archived organization");

        var location = _locations.FirstOrDefault(x => x.Id == locationId);
        if (location is null)
            throw new InvalidOperationException("Cannot update location that does not exist");

        location.Update(
            locationName,
            locationOpeningHours,
            locationAddress);

        UpdatedAt = DateTime.UtcNow;
    }

    public void ActivateLocation(OrganizationLocationId locationId)
    {
        if (Subscription.Status is OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot activate location of organization with expired subscription");

        if (Status is OrganizationStatus.Archived)
            throw new InvalidOperationException("Cannot activate location of archived organization");

        var location = _locations.FirstOrDefault(x => x.Id == locationId);
        if (location is null)
            throw new InvalidOperationException("Cannot activate location that does not exist");

        location.Activate();

        UpdatedAt = DateTime.UtcNow;
    }

    public void ArchiveLocation(OrganizationLocationId locationId)
    {
        if (Subscription.Status is OrganizationSubscriptionStatus.Expired)
            throw new InvalidOperationException("Cannot archive location of organization with expired subscription");

        var location = _locations.FirstOrDefault(x => x.Id == locationId);
        if (location is null)
            throw new InvalidOperationException("Cannot archive location that does not exist");

        var activeLocationsCount = _locations.Count(x => x.Status is OrganizationLocationStatus.Active);
        if (activeLocationsCount <= 1)
            throw new InvalidOperationException("Cannot archive last active location of organization");

        var blockingEmployees = _employees
            .Where(x =>
                x.Status is OrganizationEmployeeStatus.Active &&
                x.LocationIds.Contains(locationId) &&
                x.LocationIds.Count == 1)
            .ToList();

        if (blockingEmployees.Count > 0)
            throw new InvalidOperationException("Cannot archive location. Reassign employees who are assigned only to this location.");

        location.Archive();

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