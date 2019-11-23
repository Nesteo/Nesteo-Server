using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nesteo.Server.Data.Entities.Identity;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Data.Entities
{
    [Table("Inspections")]
    public class InspectionEntity : IEntity<int?>
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public NestingBoxEntity NestingBox { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime InspectionDate { get; set; }

        [Required]
        public UserEntity InspectedByUser { get; set; }

        [Required]
        public bool HasBeenCleaned { get; set; }

        [Required]
        public Condition Condition { get; set; }

        [Required]
        public bool JustRepaired { get; set; }

        public bool? Occupied { get; set; }

        [Required]
        public bool ContainsEggs { get; set; }

        [Range(0, 100)]
        public int? EggCount { get; set; }

        [Range(0, 100)]
        public int? ChickCount { get; set; }

        [Required]
        [Range(0, 100)]
        public int RingedChickCount { get; set; }

        [Range(1, 100)]
        public int? AgeInDays { get; set; }

        [Required]
        public ParentBirdDiscovery FemaleParentBirdDiscovery { get; set; }

        [Required]
        public ParentBirdDiscovery MaleParentBirdDiscovery { get; set; }

        public SpeciesEntity Species { get; set; }

        [MaxLength(Constants.MaxImageFileNameLength)]
        public string ImageFileName { get; set; }

        public string Comment { get; set; }

        [Required]
        [Timestamp]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }
    }
}
