using System.ComponentModel.DataAnnotations;

namespace Nesteo.Server.Models
{
    public class Region
    {
        /// <summary>
        /// Region-ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name or description of the region's location
        /// </summary>
        [Required]
        [MaxLength(Constants.MaxOwnerNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Prefix for all nesting box IDs in this region
        /// </summary>
        [StringLength(10, MinimumLength = 1)]
        public string NestingBoxIdPrefix { get; set; }
    }
}
