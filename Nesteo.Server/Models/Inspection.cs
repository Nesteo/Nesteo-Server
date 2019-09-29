using System;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Models
{
    public class Inspection
    {
        /// <summary>
        /// Inspection-ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The inspected nesting box
        /// </summary>
        public NestingBox NestingBox { get; set; }

        /// <summary>
        /// Date and time of the inspection
        /// </summary>
        public DateTime InspectionDate { get; set; }

        /// <summary>
        /// The user who inspected the nesting box (if known)
        /// </summary>
        public User InspectedByUser { get; set; }

        /// <summary>
        /// Whether the nesting box has been cleaned during the inspection
        /// </summary>
        public bool HasBeenCleaned { get; set; }

        /// <summary>
        /// The condition in which the nesting box has been found
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Whether the nesting box has been repaired during the inspection
        /// </summary>
        public bool JustRepaired { get; set; }

        /// <summary>
        /// Was the nesting box occupied by any bird?
        /// </summary>
        public bool Occupied { get; set; }

        /// <summary>
        /// Were any eggs in there?
        /// </summary>
        public bool ContainsEggs { get; set; }

        /// <summary>
        /// Count of eggs (or null when unknown)
        /// </summary>
        public int? EggCount { get; set; }

        /// <summary>
        /// Number of slipped chicks
        /// </summary>
        public int ChickCount { get; set; }

        /// <summary>
        /// Number of ringed chicks
        /// </summary>
        public int RingedChickCount { get; set; }

        /// <summary>
        /// Age of the chicks (or null when unknown)
        /// </summary>
        public int? AgeInDays { get; set; }

        /// <summary>
        /// Information about the presence of the female parent bird
        /// </summary>
        public ParentBirdDiscovery FemaleParentBirdDiscovery { get; set; }

        /// <summary>
        /// Information about the presence of the male parent bird
        /// </summary>
        public ParentBirdDiscovery MaleParentBirdDiscovery { get; set; }

        /// <summary>
        /// The bird species
        /// </summary>
        public Species Species { get; set; }

        /// <summary>
        /// Name of the image of this nesting box inspection (if any)
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The time this data entry has last been updated
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
