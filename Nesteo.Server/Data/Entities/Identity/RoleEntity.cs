using Microsoft.AspNetCore.Identity;

namespace Nesteo.Server.Data.Entities.Identity
{
    public class RoleEntity : IdentityRole, IEntity<string> { }
}
