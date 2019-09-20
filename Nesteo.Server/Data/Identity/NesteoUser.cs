using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nesteo.Server.Data.Identity
{
    public class NesteoUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
