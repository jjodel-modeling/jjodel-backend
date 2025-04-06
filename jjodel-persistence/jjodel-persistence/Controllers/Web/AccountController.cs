using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Settings;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using jjodel_persistence.Models.Mail;



namespace jjodel_persistence.Controllers.Web {

    [Route("account")]
    [Controller]
    public class AccountController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly MailService _mailService;
        private readonly AuthService _authService;
        private readonly Jwt _jwtSettings;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger,
            MailService mailService, 
            AuthService authService, 
            IOptions<Jwt> jwtSettings
            ) {

            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._logger = logger;
            this._mailService = mailService;
            this._authService = authService;
            this._jwtSettings = jwtSettings.Value;
        }

        [HttpGet]
        [Route("Add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Add() {
            ApplicationUser user = new ApplicationUser();
            user.Id = null!;
            try {            

                ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();

            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return PartialView("~/Views/Shared/UC_UserForm.cshtml", user);
        }

        [HttpGet]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin,User")]
        public async Task<ActionResult> ChangePassword() {
            return View();
        }

        [HttpGet]
        [Route("Delete/{Id:guid}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid Id) {
            try {
                if(Guid.Empty == Id) {
                    return Json(new { Success = false, Message = "Error deleting user." });
                }

                ApplicationUser user = await _userManager.FindByIdAsync(Id.ToString());
                if(user == null) {
                    return Json(new { Success = false, Message = "Error deleting user." });
                }

                user.DeletionDate = DateTime.UtcNow;
                user.IsDeleted = true;
                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded) {
                    return Json(new { Success = true, Message = "Operation completed successfully." });
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return Json(new { Success = false, Message = "Error deleting user." });
        }

        [HttpGet]
        [Route("Edit/{Id}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Edit(string Id) {
            ApplicationUser user = new ApplicationUser();
            try {
                user = await this._userManager
                            .Users
                                .Include(u => u.ApplicationRoles)
                                .ThenInclude(ur => ur.Role)
                                .Include(u => u.Collaborators)
                                .Include(u => u.Author) // 
                                .FirstOrDefaultAsync(u => u.Id == Id);

                ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();

            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return PartialView("~/Views/Shared/UC_UserForm.cshtml", user);
        }

        [HttpGet]
        [Route("List")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> List() {
            try {
                // recover list of all users.
                List<ApplicationUser> users = await _userManager.
                        Users.
                        Where(u => !u.IsDeleted).
                        Include(u => u.ApplicationRoles).
                        Include(u => u.Author).
                        Include(u => u.Collaborators).
                        AsNoTracking().
                        ToListAsync();

                return PartialView("~/Views/Shared/UC_UserList.cshtml", users);
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return PartialView("~/Views/Shared/UC_UserList.cshtml", new List<ApplicationUser>());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Login")]
        public async Task<ActionResult> Login() {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest) {
            try {
                // validation model
                if(!ModelState.IsValid) {
                    return View(loginRequest);
                }
                ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);

                if(user == null || user.IsDeleted) {
                    return View(loginRequest);
                }

                var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);


                var roles = await _signInManager.UserManager.GetRolesAsync(user);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                foreach(var role in roles) {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return View(loginRequest);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult> Logout() {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Index() {
            return View();
        }

        [HttpPost]
        [Route("Save")]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes=CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Save(ApplicationUser user, IFormCollection form) {
            try {
                if(user.Id == null) {
                    // create
                    user.Id = Guid.NewGuid().ToString();
                    user.IsDeleted = false;
                    user.EmailConfirmed = false;
                    user.NewsletterEnabled = false;
                    user.NewsletterEnableDate = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);                    
                    user.DeletionDate = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                    user.BirthDate = DateTime.SpecifyKind(user.BirthDate, DateTimeKind.Utc);

                    if(ModelState.Where(ms =>
                            ms.Value.Errors.Count > 0 &&
                            !ms.ToString().Contains("Id") &&
                            !ms.ToString().Contains("Role") &&
                            !ms.ToString().Contains("Collaborators") &&
                            !ms.ToString().Contains("Author")
                            ).Count() > 0) {
                        return Json(new {
                            success = false,
                            message = "Invalid input data",
                            errors = ModelState.Where(ms => ms.Value.Errors.Any(a =>
                            !a.ErrorMessage.Contains("Id") &&
                            !a.ErrorMessage.Contains("Role") &&
                            !a.ErrorMessage.Contains("Collaborators") &&
                            !a.ErrorMessage.Contains("Author")
                            ))
                               .ToDictionary(
                                   kvp => kvp.Key,
                                   kvp => string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                               )
                        });
                    }
                    // set random password
                    string randomPassword = this._authService.GenerateRandomPassword();

                    // create user.
                    var result = await _userManager.CreateAsync(user, randomPassword);
                    if(!result.Succeeded) {
                        _logger.LogWarning("Add user process failed: " + string.Join(";", result.Errors.Select(e => "Code: " + e.Code + " Description" + e.Description)));
                        return Json(new { success = false, message = "Internal server error" });
                    }
                    _logger.LogInformation("User " + user.Email + " was registered");
                    // assign user role.

                    List<ApplicationRole> applicationRoles = await this._roleManager.Roles.ToListAsync();
                    List<string> roles = new List<string>();
                    //todo: improve
                    foreach(string key in form.Keys.Where(u => u.Contains("Roles"))) {
                        roles.Add(applicationRoles.First(r => r.Id == form[key]).Name);                               
                    }                   

                    await _userManager.AddToRolesAsync(user, roles);


                    // generate confirm token.
                    string confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // send mail.
                    await _mailService.SendEmail(
                        new List<string> { user.Email },
                        "Confirm Account", "ConfirmAccountWithPassword",
                    new ConfirmAccountWithPassword() {
                            Name = user.Name,
                            Surname = user.Surname,
                            Token = confirmToken,
                            Password = randomPassword
                        });
                    return Json(new { success = true, message ="Operation completed successfully." });

                }
                else {
                    // edit

                    if(ModelState.Where(ms =>
                            ms.Value.Errors.Count > 0 &&
                            !ms.ToString().Contains("Role") &&
                            !ms.ToString().Contains("Collaborators") &&
                            !ms.ToString().Contains("Author")
                            ).Count() > 0) {
                        return Json(new {
                            success = false,
                            message = "Invalid input data",
                            errors = ModelState.Where(ms => ms.Value.Errors.Any(a =>
                            !a.ErrorMessage.Contains("Role") &&
                            !a.ErrorMessage.Contains("Collaborators") &&
                            !a.ErrorMessage.Contains("Author")
                            ))
                               .ToDictionary(
                                   kvp => kvp.Key,
                                   kvp => string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                               )
                        });
                    }

                    ApplicationUser existingUser = await this._userManager.
                        Users.
                        Include(u => u.ApplicationRoles).
                        FirstOrDefaultAsync(u => u.Id == user.Id);

                    if(existingUser == null || existingUser.IsDeleted) {
                        return Json(new { success = false, message = "User not found" });
                    }

                    existingUser.Name = user.Name;
                    existingUser.Surname = user.Surname;
                    existingUser.UserName = user.UserName;
                    existingUser.Affiliation = user.Affiliation;
                    existingUser.Country = user.Country;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Email = user.Email;
                    existingUser.BirthDate = DateTime.SpecifyKind(user.BirthDate, DateTimeKind.Utc);

                    // clear roles.
                    existingUser.ApplicationRoles.Clear();
                    // add new roles.
                    foreach(string key in form.Keys.Where(u => u.Contains("Roles"))) {

                        existingUser.ApplicationRoles.Add(
                            new ApplicationUserRole() {
                                RoleId = form[key],
                                UserId = existingUser.Id,
                            });
                    }

                    var result = await _userManager.UpdateAsync(existingUser);
                    if(result.Succeeded) {
                        return Json(new { success = true, message = "User updated successfully" });
                    }

                    return Json(new { success = false, message = "Error updating user" });
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return Json(new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet]
        [Route("p-add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> P_Add() {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword() {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest) {
            try {
                // validation model
                if(!ModelState.IsValid) {
                    return View(resetPasswordRequest);
                }

                var user = await _userManager.FindByNameAsync(resetPasswordRequest.Nickname);
                if(user == null) {
                    _logger.LogInformation("User " + resetPasswordRequest.Nickname + " not found");
                    return BadRequest();
                }

                // generate password.
                string password = _authService.GenerateRandomPassword();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, password);

                if(!result.Succeeded) {
                    this._logger.LogWarning("Password reset failed for user " + user.UserName);
                    return BadRequest();
                }
                await _mailService.SendEmail(new List<string> { user.Email }, "Reset Password", "ResetPassword", new ResetPassword() { NewPassoword = password, Username = resetPasswordRequest.Nickname });
                this._logger.LogInformation("The password has been reset");
                return Ok();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return View();
        }


        
    }
}