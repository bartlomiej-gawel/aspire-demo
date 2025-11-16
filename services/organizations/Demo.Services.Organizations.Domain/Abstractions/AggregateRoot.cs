namespace Demo.Services.Organizations.Domain.Abstractions;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }
}