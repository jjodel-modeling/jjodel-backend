﻿using System.ComponentModel.DataAnnotations;

namespace jjodel_persistence.Models.Dto {
    public class TokenResponse {
        [Required]
        public string Token { get; set; }
        public DateTime Expires { get; set; }

        [Required]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
