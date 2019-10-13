using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nesteo.Server.Data.Entities
{
    [Table("Regions")]
    public class RegionEntity : IEntity<int>
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MaxRegionNameLength)]
        public string Name { get; set; }

        [StringLength(10, MinimumLength = 1)]
        public string NestingBoxIdPrefix { get; set; }

        public ICollection<NestingBoxEntity> NestingBoxes { get; set; }
    }
}
