using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class CreateProjectRequest {

        public string _Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        
    }
}
