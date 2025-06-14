using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class CreateProjectRequest {

        public string? _Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public bool Imported { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public int ViewpointsNumber  { get; set; }

        [Required]
        public int MetamodelsNumber { get; set; }

        [Required]
        public int ModelsNumber { get; set; }

        [Required]
        public bool IsFavorite { get; set; }

        // present only if imported.
        public string? State { get; set; }
    }
}
