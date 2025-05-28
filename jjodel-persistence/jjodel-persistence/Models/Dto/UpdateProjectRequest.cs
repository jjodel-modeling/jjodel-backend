using jjodel_persistence.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class UpdateProjectRequest {

        [Required]
        public Guid Id { get; set; }

        public string? _Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        //[Required]
        public string State { get; set; }

        [Required]
        public double ViewpointsNumber { get; set; } // relationship?

        [Required]
        public double MetamodelsNumber { get; set; } // relationship?

        [Required]
        public double ModelsNumber { get; set; } // relationship?

        public DateTime LastModified { get; set; }

        public bool IsFavorite { get; set; }

        public ICollection<string> Collaborators { get; set; }


    }
}
