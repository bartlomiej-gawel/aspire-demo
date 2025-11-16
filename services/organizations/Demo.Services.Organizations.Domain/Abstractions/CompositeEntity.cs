namespace Demo.Services.Organizations.Domain.Abstractions;

public abstract class CompositeEntity<TKey1, TKey2> :
    IEntity,
    IEquatable<CompositeEntity<TKey1, TKey2>>,
    IComparable,
    IComparable<CompositeEntity<TKey1, TKey2>>
    where TKey1 : IComparable<TKey1>
    where TKey2 : IComparable<TKey2>
{
    protected CompositeEntity()
    {
    }

    protected abstract TKey1 Key1 { get; }
    protected abstract TKey2 Key2 { get; }

    private bool IsTransient()
    {
        var key1 = Key1;
        var key2 = Key2;

        return EqualityComparer<TKey1>.Default.Equals(key1, default!) ||
               EqualityComparer<TKey2>.Default.Equals(key2, default!);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        var other = (CompositeEntity<TKey1, TKey2>)obj;

        if (IsTransient() || other.IsTransient())
            return false;

        return EqualityComparer<TKey1>.Default.Equals(Key1, other.Key1) &&
               EqualityComparer<TKey2>.Default.Equals(Key2, other.Key2);
    }

    public bool Equals(CompositeEntity<TKey1, TKey2>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (other.GetType() != GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return EqualityComparer<TKey1>.Default.Equals(Key1, other.Key1) &&
               EqualityComparer<TKey2>.Default.Equals(Key2, other.Key2);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GetType());
        hash.Add(Key1, EqualityComparer<TKey1>.Default);
        hash.Add(Key2, EqualityComparer<TKey2>.Default);

        return hash.ToHashCode();
    }

    public static bool operator ==(CompositeEntity<TKey1, TKey2>? left, CompositeEntity<TKey1, TKey2>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(CompositeEntity<TKey1, TKey2>? left, CompositeEntity<TKey1, TKey2>? right)
    {
        return !(left == right);
    }

    public virtual int CompareTo(CompositeEntity<TKey1, TKey2>? other)
    {
        if (other is null)
            return 1;

        if (ReferenceEquals(this, other))
            return 0;

        var key1Comparison = Key1.CompareTo(other.Key1);
        if (key1Comparison != 0)
            return key1Comparison;

        return Key2.CompareTo(other.Key2);
    }

    public virtual int CompareTo(object? obj)
    {
        return CompareTo(obj as CompositeEntity<TKey1, TKey2>);
    }
}