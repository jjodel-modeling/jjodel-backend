using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {

    [Route("")]
    [Route("home")]
    [Controller]
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ProjectService _projectService;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes =CookieAuthenticationDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpGet]
        public IActionResult Index() {
            try {

            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }

            return View();
        }

         
    }
}
