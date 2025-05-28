using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class RegisterRequest {

        public string? _Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Affiliation { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public bool NewsletterEnabled { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]       
        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
