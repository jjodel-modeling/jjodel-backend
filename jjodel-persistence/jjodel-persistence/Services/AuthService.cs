using jjodel_persistence.Models.Dto;
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
        private readonly ApplicationDbContext _context;


        public AuthService(
            IOptions<Identity> identitySettings,
            IOptions<Jwt> jwtSettings,
            ILogger<AuthService> logger,
            ApplicationDbContext context
            //UserManager<ApplicationUser> userManager

            )
        {
            this._identitySettings = identitySettings.Value;
            this._jwtSettings = jwtSettings.Value;
            this._logger = logger;
            //this._userManager = userManager;
            this._context = context;

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

        #region Dashboard

        // user statistics for the backoffice dashboard. fromMonth = first day (UTC) of the 12-month window.
        public async Task<UserStatsResponse> GetUserStats(DateTime fromMonth) {
            UserStatsResponse res = new UserStatsResponse();

            DateTime now = DateTime.UtcNow;
            DateTime last30Days = now.AddDays(-30);
            DateTime last7Days = now.AddDays(-7);

            IQueryable<ApplicationUser> users = this._context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted);

            // counters.
            res.Total = await users.CountAsync();
            res.NewLast30Days = await users.CountAsync(u => u.RegistrationDate >= last30Days);
            res.NewLast7Days = await users.CountAsync(u => u.RegistrationDate >= last7Days);

            // new users per month (grouped in SQL).
            var perMonth = await users
                .Where(u => u.RegistrationDate >= fromMonth)
                .GroupBy(u => new { u.RegistrationDate!.Value.Year, u.RegistrationDate.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            res.NewPerMonth = Enumerable.Range(0, 12)
                .Select(i => fromMonth.AddMonths(i))
                .Select(m => perMonth.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0)
                .ToArray();

            // users by last access (only the LastLogin column is read).
            List<DateTime?> lastLogins = await users.Select(u => u.LastLogin).ToListAsync();
            int[] buckets = new int[6];
            foreach(DateTime? lastLogin in lastLogins) {
                if(lastLogin == null) {
                    buckets[5]++;
                    continue;
                }

                double days = (now - lastLogin.Value).TotalDays;
                if(days < 7) buckets[0]++;
                else if(days < 30) buckets[1]++;
                else if(days < 90) buckets[2]++;
                else if(days < 365) buckets[3]++;
                else buckets[4]++;
            }
            res.LastAccessBuckets = buckets;
            res.ActiveLast30Days = buckets[0] + buckets[1];

            // affiliation and country are free text: normalized (trim + lowercase) before grouping.
            var affiliations = await users
                .GroupBy(u => (u.Affiliation ?? "").Trim().ToLower())
                .Select(g => new { g.Key, Label = g.Max(u => u.Affiliation), Count = g.Count() })
                .ToListAsync();
            res.ByAffiliation = TopWithOther(affiliations.Select(a => (a.Key, a.Label, a.Count)), 5, "Not specified");

            var countries = await users
                .GroupBy(u => (u.Country ?? "").Trim().ToLower())
                .Select(g => new { g.Key, Label = g.Max(u => u.Country), Count = g.Count() })
                .ToListAsync();
            res.ByCountry = TopWithOther(countries.Select(c => (c.Key, c.Label, c.Count)), 5, "Not specified");

            return res;
        }

        // the first "top" groups by count, then "Other" and the not specified ones.
        private static List<KeyValuePair<string, int>> TopWithOther(
            IEnumerable<(string Key, string? Label, int Count)> groups, int top, string notSpecifiedLabel) {

            List<(string Key, string? Label, int Count)> list = groups.ToList();
            int notSpecified = list.Where(g => string.IsNullOrEmpty(g.Key)).Sum(g => g.Count);
            List<(string Key, string? Label, int Count)> specified = list
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .OrderByDescending(g => g.Count)
                .ToList();

            List<KeyValuePair<string, int>> res = specified
                .Take(top)
                .Select(g => new KeyValuePair<string, int>((g.Label ?? g.Key).Trim(), g.Count))
                .ToList();

            int other = specified.Skip(top).Sum(g => g.Count);
            if(other > 0) {
                res.Add(new KeyValuePair<string, int>("Other", other));
            }
            if(notSpecified > 0) {
                res.Add(new KeyValuePair<string, int>(notSpecifiedLabel, notSpecified));
            }
            return res;
        }

        #endregion


    }
}
