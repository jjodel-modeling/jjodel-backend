using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class CreateTagRequest {

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string Color { get; set; }

    
    }
}
