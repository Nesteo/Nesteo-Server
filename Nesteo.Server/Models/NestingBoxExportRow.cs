using System;
using System.ComponentModel.DataAnnotations;
using Nesteo.Server.Data.Enums;

namespace Nesteo.Server.Models
{
    public class NestingBoxExportRow
    {
        /// <summary>
        /// Nesting box ID
        /// </summary>
        [StringLength(Constants.NestingBoxIdLength, MinimumLength = Constants.NestingBoxIdLength)]
        public string Id { get; set; }

        /// <summary>
        /// The old ID (if any)
        /// </summary>
        [StringLength(100, MinimumLength = 2)]
        public string OldId { get; set; }

        /// <summary>
        /// The foreign ID (if any)
        /// </summary>
        [StringLength(100, MinimumLength = 2)]
        public string ForeignId { get; set; }

        /// <summary>
        /// Region, where the nesting box hangs
        /// </summary>
        [Required]
        public string RegionName { get; set; }

        /// <summary>
        /// Decimal coordinate longitude
        /// </summary>
        public double? CoordinateLongitude { get; set; }

        /// <summary>
        /// Decimal coordinate latitude
        /// </summary>
        public double? CoordinateLatitude { get; set; }

        /// <summary>
        /// Hang up date (if known)
        /// </summary>
        public DateTime? HangUpDate { get; set; }

        /// <summary>
        /// Hang up user (if known)
        /// </summary>
        public string HangUpUserName { get; set; }

        /// <summary>
        /// Owner of the nesting box
        /// </summary>
        [Required]
        public string OwnerName { get; set; }

        /// <summary>
        /// Material
        /// </summary>
        [Required]
        [EnumDataType(typeof(Material))]
        public Material Material { get; set; }

        /// <summary>
        /// Size of the hole
        /// </summary>
        [Required]
        [EnumDataType(typeof(HoleSize))]
        public HoleSize? HoleSize { get; set; }

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
