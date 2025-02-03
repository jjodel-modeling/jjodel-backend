using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Settings {
    public class Identity {

        public bool SignInRequireConfirmedAccount { get; set; }
        public int RequiredLength { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
    
  
    }
}
