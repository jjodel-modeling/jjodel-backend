using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class LoginRequest {
       
        [EmailAddress]
        [Required]
        public string Email {  get; set; }

        [Required]
        public string Password {  get; set; }
    }
}
