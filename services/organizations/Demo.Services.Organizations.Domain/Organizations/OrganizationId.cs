using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationId : EntityId<Guid>
{
    public OrganizationId(Guid value) : base(value)
    {
    }

    public static OrganizationId Create() => new(Guid.NewGuid());
    public static OrganizationId CreateFrom(Guid value) => new(value);
}