using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace jjodel_persistence.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase {

        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TagService _tagService;
        private readonly IMemoryCache _cache; // cache

        public ProjectController(
            ILogger<ProjectController> logger,
            ProjectService projectService,
            UserManager<ApplicationUser> userManager,
            TagService tagService,
            IMemoryCache cache
            ) {
        
            this._logger = logger;
            this._projectService = projectService;
            this._userManager = userManager;
            this._tagService = tagService;
            this._cache = cache;
        }

        [Authorize(Roles ="User")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateProjectRequest createProjectRequest) {

            try {
                this._logger.LogInformation("Add Project request ");
                

                if (ModelState.IsValid) {
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
                        Tags = (createProjectRequest.TagNames is not null && createProjectRequest.TagNames.Count > 0) ? await this._tagService.GetsByNames(createProjectRequest.TagNames) : new List<Tag>()
                    };
                    
                    if(await this._projectService.Add(project)) {
                        return Ok(Convert(project));
                    }
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return BadRequest();

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("templates")]
        public async Task<IActionResult> Add([FromBody] CreateProjectTemplateRequest createProjectTemplateRequest) {

            try {
                this._logger.LogInformation("Add Project Template request ");
                if(ModelState.IsValid) {
                    ProjectTemplate projectTemplate = new ProjectTemplate() {
                        Id = Guid.NewGuid(),
                        Name = createProjectTemplateRequest.Name,
                        Description = createProjectTemplateRequest.Description,
                        Creation = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        State = createProjectTemplateRequest.State != null ? createProjectTemplateRequest.State : "",
                        Version = createProjectTemplateRequest.Version,
                        ViewpointsNumber = createProjectTemplateRequest.ViewpointsNumber,
                        MetamodelsNumber = createProjectTemplateRequest.MetamodelsNumber,
                        ModelsNumber = createProjectTemplateRequest.ViewpointsNumber,
                        
                    };
                    if(await this._projectService.AddProjectTemplate(projectTemplate)) {
                        return Ok(Convert(projectTemplate));
                    }
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
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
        [HttpGet("jjodel/{Id:minlength(1)}")]
        public async Task<IActionResult> GetByJJodelId(string Id) {
            // gets all project.
            try {
                this._logger.LogInformation("Get project by jjodel id request:" + Id);

                if(string.IsNullOrWhiteSpace(Id)) {
                    return BadRequest();
                }
                // todo check permission to open project (public/private)
                Project result = await this._projectService.GetByJJodelId(Id);

                //todo: remove
                //result.Collaborators.Add(await this._userManager.FindByNameAsync(this.User.Identity.Name));
                //await this._projectService.Save();


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

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> Gets() {
            // gets all project withot state property.
            try {
                this._logger.LogInformation("Get projects request.");
                List<Project> projects = await this._projectService.GetByAuthor(User.Identity.Name);

                return Ok(ConvertShort(projects));
            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet("full")]
        public async Task<IActionResult> GetsFull() {
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

        [Authorize(Roles = "Admin, User")]
        [HttpGet("get-templates")]
        public async Task<IActionResult> GetTemplates() {
            // gets all project template.
            try {
                this._logger.LogInformation("Get all project templates");

                List<ProjectTemplate> templates = await _cache.GetOrCreateAsync("project_template", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                    return await this._projectService.GetTemplatesAsNoTracking();
                });

                return Ok(Convert(templates));

            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("get-templates/pagination/{Skip:int}/{Take:int}")]
        public async Task<IActionResult> GetTemplatesWithPagination(int Skip, int Take) {
            // gets all project template.
            try {
                this._logger.LogInformation("Get all project templates with pagination");

                List<ProjectTemplate> templates = await _cache.GetOrCreateAsync($"project_template_{Skip}_{Take}", async entry => {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                    return await this._projectService.GetTemplatesAsNoTrackingWithPagination(Skip, Take);
                });

                return Ok(Convert(templates));
            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }

        [HttpGet("templates/{Id:guid}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetTemplateById(Guid Id) {
            // get project template by id.
            try {
                this._logger.LogInformation("Get project template by id" + Id);


                if(Guid.Empty == Id) {
                    return BadRequest();
                }

                ProjectTemplate template = await this._projectService.GetTemplateById(Id);

                return Ok(Convert(template));

            }
            catch(Exception ex) {
                this._logger.LogError(ex.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet("search-by-tag/{TagName:minlength(1)}")]
        public async Task<IActionResult> SearchByTags(string TagName) {
            try {
                this._logger.LogInformation("Search projects by tags request: " + TagName);

                List<Project> projects = await this._projectService.GetsAsNoTrackingByTagName(TagName.Trim());


                return Ok(ConvertShort(projects));
            }
            catch(Exception ex) {
                this._logger.LogError("Search By Tag error: " + ex.ToString());
                return BadRequest();
            }
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProjectRequest updateProjectRequest) {
            try {
                if(ModelState.IsValid) {
                    this._logger.LogInformation("Edit user request: " + updateProjectRequest.Id);

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

                        if(updateProjectRequest.TagNames is not null && updateProjectRequest.TagNames.Count > 0) {
                            projectToUpdate.Tags = await this._tagService.GetsByNames(updateProjectRequest.TagNames);

                        }

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
                Tags = (p.Tags != null) ? p.Tags.Select(c => c.Name).ToList() : new List<string?>(),


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

        public static ProjectShortResponse ConvertShort(Project p) {
            ProjectShortResponse response = new ProjectShortResponse() {
                Id = p.Id,
                _Id = p._Id,
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                ViewpointsNumber = p.ViewpointsNumber,
                MetamodelsNumber = p.MetamodelsNumber,
                ModelsNumber = p.ModelsNumber,
                Creation = p.Creation,
                LastModified = p.LastModified,
                IsFavorite = p.IsFavorite,
                Author = (p.Author != null) ? p.Author.UserName : "",
                Collaborators = (p.Collaborators != null) ? p.Collaborators.Select(c => c.UserName).ToList() : new List<string?>(),
                Tags = (p.Tags != null) ? p.Tags.Select(c => c.Name).ToList() : new List<string?>(),

            };
            return response;

        }

        private static List<ProjectShortResponse> ConvertShort(List<Project> projects) {
            List<ProjectShortResponse> result = new List<ProjectShortResponse>();
            foreach(Project project in projects) {
                result.Add(ConvertShort(project));
            }
            return result;
        }

        public static ProjectTemplateResponse Convert(ProjectTemplate p) {
            ProjectTemplateResponse response = new ProjectTemplateResponse() {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                State = p.State,
                Creation = p.Creation,
                LastModified = p.LastModified,
                Version = p.Version,
                ViewpointsNumber = p.ViewpointsNumber,
                MetamodelsNumber = p.MetamodelsNumber,
                ModelsNumber = p.ModelsNumber,
            };
            return response;
        }

        private static List<ProjectTemplateResponse> Convert(List<ProjectTemplate> projects) {
            List<ProjectTemplateResponse> result = new List<ProjectTemplateResponse>();
            foreach(ProjectTemplate project in projects) {
                result.Add(Convert(project));
            }
            return result;
        }

        #endregion
    }
}
