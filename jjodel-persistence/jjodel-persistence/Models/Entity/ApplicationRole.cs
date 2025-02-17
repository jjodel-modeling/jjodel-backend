using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Entity {

    public class ApplicationRole : IdentityRole {
        public ICollection<ApplicationUserRole> ApplicationUsers { get; set; }
    }
}
