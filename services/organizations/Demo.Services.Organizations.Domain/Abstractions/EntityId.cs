namespace Demo.Services.Organizations.Domain.Abstractions;

public abstract class EntityId<T> : ValueObject
    where T : notnull
{
    public T Value { get; }

    protected EntityId(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }

    public static implicit operator T(EntityId<T> id)
    {
        return id.Value;
    }
}