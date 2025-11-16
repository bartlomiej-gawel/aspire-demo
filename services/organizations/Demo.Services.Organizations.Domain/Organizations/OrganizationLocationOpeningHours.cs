using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationLocationOpeningHours : ValueObject
{
    private OrganizationLocationOpeningHours()
    {
        Weekly = new Dictionary<DayOfWeek, OrganizationLocationOpeningHoursRange>();
    }

    private OrganizationLocationOpeningHours(Dictionary<DayOfWeek, OrganizationLocationOpeningHoursRange> weekly)
    {
        Weekly = new Dictionary<DayOfWeek, OrganizationLocationOpeningHoursRange>(weekly);
    }

    public IReadOnlyDictionary<DayOfWeek, OrganizationLocationOpeningHoursRange> Weekly { get; }

    public static OrganizationLocationOpeningHours Create(Dictionary<DayOfWeek, OrganizationLocationOpeningHoursRange> weekly)
    {
        var days = Enum.GetValues<DayOfWeek>();

        if (weekly is null)
            throw new ArgumentException("Weekly dictionary cannot be null");

        if (weekly.Count != days.Length || days.Any(day => !weekly.ContainsKey(day)) || weekly.Any(_ => false))
            throw new ArgumentException("Weekly dictionary must contain all days of the week");

        var normalizedWeeklyDictionary = weekly
            .OrderBy(keyValuePair => keyValuePair.Key)
            .ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value);

        return new OrganizationLocationOpeningHours(normalizedWeeklyDictionary);
    }

    public static OrganizationLocationOpeningHours CreateDefault()
    {
        var hours = OrganizationLocationOpeningHoursRange.Create(
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true);

        var weeklyDictionary = Enum
            .GetValues<DayOfWeek>()
            .ToDictionary(
                dayOfWeek => dayOfWeek,
                _ => hours);

        return Create(weeklyDictionary);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        foreach (var kvp in Weekly.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}