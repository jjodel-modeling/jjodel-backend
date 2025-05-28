using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class UserResponse {
        [Required]
        public String Id { get; set; }

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
        public bool Newsletter { get; set; } // map to newsletter enabled

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime BirthDate { get; set; }
        
    }
}
