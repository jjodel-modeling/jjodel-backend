using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Settings {
    public class MailSettings {
        public string FromDefault { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public bool UseSSL { get; set; }
        public bool RequiresAuthentication { get; set; }

    }
}
