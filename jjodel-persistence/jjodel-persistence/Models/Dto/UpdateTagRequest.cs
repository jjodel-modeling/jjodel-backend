using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class UpdateTagRequest {

        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

    }
}
