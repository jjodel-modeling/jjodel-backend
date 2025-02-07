using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {
    public class ProjectController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
