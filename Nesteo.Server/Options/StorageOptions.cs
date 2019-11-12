using System.ComponentModel.DataAnnotations;

namespace Nesteo.Server.Options
{
    public class StorageOptions
    {
        [Required]
        public string ImageUploadsDirectoryPath { get; set; }
    }
}
