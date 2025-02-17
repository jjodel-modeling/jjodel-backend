using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jjodel_persistence.Controllers.Web {

    [Route("account")]
    [Controller]
    public class AccountController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;

        }

        [HttpGet]
        [Route("edit/{Id}")]
        public async Task<ActionResult> Edit(string Id) {

            return PartialView("~/Views/Shared/UC_UserForm.cshtml", await _userManager.FindByIdAsync(Id));
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            // Recupera tutti gli utenti dal database
            List<ApplicationUser> users = await _userManager.
                    Users.
                    Where(u => !u.IsDeleted).
                    Include(u => u.ApplicationRoles).
                    Include(u => u.Author).
                    AsNoTracking().
                    ToListAsync();



            return View(users);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Update(UpdateUserRequest updateUserRequest) {
            try {
                if (!ModelState.IsValid) {
                    return Json(new { success = false, message = "Invalid input data" });
                }


                ApplicationUser user = await _userManager.FindByIdAsync(updateUserRequest.Id);
                if (user == null || user.IsDeleted) {
                    return Json(new { success = false, message = "User not found" });
                }

                // Unique email verification
                var existingUser = await _userManager.FindByEmailAsync(updateUserRequest.Email);
                if (existingUser != null && existingUser.Id != updateUserRequest.Id) {
                    return Json(new { success = false, message = "Email already in use" });
                }

                // Update user fields
                user.Name = updateUserRequest.Name;
                user.Surname = updateUserRequest.Surname;
                user.UserName = updateUserRequest.Nickname;
                user.Affiliation = updateUserRequest.Affiliation;
                user.Country = updateUserRequest.Country;
                user.PhoneNumber = updateUserRequest.PhoneNumber;
                user.Email = updateUserRequest.Email;
                user.BirthDate = DateTime.SpecifyKind(updateUserRequest.BirthDate, DateTimeKind.Utc);

                if (user.NewsletterEnabled != updateUserRequest.Newsletter) {
                    user.NewsletterEnabled = updateUserRequest.Newsletter;
                    if (updateUserRequest.Newsletter) {
                        user.NewsletterEnableDate = DateTime.UtcNow;
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded) {
                    return Json(new { success = true, message = "User updated successfully" });
                }

                return Json(new { success = false, message = "Error updating user" });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = "Internal server error" });
            }
        }
    }
 }



