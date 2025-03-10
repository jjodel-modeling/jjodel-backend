using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jjodel_persistence.Controllers.Web {
    [Route("project")]
    [Controller]
    public class ProjectController : Controller {

        private readonly ProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProjectController(UserManager<ApplicationUser> userManager, ProjectService projectService, ILogger<ProjectController> logger) {
            this._projectService = projectService;
            this._logger = logger;
            this._userManager = userManager;
        }

        [HttpGet]
        [Route("add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Add() {
            Project project = new Project();
            
            return PartialView("~/Views/Shared/UC_ProjectForm.cshtml", project);
        }

        [HttpGet]
        [Route("delete/{Id:guid}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid Id) {
            try {
                if(Guid.Empty == Id) {
                    return Json(new { Success = false, Message = "Error deleting project." });
                }

                if(await this._projectService.Delete(Id)) {
                    return Json(new { Success = true, Message = "Operation completed successfully." });
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return Json(new { success = false, message = "Error deleting project." });
        }

        [HttpGet]
        [Route("edit/{Id}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Edit(Guid Id) {       
            Project project = await this._projectService.GetById(Id);

            return PartialView("~/Views/Shared/UC_ProjectForm.cshtml", project);
        }
      
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Index() {

            return View();
        }

        [HttpGet]
        [Route("List")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> List() {
            try {
                // recover list of all users.
                List<Project> projects = await this._projectService.GetsAsNoTracking();

                return PartialView("~/Views/Shared/UC_ProjectList.cshtml", projects);
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return PartialView("~/Views/Shared/UC_ProjectList.cshtml", new List<Project>());
        }

        [HttpGet]
        [Route("p-add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> P_Add() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Save")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Save(UpdateProjectRequest project) {
            try {

                if(project.Id == Guid.Empty) {
                    // create

                    project.Id = Guid.NewGuid();
                    if(
                        ModelState.Where(ms =>
                            ms.Value.Errors.Count > 0 &&
                            !ms.ToString().Contains("Id") &&
                            !ms.ToString().Contains("Collaborators") &&
                            !ms.ToString().Contains("Author")
                            ).Count() > 0
                        ) {

                        return Json(new {
                            success = false,
                            message = "Invalid input data",
                            errors = ModelState.Where(ms => ms.Value.Errors.Any(a =>
                            !a.ErrorMessage.Contains("Id") &&
                            !a.ErrorMessage.Contains("Collaborators") &&
                            !a.ErrorMessage.Contains("Author")
                            ))
                               .ToDictionary(
                                   kvp => kvp.Key,
                                   kvp => string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                               )
                        });
                    }
                    Project newProject = new Project() {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        Type = project.Type,
                        State = project.State,
                        Author = await this._userManager.FindByNameAsync(User.Identity.Name),
                        Creation = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        IsFavorite  = false,
                        MetamodelsNumber = 0,
                        ModelsNumber = 0,
                        ViewpointsNumber = 0
                      
                    };

                    if(await this._projectService.Add(newProject)) {
                        return Json(new { Success = true, Message = "Operation completed successfully." });

                    }
                    return Json(new { success = false, message = "Error adding project" });

                }
                else {
                    // edit.
                    if(
                        ModelState.Where(ms =>
                            ms.Value.Errors.Count > 0 &&
                            !ms.ToString().Contains("Collaborators") &&
                            !ms.ToString().Contains("Author")
                            ).Count() > 0
                        ) {

                        return Json(new {
                            success = false,
                            message = "Invalid input data",
                            errors = ModelState.Where(ms => ms.Value.Errors.Any(a =>
                            !a.ErrorMessage.Contains("Collaborators") &&
                            !a.ErrorMessage.Contains("Author")
                            ))
                               .ToDictionary(
                                   kvp => kvp.Key,
                                   kvp => string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                               )
                        });
                    }
                    Project projectFromDB = await this._projectService.GetById(project.Id);

                    projectFromDB.Name = project.Name;
                    projectFromDB.Description = project.Description;
                    projectFromDB.Type = project.Type;
                    projectFromDB.State = project.State;
                    projectFromDB.LastModified = DateTime.UtcNow;

                    if(await this._projectService.Save()) {
                        return Json(new { success = true, message = "Project updated successfully" });
                    }

                    return Json(new { success = false, message = "Error updating project" });

                }

                

            }
            catch(Exception ex) {
                this._logger.LogError("Edit project error: " + ex.Message);
                return Json(new { success = false, message = "Internal server error" });
            }
        }

    }
        
}
