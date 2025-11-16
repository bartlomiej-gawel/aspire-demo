using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationLocationOpeningHoursRange : ValueObject
{
    private OrganizationLocationOpeningHoursRange()
    {
    }

    private OrganizationLocationOpeningHoursRange(
        TimeOnly from,
        TimeOnly to,
        bool isEnabled)
    {
        From = from;
        To = to;
        IsEnabled = isEnabled;
    }

    public TimeOnly From { get; }
    public TimeOnly To { get; }
    public bool IsEnabled { get; }

    public static OrganizationLocationOpeningHoursRange Create(
        TimeOnly from,
        TimeOnly to,
        bool isEnabled)
    {
        if (isEnabled && from >= to)
            throw new ArgumentException("From time value must be earlier that to time value when enabled");

        return new OrganizationLocationOpeningHoursRange(
            from,
            to,
            isEnabled);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return From;
        yield return To;
        yield return IsEnabled;
    }
}