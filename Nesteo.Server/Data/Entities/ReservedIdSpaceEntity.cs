using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.Data.Entities
{
    [Table("ReservedIdSpaces")]
    public class ReservedIdSpaceEntity : IEntity<int>
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public RegionEntity Region { get; set; }

        [Required]
        public UserEntity Owner { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }

        [Required]
        public int FirstNestingBoxIdWithoutPrefix { get; set; }

        [Required]
        public int LastNestingBoxIdWithoutPrefix { get; set; }
    }
}
