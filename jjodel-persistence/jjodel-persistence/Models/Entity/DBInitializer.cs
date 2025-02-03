
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace jjodel_persistence.Models.Entity {
    public class DbInitializer {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager
            ) {
            this._db = db;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public void Initialize() {
            // Please Note: async methods not working here.


            // create roles
            if(_roleManager.FindByNameAsync("Admin").Result == null) {
                _roleManager.CreateAsync(new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name= "Admin" }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new ApplicationRole() { Id = Guid.NewGuid().ToString(), Name = "User" }).GetAwaiter().GetResult();
            }

            if(_userManager.FindByEmailAsync("andreaperelli.95@gmail.com").Result == null) {
                ApplicationUser admin = new ApplicationUser() {
                    UserName = "andreaperelli.95@gmail.com",
                    Email = "andreaperelli.95@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "3458793739",
                    Name = "Andrea",
                    Surname = "Perelli",
                    
                };

                _userManager.CreateAsync(admin, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(admin, "Admin").GetAwaiter().GetResult();


                ApplicationUser user = new ApplicationUser() {
                    UserName = "a.perelli@capoweb.it",
                    Email = "a.perelli@capoweb.it",
                    EmailConfirmed = true,
                    PhoneNumber = "3458793739",
                    Name = "Andrea",
                    Surname = "Perelli"

                };

                _userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "User").GetAwaiter().GetResult();
            }

        }
    }
}
