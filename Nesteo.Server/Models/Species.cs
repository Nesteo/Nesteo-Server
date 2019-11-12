using System.ComponentModel.DataAnnotations;

namespace Nesteo.Server.Models
{
    public class Species
    {
        /// <summary>
        /// Species-ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name of the species
        /// </summary>
        [Required]
        [MaxLength(Constants.MaxSpeciesNameLength)]
        public string Name { get; set; }
    }
}
