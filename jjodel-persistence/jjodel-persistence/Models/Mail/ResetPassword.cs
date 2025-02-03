using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Mail {
    public class ResetPassword {

        [Required]
        public string Username { get; set; }
        [Required]
        public string NewPassoword { get; set; }
    }
}
