using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class TagResponse {

        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string Color { get; set; }

        public DateTime Creation { get; set; }

        public string Author { get; set; }  

        public List<ProjectShortResponse> Projects { get; set; }
    }
}
