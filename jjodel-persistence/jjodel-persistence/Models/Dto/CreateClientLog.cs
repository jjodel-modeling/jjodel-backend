using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace jjodel_persistence.Models.Dto {
    public class CreateClientLog {

        [Required]
        [AllowedValues("Log", "Information", "Warning", "Error", "Exception", "DevError", "DevException")]
        public string Level { get; set; }

        public string? Url { get; set; }

        [Required]
        public string Version { get; set; }

        public string? State { get; set; } //D state { state: "{a:"\pippo\"}" "}

        [Required]
        public DateTime Creation { get; set; }

        [Required]
        public CreateClientLogError Error { get; set; }

        public string? CompoStack { get; set; }
        
        public string? ReactMsg { get; set; }

        //recentMessages: LoggerCategoryState[]; // new
        
        public string? History { get; set; }

        public CreateClientLogBrowserInfo Browser { get; set; } 

    }

    public class CreateClientLogError {
        public string Message { get; set; }
        public string Error { get; set; }
    }

    public class CreateClientLogBrowserInfo {
        public string Screen { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string BrowserMajorVersion { get; set; }
        public bool Mobile { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Cookies { get; set; }
        public string UserAgent { get; set; }
    }

    public class CreateClientLogLoggerCategoryState {
        public int Counter { get; set; }
        public string Category { get; set; }
        //category: LoggerType;
        //time: number;
        //expireTime?: number; // set dynamically during display phase, it is just a cache.
        //raw_args: any[];
        //short_string: string;
        //long_string: string;
        //exception?: Error
        //key: string ;
        //toastHidden?: true;
    }
}
