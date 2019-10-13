using System;

namespace Nesteo.Server.Models
{
    public class NestingBoxPreview
    {
        /// <summary>
        /// Nesting box ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Region, where the nesting box hangs
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// Decimal coordinate longitude
        /// </summary>
        public double? CoordinateLongitude { get; set; }

        /// <summary>
        /// Decimal coordinate latitude
        /// </summary>
        public double? CoordinateLatitude { get; set; }

        /// <summary>
        /// How often this box has been inspected
        /// </summary>
        public int InspectionsCount { get; set; }

        /// <summary>
        /// The last time this box has been inspected
        /// </summary>
        public DateTime LastInspected { get; set; }
    }
}
