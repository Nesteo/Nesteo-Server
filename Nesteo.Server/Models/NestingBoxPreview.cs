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
    }
}
