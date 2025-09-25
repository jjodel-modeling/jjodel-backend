using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class LoginResponse {

        [Required]
        public string Token { get; set; }
        public DateTime Expires { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

    }
}
