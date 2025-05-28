using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {

    [Route("client-log")]
    [Controller]
    public class ClientLogController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClientLogService _analyticsService;
        private ILogger<ClientLogController> _logger;

        public ClientLogController(UserManager<ApplicationUser> userManager, ClientLogService analyticsService, ILogger<ClientLogController> logger) {
            this._userManager = userManager;
            this._analyticsService = analyticsService;
            this._logger = logger;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public ActionResult Index() {
            return View();
        }


        [HttpGet]
        [Route("List/{type?}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> List(string? type = "Error") {

            try {

                List<ClientLog> logs = new List<ClientLog>();
                if(type == "All") {
                    logs = await this._analyticsService.GetAllAsync();
                }
                else if(type == "Warning") {
                    logs = await this._analyticsService.GetAllWarningAsync();
                }
                else if(type == "Information") {
                    logs = await this._analyticsService.GetAllInformationAsync();

                }
                logs = await this._analyticsService.GetAllErrorAsync();
                return PartialView("~/Views/Shared/UC_AnalyticsDevList.cshtml", logs);

            }
            catch (Exception ex) {
                this._logger.LogError(ex.Message);
            }
           
            return PartialView("~/Views/Shared/UC_AnalyticsDevList.cshtml", new List<ClientLog>());
        }

    }
}
