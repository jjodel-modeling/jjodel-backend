using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class RefreshTokenRequest {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
