using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace jjodel_persistence.Models.Entity {
    public class ClientLog {

        [Key]
        [Required]
        public Guid Id { get; set; }
        [AllowedValues("Error", "Warning", "Information")]
        public string Level { get; set; }
        public string? Title { get; set; } 
        public string? Version { get; set; } //client version
        public string? DState { get; set; }
        public DateTime Creation { get; set; }

        public string? Message { get; set; } 
        public string? StackTrace { get; set; } 
        public string? CompoStack { get; set; }

        public string? ContextJson { get; set; }
        public string? TransientJson { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
