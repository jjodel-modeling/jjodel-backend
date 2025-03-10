using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Settings;

namespace jjodel_persistence.Services {
    public class AuthService {

        private readonly Identity _identitySettings;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private ApplicationDbContext _context;


        public AuthService(
            IOptions<Identity> identitySettings
            //ApplicationDbContext context,
            //UserManager<ApplicationUser> userManager

            )
        {
            this._identitySettings = identitySettings.Value;
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


        #region Roles
        //public async Task<List<string>> GetRoles() {
        //    return await this._context.Roles.Select(r => r.Name).ToListAsync();
        //}

        

        #endregion


        public string GenerateRandomPassword() {
            PasswordOptions opts = new PasswordOptions() {
                RequiredLength = _identitySettings.RequiredLength,
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
