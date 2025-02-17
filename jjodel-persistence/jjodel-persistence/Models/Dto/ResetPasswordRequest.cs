using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class ResetPasswordRequest {

        [Required]
        public string Nickname { get; set; }
    }
}
