using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace jjodel_persistence.Models.Entity {
    public class Project {
        [Key]
        [Required]
        public Guid Id { get; set; }

        public string _Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string State { get; set; } // project content

        [Required]
        public double ViewpointsNumber { get; set; } // relationship?

        [Required]
        public double MetamodelsNumber { get; set; } // relationship?

        [Required]
        public double ModelsNumber { get; set; } // relationship?

        public DateTime Creation { get; set; }
        public DateTime LastModified { get; set; }

        public bool IsFavorite { get; set; }

        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }

        public ICollection<ApplicationUser> Collaborators { get; set; }

    }


}
