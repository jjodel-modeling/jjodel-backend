using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {

    [Route("home")]
    [Controller]
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() {
            
            
            return View();
        }
    }
}
