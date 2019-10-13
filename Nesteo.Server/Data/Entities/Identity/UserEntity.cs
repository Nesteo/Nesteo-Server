using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nesteo.Server.Data.Entities.Identity
{
    public class UserEntity : IdentityUser, IEntity<string>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
