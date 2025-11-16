using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationLocationId : EntityId<Guid>
{
    public OrganizationLocationId(Guid value) : base(value)
    {
    }

    public static OrganizationLocationId Create() => new(Guid.NewGuid());
    public static OrganizationLocationId CreateFrom(Guid value) => new(value);
}