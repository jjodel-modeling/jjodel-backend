using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace jjodel_persistence.Models.Entity {

    [Index(nameof(Name), IsUnique = true)] // name is unique
    public class Tag {

        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        
        public string Name { get; set; }

        public string? Description { get; set; }

        public string Color { get; set; }

        public DateTime  Creation { get; set; }

        public ICollection<Project> Projects { get; set; }

        public string AuthorId { get; set; }  // FK
        public ApplicationUser Author { get; set; } // Navigation
    }
}
