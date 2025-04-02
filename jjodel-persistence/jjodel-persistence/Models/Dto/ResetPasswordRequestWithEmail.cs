using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class ResetPasswordRequestWithEmail {


        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
