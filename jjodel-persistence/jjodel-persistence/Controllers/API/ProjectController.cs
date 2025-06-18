using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace jjodel_persistence.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase {

        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(
            ILogger<ProjectController> logger,
            ProjectService projectService,
            UserManager<ApplicationUser> userManager
            ) {
        
            this._logger = logger;
            this._projectService = projectService;
            this._userManager = userManager;
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProjectRequest createProjectRequest) {

            try {
                this._logger.LogInformation("Add Project request ");
                if(ModelState.IsValid) {
                    Project project = new Project() {
                        Id = Guid.NewGuid(),
                        _Id = createProjectRequest._Id != null ? createProjectRequest._Id : "",
                        Name = createProjectRequest.Name,
                        Description = createProjectRequest.Description,
                        Type = createProjectRequest.Type,
                        Creation = createProjectRequest.Imported ? createProjectRequest.Creation!.Value : DateTime.UtcNow,
                        LastModified = createProjectRequest.Imported ? createProjectRequest.LastModified!.Value : DateTime.UtcNow,
                        State = createProjectRequest.State != null ? createProjectRequest.State : "",
                        Author = await this._userManager.FindByNameAsync(User.Identity.Name),
                        Imported = createProjectRequest.Imported,
                        Version = createProjectRequest.Version,
                        ViewpointsNumber = createProjectRequest.ViewpointsNumber,
                        MetamodelsNumber = createProjectRequest.MetamodelsNumber,
                        ModelsNumber = createProjectRequest.ViewpointsNumber,
                        IsFavorite = createProjectRequest.IsFavorite,
                        
                    };
                    if(await this._projectService.Add(project)) {
                        return Ok(Convert(project));
                    }
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex.Message);
            }
            return BadRequest();

        }

        [Authorize(Roles = "User")]
        [HttpDelete("{Id:guid}")]
        public async Task<IActionResult> Delete(Guid Id) {
            try {
                this._logger.LogInformation("Delete project by id: " + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }

                if(await this._projectService.Delete(Id)) {
                    return Ok();
                }
                
            }
            catch(Exception ex) {
                this._logger.LogError("Delete project error: " + ex.Message);
            }
            return BadRequest();

        }

        [Authorize(Roles = "User")]
        [HttpGet("{Id:guid}")]
        public async Task<IActionResult> GetById(Guid Id) {
            // gets all project.
            try {
                this._logger.LogInformation("Get project by id request:" + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }
                // todo check permission to open project (public/private)
                Project result = await this._projectService.GetById(Id);

                if(result == null) {
                    return BadRequest();
                }
                return Ok(Convert(result));
            }
            catch(Exception ex) {
                this._logger.LogError("Get project by id: " + ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet("jjodel/{Id}")]
        public async Task<IActionResult> GetByJJodelId(string Id) {
            // gets all project.
            try {
                this._logger.LogInformation("Get project by jjodel id request:" + Id);

                if(string.IsNullOrWhiteSpace(Id)) {
                    return BadRequest();
                }
                // todo check permission to open project (public/private)
                Project result = await this._projectService.GetByJJodelId(Id);

                if(result == null) {
                    return BadRequest();
                }
                return Ok(Convert(result));
            }
            catch(Exception ex) {
                this._logger.LogError("Get project by jjodel id: " + ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Gets() {
            // gets all project.
            try {
                this._logger.LogInformation("Get projects request.");
                List<Project> projects = await this._projectService.GetByAuthor(User.Identity.Name);

                return Ok(Convert(projects));
            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() {
            // gets all project.
            try {
                this._logger.LogInformation("Get all projects request.");

                return Ok(Convert(await this._projectService.GetsAsNoTracking()));
            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProjectRequest updateProjectRequest) {
            try {
                if(ModelState.IsValid) {
                    this._logger.LogInformation("Edit user request:" + updateProjectRequest.Id);

                    Project projectToUpdate = await this._projectService.GetById(updateProjectRequest.Id);
                    if(projectToUpdate != null) {
                        projectToUpdate._Id = updateProjectRequest._Id != null ? updateProjectRequest._Id : "";
                        projectToUpdate.Name = updateProjectRequest.Name;
                        projectToUpdate.Description = updateProjectRequest.Description;
                        projectToUpdate.State = updateProjectRequest.State;
                        projectToUpdate.Type = updateProjectRequest.Type;
                        projectToUpdate.ViewpointsNumber = updateProjectRequest.ViewpointsNumber;
                        projectToUpdate.MetamodelsNumber = updateProjectRequest.MetamodelsNumber;
                        projectToUpdate.ModelsNumber = updateProjectRequest.ModelsNumber;
                        projectToUpdate.LastModified = updateProjectRequest.LastModified;
                        projectToUpdate.IsFavorite = updateProjectRequest.IsFavorite;

                        List<ApplicationUser> users = this._userManager.Users.Where(u => updateProjectRequest.Collaborators.Contains(u.UserName)).ToList();

                        projectToUpdate.Collaborators = users;

                        if(await this._projectService.Save()) {
                            return Ok();
                        }
                    }
                }                
            }
            catch(Exception ex) {
                this._logger.LogError("Edit project error: " + ex.Message);
            }
            return BadRequest();

        }

        #region Convert

        public static ProjectResponse Convert(Project p) {
            ProjectResponse response = new ProjectResponse() {
                Id = p.Id,
                _Id = p._Id,
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                State = p.State,
                ViewpointsNumber = p.ViewpointsNumber,
                MetamodelsNumber = p.MetamodelsNumber,
                ModelsNumber = p.ModelsNumber,
                Creation = p.Creation,
                LastModified = p.LastModified,
                IsFavorite = p.IsFavorite,
                Author = (p.Author != null) ? p.Author.UserName : "",
                Collaborators = (p.Collaborators != null) ? p.Collaborators.Select(c=> c.UserName).ToList() : new List<string?>(),

            };
            return response;
        }

        private static List<ProjectResponse> Convert(List<Project> projects) {
            List<ProjectResponse> result = new List<ProjectResponse>();
            foreach(Project project in projects) {
                result.Add(Convert(project));
            }
            return result;
        }

        #endregion
    }
}
