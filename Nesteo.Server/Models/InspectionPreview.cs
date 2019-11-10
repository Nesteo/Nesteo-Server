using System;
using System.ComponentModel.DataAnnotations;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Models
{
    public class InspectionPreview
    {
        /// <summary>
        /// Inspection-ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The id of the inspected nesting box
        /// </summary>
        [Required]
        public string NestingBoxId { get; set; }

        /// <summary>
        /// Date and time of the inspection
        /// </summary>
        [Required]
        public DateTime? InspectionDate { get; set; }

        /// <summary>
        /// The condition in which the nesting box has been found
        /// </summary>
        [Required]
        [EnumDataType(typeof(Condition))]
        public Condition? Condition { get; set; }

        /// <summary>
        /// Number of ringed chicks
        /// </summary>
        [Required]
        [Range(0, 100)]
        public int? RingedChickCount { get; set; }

        /// <summary>
        /// Information about the presence of the female parent bird
        /// </summary>
        [Required]
        [EnumDataType(typeof(ParentBirdDiscovery))]
        public ParentBirdDiscovery? FemaleParentBirdDiscovery { get; set; }

        /// <summary>
        /// Information about the presence of the male parent bird
        /// </summary>
        [Required]
        [EnumDataType(typeof(ParentBirdDiscovery))]
        public ParentBirdDiscovery? MaleParentBirdDiscovery { get; set; }
    }
}
