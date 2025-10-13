using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Mail;
using jjodel_persistence.Models.Settings;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace jjodel_persistence.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly MailService _mailService;
        private readonly AuthService _authService;
        private readonly Jwt _jwtSettings;
        private readonly IConfiguration _configuration;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger,
            MailService mailService,
            AuthService authService,
            IOptions<Jwt> jwtSettings,
            IConfiguration configuration

            ) {

            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._mailService = mailService;
            this._logger = logger;
            this._authService = authService;
            this._jwtSettings = jwtSettings.Value;
            this._configuration = configuration;

        }


        [AllowAnonymous]
        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] ConfirmAccountRequest confirmAccountRequest) {
            try {
                if(ModelState.IsValid) {
                    this._logger.LogInformation("Confirming Account:" + confirmAccountRequest.UserId);

                    ApplicationUser user = await _userManager.FindByIdAsync(confirmAccountRequest.UserId);
                    if(user != null) {
                        confirmAccountRequest.Token = confirmAccountRequest.Token.Replace('§', '/');
                        IdentityResult result = await _userManager.ConfirmEmailAsync(user, confirmAccountRequest.Token);
                        if(result.Succeeded) {
                            this._logger.LogInformation("Confirmed Account:" + confirmAccountRequest.UserId);

                            return Ok();
                        }
                    }
                    this._logger.LogInformation("Confirming Account failed for user: " + confirmAccountRequest.UserId);
                }

                return BadRequest();
            }
            catch(Exception ex) {
                this._logger.LogError("Confirm Account error:" + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest) {
            try {
                if(ModelState.IsValid) {
                    this._logger.LogInformation("Changing Password: " + changePasswordRequest.UserName);

                    var user = await _userManager.FindByNameAsync(changePasswordRequest.UserName);
                    if(user == null || !await _userManager.IsEmailConfirmedAsync(user)) {
                        this._logger.LogInformation("User does not exists or is not confirmed: " + changePasswordRequest.UserName);

                        return BadRequest();
                    }

                    else {
                        var res = await _userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.Password);
                        if(res.Succeeded) {
                            this._logger.LogInformation("Password changed: " + changePasswordRequest.UserName);
                            return Ok();
                        }
                        this._logger.LogInformation("Change Password error: " + changePasswordRequest.UserName);
                    }
                }
                return BadRequest();
            }
            catch(Exception ex) {
                this._logger.LogError("Change Password error:" + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id:guid}")]
        public async Task<IActionResult> Delete(Guid Id) {
            try {
                this._logger.LogInformation("Delete user by id:" + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }

                ApplicationUser user = await _userManager.FindByIdAsync(Id.ToString());
                if(user == null) {
                    return BadRequest();
                }
                
                user.DeletionDate = DateTime.UtcNow;
                user.IsDeleted = true;
                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded) {
                    return Ok();
                }
               
                return BadRequest();
                
            }
            catch(Exception ex) {
                this._logger.LogError("Delete user error:" + ex.Message);
                return BadRequest();
            }
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet("{username}")]
        //public async Task<IActionResult> Get(string userName) {
        //    ApplicationUser result = _authService.GetByUsername(userName);
        //    if(result == null) {
        //        return BadRequest();
        //    }
        //    return Ok(Convert(result));
        //}

        [Authorize(Roles = "Admin, User")]
        [HttpGet("by-id/{Id:guid}")]
        [HttpGet("{Id:guid}")]
        public async Task<IActionResult> Get(Guid Id) {
            try {
                this._logger.LogInformation("Get user by id request:" + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }
                ApplicationUser result = await _userManager.FindByIdAsync(Id.ToString());

                if(result == null) {
                    return BadRequest();
                }
                return Ok(Convert(result));
            }
            catch(Exception ex) {
                this._logger.LogError("Get user by id error:" + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Gets() {
            // gets all aspnetusers not deleted
            try {
                this._logger.LogInformation("Get users request.");

                List<UserResponse> usersResponses = Convert(await _userManager.
                    Users.
                    Where(u => !u.IsDeleted).
                    Include(u => u.ApplicationRoles).
                    AsNoTracking().
                    ToListAsync());
                return Ok(usersResponses);
            }
            catch(Exception ex) {
                this._logger.LogError("Get all user error:" + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers() {
            // gets all aspnetusers
            try {
                this._logger.LogInformation("Get users request.");

                List<UserResponse> usersResponses = Convert(await _userManager.
                    Users.
                    Include(u => u.ApplicationRoles).
                    AsNoTracking().
                    ToListAsync());
                return Ok(usersResponses);
            }
            catch(Exception ex) {
                this._logger.LogError("Get all user error:" + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() {
            // gets all Roles.
            try {
                this._logger.LogInformation("Geting all user roles");

                List<string> roles = await this._roleManager.Roles.Select(r => r.Name).ToListAsync();
                return Ok(roles);
            }
            catch(Exception ex) {
                this._logger.LogError("Get all user roles name error:" + ex.Message);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest) {
            try {
                LoginResponse response = new LoginResponse();

                if(ModelState.IsValid) {

                    _logger.LogInformation("Login request by user: " + loginRequest.Email);

                    ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);

                    if(user == null || user.IsDeleted || !user.EmailConfirmed) {
                        _logger.LogWarning("User: " + loginRequest.Email + " does not exists, is deleted or is not confirmed.");
                        response.Title = "Login failed";
                        if(user== null) {
                            response.Description = "Invalid credentials.";

                        }
                        else {
                            response.Description = "Your account is not ready to use. Please confirm your email via the link we sent.  Or, if already confirmed, reset your password to continue. ";
                        }
                        return BadRequest(response);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);

                    if(!result.Succeeded) {
                        _logger.LogWarning("User failed login: " + loginRequest.Email);
                        response.Title = "Login failed";
                        response.Description = "Invalid credentials.";
                        return BadRequest(response);
                    }
                    _logger.LogInformation("User " + loginRequest.Email + " login successfully");

                    // set last login.
                    user.LastLogin = DateTime.UtcNow;
                    await this._userManager.UpdateAsync(user);

                    var roles = await this._signInManager.UserManager.GetRolesAsync(user);

                    JwtSecurityToken token = this._authService.CreateJwtToken(user, roles.ToList());

                    if(token == null) {
                        _logger.LogWarning("Token creation failed for user: " + loginRequest.Email);
                        return BadRequest();
                    }

                    user.RefreshToken = AuthService.GenerateRefreshToken();
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(this._jwtSettings.RefreshTokenValidityInDays);
                    await _userManager.UpdateAsync(user);

                    return Ok(
                        new TokenResponse() {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expires = token.ValidTo,
                            RefreshToken = user.RefreshToken,
                            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
                        });
                }
                return BadRequest();
            }
            
            catch(Exception ex) {
                this._logger.LogError("Login error: " + ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request) {
            try {
                var principal = this._authService.GetPrincipalFromExpiredToken(request.Token);
                if(principal == null) {
                    return BadRequest();
                }
                string username = principal.Identity.Name;
                ApplicationUser user = await _userManager.FindByNameAsync(username);

                if(user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) {
                    return BadRequest();
                }
                IList<string> roles = await _signInManager.UserManager.GetRolesAsync(user);

                JwtSecurityToken token = this._authService.CreateJwtToken(user, roles.ToList());

                if(token == null) {
                    _logger.LogWarning("Token creation failed for user: " + username);
                    return BadRequest();
                }

                user.RefreshToken = AuthService.GenerateRefreshToken();

                await _userManager.UpdateAsync(user);

                return Ok(new TokenResponse() {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expires = token.ValidTo,
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
                });
            }
            catch(Exception ex) {
                _logger.LogError("Refresh token error: " + ex.Message);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request) {
            RegisterResponse response = new RegisterResponse();
            response.Title = "Registration completed";
            response.Description = "A confirmation email has been sent to your email address. Please confirm your account before logging in.";
            try {
              
                ApplicationUser existingUser = await this._userManager.FindByEmailAsync(request.Email);
                if(ModelState.IsValid &&  (existingUser == null || existingUser.IsDeleted)) {
                    var user = new ApplicationUser {
                        Id = Guid.NewGuid().ToString(),
                        _Id = request._Id,
                        UserName = request.Nickname,
                        Surname = request.Surname,
                        Name = request.Name,
                        Email = request.Email,
                        EmailConfirmed = false,
                        IsDeleted = false,
                        NewsletterEnabled = request.NewsletterEnabled,
                        NewsletterEnableDate = request.NewsletterEnabled ? DateTime.UtcNow : DateTime.MinValue,
                        Country = request.Country,
                        BirthDate = request.BirthDate.HasValue ? request.BirthDate.Value : DateTime.MinValue,
                        Affiliation = !string.IsNullOrEmpty(request.Affiliation) ? request.Affiliation : "",
                        PhoneNumber = request.PhoneNumber,
                        RegistrationDate = DateTime.UtcNow
                    };

                    // create user.
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if(!result.Succeeded) {
                        this._logger.LogWarning("Registration process failed for user " + request.Email + ": " + string.Join(";", result.Errors.Select(e => "Code: " + e.Code + " Description" + e.Description)));

                        response.Title = "Registration process failed";
                        response.Description = "The following fields are invalid: " + string.Join(" ", result.Errors.Select(e => e.Description));

                        return BadRequest(response);
                    }
                    _logger.LogInformation("User " + request.Email + " was registered");
                    // assign user role.
                    await _userManager.AddToRolesAsync(user, new List<string> { "User" });

                    // generate confirm token.
                    string confirmTokenWithSlashes = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmToken = confirmTokenWithSlashes.Replace('/', '§');
                    // send mail.
                    await _mailService.SendEmail(
                        new List<string> { request.Email },
                        "Confirm Account", "ConfirmAccount",
                        new ConfirmAccount() {
                            Name = request.Name,
                            Surname = request.Surname,
                            Token = confirmToken,
                            Id = user.Id,
                            Url = this._configuration["FrontendEndpoint"]
                        });
                    return Ok(result);
                }
                else {
                    response.Title = "Registration process failed";
                    var errorFields = ModelState.Where(ms => ms.Value.Errors.Count > 0).Select(ms => ms.Key);
                    response.Description  = "The following fields are invalid: " + string.Join(", ", errorFields) + ".";

                    if(existingUser != null && !existingUser.IsDeleted) {
                        this._logger.LogWarning("Registration process failed: user " + request.Email + " already exists.");
                        response.Description = "An account with this email address already exists.";
                        return BadRequest(response);
                    }
                   
                    return BadRequest(response);
                }
            }
            catch(Exception ex) {
                this._logger.LogError("Register error: " + ex.Message);
                response.Title = "Registration error";
                response.Description = "An error occurred during registration. Please try again later.";
                return BadRequest(response);
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest) {
            try {
                this._logger.LogInformation("Richiesto reset password");
                if (ModelState.IsValid) {
                    var user = await _userManager.FindByNameAsync(resetPasswordRequest.Nickname);
                    if (user == null) {
                        _logger.LogInformation("User " + resetPasswordRequest.Nickname + " not found");
                        return BadRequest();
                    }
                    // generate password.
                    string password = _authService.GenerateRandomPassword();
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, password);

                    if (!result.Succeeded) {

                        this._logger.LogWarning("Password reset failed for user " + user.UserName);
                        return BadRequest();
                    }
                        await _mailService.SendEmail(new List<string> { user.Email }, "Reset Password", "ResetPassword", new ResetPassword() { NewPassoword = password, Username = resetPasswordRequest.Nickname });
                        this._logger.LogInformation("The password has been reset");
                        return Ok();                    
                }

                return BadRequest();
            }
            catch (Exception ex) {
                this._logger.LogError("Reset error: " + ex.Message);
                return BadRequest();
            }
        } 
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPasswordWithEmail([FromBody] ResetPasswordRequestWithEmail retrivePasswordRequest) {
            try {
                this._logger.LogInformation("Richiesto reset password");
                if (ModelState.IsValid) {
                    var user = await _userManager.FindByEmailAsync(retrivePasswordRequest.Email);
                    
                    if (user == null) {
                        _logger.LogInformation("User " + retrivePasswordRequest.Email + " not found");
                        return BadRequest();
                    }
                    // generate password.
                    string password = _authService.GenerateRandomPassword();
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, password);

                    if (!result.Succeeded) {

                        this._logger.LogWarning("Password reset failed for user " + user.UserName);
                        return BadRequest();
                    }
                    await _mailService.SendEmail(new List<string> { user.Email }, "Reset Password", "ResetPassword", new ResetPassword() { NewPassoword = password, Username = user.UserName });
                    this._logger.LogInformation("The password has been reset");
                    return Ok();

                }

                return BadRequest();
            }
            catch (Exception ex) {
                this._logger.LogError("Reset error: " + ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request) {
            try {
                ApplicationUser user = await _userManager.FindByNameAsync(request.UserName);
                if(user == null) {
                    return BadRequest();
                }

                user.RefreshToken = null;
                await this._userManager.UpdateAsync(user);

                return Ok();
            }
            catch(Exception ex) {
                _logger.LogError($"Revoke token error for user {request.UserName}: " + ex.Message);
                return BadRequest();
            }
        }


        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest updateUserRequest) {

            try {
                if(ModelState.IsValid) {
                    this._logger.LogInformation("Edit user request:" + updateUserRequest.Id);

                    ApplicationUser user = await _userManager.FindByIdAsync(updateUserRequest.Id);
                    if(user == null || user.IsDeleted) {
                        _logger.LogWarning("User: " + updateUserRequest.Id + " does not exists or is deleted.");

                        return BadRequest();
                    }

                    // unique email verification  
                    var existingUser = await this._userManager.FindByEmailAsync(updateUserRequest.Email);
                    if(existingUser != null && existingUser.Id != updateUserRequest.Id) {
                        this._logger.LogWarning(updateUserRequest.Id + " is attempting to update the email of " + existingUser.Id);
                        return BadRequest();

                    }

                    // update fields.
                    user._Id = updateUserRequest._Id;
                    user.Surname = updateUserRequest.Surname;
                    user.Name = updateUserRequest.Name;
                    user.UserName = updateUserRequest.Nickname;
                    user.Affiliation = updateUserRequest.Affiliation;
                    user.Country = updateUserRequest.Country;
                    user.PhoneNumber = updateUserRequest.PhoneNumber;
                    user.Email = updateUserRequest.Email;
                    user.PhoneNumber = updateUserRequest.PhoneNumber;
                    user.BirthDate = updateUserRequest.BirthDate;
                    if(user.NewsletterEnabled != updateUserRequest.Newsletter ) {
                        user.NewsletterEnabled = updateUserRequest.Newsletter;
                        if(updateUserRequest.Newsletter) {
                            user.NewsletterEnableDate = DateTime.UtcNow;
                        }
                    }

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded) {                        
                        return Ok();
                    }
                }
                return BadRequest();
            }
            catch(Exception ex) {
                this._logger.LogError("Edit User error: " + ex.Message);
                return BadRequest();
            }
        }


        #region Convert

        private static UserResponse Convert(ApplicationUser user) {
            UserResponse u = new UserResponse() {
                Id = user.Id,
                _Id = user._Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.UserName,
                Affiliation = user.Affiliation,
                BirthDate = user.BirthDate,
                Country = user.Country,
                Newsletter = user.NewsletterEnabled,
                PhoneNumber = user.PhoneNumber,
            };
            return u;
        }

        private static List<UserResponse> Convert(List<ApplicationUser> users) {
            List<UserResponse> result = new List<UserResponse>();
            foreach(ApplicationUser user in users) {
                result.Add(Convert(user));
            }
            return result;
        }

        #endregion
    }

}
