using System;
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
        public string NestingBoxId { get; set; }

        /// <summary>
        /// Date and time of the inspection
        /// </summary>
        public DateTime InspectionDate { get; set; }

        /// <summary>
        /// The condition in which the nesting box has been found
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// Number of ringed chicks
        /// </summary>
        public int RingedChickCount { get; set; }

        /// <summary>
        /// Information about the presence of the female parent bird
        /// </summary>
        public ParentBirdDiscovery FemaleParentBirdDiscovery { get; set; }

        /// <summary>
        /// Information about the presence of the male parent bird
        /// </summary>
        public ParentBirdDiscovery MaleParentBirdDiscovery { get; set; }
    }
}
