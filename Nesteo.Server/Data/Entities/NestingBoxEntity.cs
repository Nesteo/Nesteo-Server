using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Data.Entities
{
    [Table("NestingBoxes")]
    public class NestingBoxEntity : IEntity<string>
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(6, MinimumLength = 6)]
        public string Id { get; set; }

        [Required]
        public RegionEntity Region { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string OldId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ForeignId { get; set; }

        public double? CoordinateLongitude { get; set; }

        public double? CoordinateLatitude { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? HangUpDate { get; set; }

        public UserEntity HangUpUser { get; set; }

        [Required]
        public OwnerEntity Owner { get; set; }

        [Required]
        public Material Material { get; set; }

        [Required]
        public HoleSize HoleSize { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; }

        public string Comment { get; set; }

        [Required]
        [Timestamp]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }

        public ICollection<InspectionEntity> Inspections { get; set; }
    }
}
