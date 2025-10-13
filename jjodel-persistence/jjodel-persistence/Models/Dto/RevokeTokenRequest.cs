using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class RevokeTokenRequest {
       
        [Required]
        public string UserName {  get; set; }

        
    }
}
