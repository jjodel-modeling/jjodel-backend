using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Entity {
    public class ApplicationUserRole : IdentityUserRole<string> {
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ApplicationRole ApplicationRole { get; set; }
    }
}
