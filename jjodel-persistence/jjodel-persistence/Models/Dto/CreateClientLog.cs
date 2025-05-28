using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class CreateClientLog {

        [Required]
        [AllowedValues("Error", "Warning", "Information")]
        public string Level { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Version { get; set; }

        [Required]
        public DateTime Creation { get; set; }

        [Required]
        public string Message { get; set; }

        public string? StackTrace { get; set; }
        public string? CompoStack { get; set; }
        public string? ContextJson { get; set; }
        public string? DState { get; set; }
        public string? TransientJson { get; set; }

    }
}
