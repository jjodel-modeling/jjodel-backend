using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Entity {
    public class ClientLog {

        [Key]
        [Required]
        public Guid Id { get; set; }

        [AllowedValues("Log", "Information", "Warning", "Error", "Exception", "DevError", "DevException")]
        public string Level { get; set; }

        public string? Url { get; set; }

        [Required]
        public string Version { get; set; } //client version

        public string? State { get; set; } //D state

        [Required]
        public DateTime Creation { get; set; }

        public string Message { get; set; }

        public string Error { get; set; }

        public string? CompoStack { get; set; }

        public string? ReactMsg { get; set; }


        // Browser Info
        public string Screen { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string BrowserMajorVersion { get; set; }
        public bool Mobile { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Cookies { get; set; }
        public string UserAgent { get; set; }


        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
