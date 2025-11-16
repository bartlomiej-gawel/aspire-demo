using Demo.Services.Organizations.Domain.Abstractions;

namespace Demo.Services.Organizations.Domain.Organizations;

public sealed class OrganizationEmployeeId : EntityId<Guid>
{
    public OrganizationEmployeeId(Guid value) : base(value)
    {
    }

    public static OrganizationEmployeeId Create() => new(Guid.NewGuid());
    public static OrganizationEmployeeId CreateFrom(Guid value) => new(value);
}