using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Dto {
    public class ChangePasswordRequest {

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password field and Confirm Password field does not match!")]
        public string PasswordConfirm { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
