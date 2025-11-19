using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationSubscription : ValueObject
{
    private const int TrialPeriodInDays = 14;
    private const int TrialSeatLimit = 50;

    private OrganizationSubscription()
    {
    }

    private OrganizationSubscription(
        OrganizationSubscriptionPlan plan,
        OrganizationSubscriptionStatus status,
        DateTime expiresAt,
        int totalSeats,
        int availableSeats)
    {
        Plan = plan;
        Status = status;
        ExpiresAt = expiresAt;
        TotalSeats = totalSeats;
        AvailableSeats = availableSeats;
    }

    public OrganizationSubscriptionPlan Plan { get; }
    public OrganizationSubscriptionStatus Status { get; }
    public DateTime ExpiresAt { get; }
    public int TotalSeats { get; }
    public int AvailableSeats { get; }

    public static OrganizationSubscription CreateBackofficeTrial(int organizationAdminsCount)
    {
        return new OrganizationSubscription(
            OrganizationSubscriptionPlan.Platinum,
            OrganizationSubscriptionStatus.Trial,
            DateTime.UtcNow.AddDays(TrialPeriodInDays),
            TrialSeatLimit,
            TrialSeatLimit - organizationAdminsCount);
    }

    public static OrganizationSubscription CreateCanceled(
        OrganizationSubscriptionPlan plan,
        DateTime expiresAt,
        int totalSeats,
        int availableSeats)
    {
        return new OrganizationSubscription(
            plan,
            OrganizationSubscriptionStatus.Canceled,
            expiresAt,
            totalSeats,
            availableSeats);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Plan;
        yield return Status;
        yield return ExpiresAt;
        yield return TotalSeats;
        yield return AvailableSeats;
    }
}