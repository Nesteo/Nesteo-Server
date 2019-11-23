using System;
using System.ComponentModel.DataAnnotations;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Models
{
    public class InspectionExportRow
    {
        /// <summary>
        /// Inspection-ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The inspected nesting box Id
        /// </summary>
        [Required]
        public string NestingBoxId { get; set; }

        /// <summary>
        /// Date and time of the inspection
        /// </summary>
        [Required]
        public DateTime? InspectionDate { get; set; }

        /// <summary>
        /// Inspection User
        /// </summary>
        [Required]
        public User InspectionUser { get; set; }

        /// <summary>
        /// Whether the nesting box has been cleaned during the inspection
        /// </summary>
        [Required]
        public bool? HasBeenCleaned { get; set; }

        /// <summary>
        /// The condition in which the nesting box has been found
        /// </summary>
        [Required]
        [EnumDataType(typeof(Condition))]
        public Condition? Condition { get; set; }

        /// <summary>
        /// Whether the nesting box has been repaired during the inspection
        /// </summary>
        [Required]
        public bool? JustRepaired { get; set; }

        /// <summary>
        /// Was the nesting box occupied by any bird?
        /// </summary>
        [Required]
        public bool? Occupied { get; set; }

        /// <summary>
        /// Does the nesting box contain eggs?
        /// </summary>
        [Required]
        public bool? ContainsEggs { get; set; }

        /// <summary>
        /// Count of eggs (or null when unknown)
        /// </summary>
        [Range(0, 100)]
        public int? EggCount { get; set; }

        /// <summary>
        /// Number of chicks
        /// </summary>
        [Required]
        [Range(0, 100)]
        public int? ChickCount { get; set; }

        /// <summary>
        /// Number of ringed chicks
        /// </summary>
        [Required]
        [Range(0, 100)]
        public int? RingedChickCount { get; set; }

        /// <summary>
        /// Age of the chicks (or null when unknown)
        /// </summary>
        [Range(0, 100)]
        public int? AgeInDays { get; set; }

        /// <summary>
        /// Does the nesting box contain a female parent?
        /// </summary>
        [Required]
        public ParentBirdDiscovery? FemaleParent { get; set; }

        /// <summary>
        /// Does the nesting box contain a male parent?
        /// </summary>
        [Required]
        public ParentBirdDiscovery? MaleParent { get; set; }

        /// <summary>
        /// The bird species
        /// </summary>
        [Required]
        [EnumDataType(typeof(Species))]
        public Species? Species { get; set; }

        /// <summary>
        /// Image file name
        /// </summary>
        public string ImageFilename { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The time this data entry has last been updated
        /// </summary>
        public DateTime? LastUpdated { get; set; }

    }
}
