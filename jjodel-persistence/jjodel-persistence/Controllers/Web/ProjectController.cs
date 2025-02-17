using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {
    [Route("project")]
    [Controller]
    public class ProjectController : Controller {

        private readonly ProjectService _projectService;
      
        public ProjectController(ProjectService projectService) {
            _projectService = projectService;
        }
        [HttpGet]
        public async Task<IActionResult> Index() {

            List<Project> projects = await this._projectService.Gets();

        
            return View(projects);
        }
    }
}
