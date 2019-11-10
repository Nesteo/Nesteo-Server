using System.ComponentModel.DataAnnotations;

namespace Nesteo.Server.Models
{
    public class Owner
    {
        /// <summary>
        /// Owner-ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name of the owner
        /// </summary>
        [Required]
        [MaxLength(Constants.MaxOwnerNameLength)]
        public string Name { get; set; }
    }
}
