using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Mail {
    public class ConfirmAccount {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string Id { get; set; }
    }
}
