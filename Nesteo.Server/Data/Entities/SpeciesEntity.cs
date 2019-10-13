using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nesteo.Server.Data.Entities
{
    [Table("Species")]
    public class SpeciesEntity : IEntity<int>
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MaxOwnerNameLength)]
        public string Name { get; set; }

        public ICollection<InspectionEntity> Inspections { get; set; }
    }
}
