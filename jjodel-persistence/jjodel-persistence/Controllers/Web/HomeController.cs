using jjodel_persistence.Models.Dto;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace jjodel_persistence.Controllers.Web {

    [Route("")]
    [Route("home")]
    [Controller]
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ProjectService _projectService;
        private readonly AuthService _authService;

        public HomeController(
            ILogger<HomeController> logger,
            ProjectService projectService,
            AuthService authService) {
            _logger = logger;
            _projectService = projectService;
            _authService = authService;
        }

        [Authorize(AuthenticationSchemes =CookieAuthenticationDefaults.AuthenticationScheme, Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Index() {
            DashboardResponse model = new DashboardResponse();
            try {
                // window of the growth charts: last 12 months, current month included.
                DateTime now = DateTime.UtcNow;
                model.FromMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);
                model.MonthLabels = Enumerable.Range(0, 12)
                    .Select(i => model.FromMonth.AddMonths(i).ToString("MMM yy", CultureInfo.InvariantCulture))
                    .ToArray();

                model.Users = await this._authService.GetUserStats(model.FromMonth);
                model.Projects = await this._projectService.GetProjectStats(model.FromMonth);
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
            }

            return View(model);
        }

         
    }
}
