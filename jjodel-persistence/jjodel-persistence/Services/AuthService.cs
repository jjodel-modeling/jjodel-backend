using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace jjodel_persistence.Services {
    public class AuthService {

        private readonly Identity _identitySettings;
        private readonly Jwt _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private ApplicationDbContext _context;


        public AuthService(
            IOptions<Identity> identitySettings,
            IOptions<Jwt> jwtSettings,
            ILogger<AuthService> logger
            //ApplicationDbContext context,
            //UserManager<ApplicationUser> userManager

            )
        {
            this._identitySettings = identitySettings.Value;
            this._jwtSettings = jwtSettings.Value;
            this._logger = logger;
            //this._userManager = userManager;
            //this._context = context;

        }

        //#region Users

        //public ApplicationUser GetByUsername(string UserName) {

        //    var result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(u => u.UserName == UserName);
        //    return result;
        //}

        //public ApplicationUser GetByUsernameAsNoTracking(string UserName) {

        //    var result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsNoTracking().FirstOrDefault(u => u.UserName == UserName);
        //    return result;
        //}

        //public ApplicationUser GetById(string Id) {

        //    var result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsNoTracking().FirstOrDefault(u => u.Id == Id);
        //    return result;
        //}

        //#endregion

        public JwtSecurityToken CreateJwtToken(ApplicationUser user, List<string> roles) {

            try {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim("_Id", user._Id != null ? user._Id : "-"));
                foreach(var role in roles) {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddMinutes(System.Convert.ToInt32(this._jwtSettings.ExpiresInMinutes));

                JwtSecurityToken token = new JwtSecurityToken(
                    this._jwtSettings.Issuer,
                    this._jwtSettings.Audience,
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );
                return token;
            }
            catch(Exception ex) {
                this._logger.LogError("Login error: " + ex.Message);
                return null!;
            }

        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token) {
            // Validate jwt token.
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = this._jwtSettings.Issuer,
                ValidAudience = this._jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(this._jwtSettings.SecurityKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public static string GenerateRefreshToken() {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        #region Roles
        //public async Task<List<string>> GetRoles() {
        //    return await this._context.Roles.Select(r => r.Name).ToListAsync();
        //}



        #endregion


        public string GenerateRandomPassword() {
            PasswordOptions opts = new PasswordOptions() {
                RequiredLength = _identitySettings.RequireLength,
                RequireDigit = _identitySettings.RequireDigit,
                RequireLowercase = _identitySettings.RequireLowercase,
                RequireNonAlphanumeric = _identitySettings.RequireNonAlphanumeric,
                RequireUppercase = _identitySettings.RequireUppercase
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength; i++) {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }


    }
}
