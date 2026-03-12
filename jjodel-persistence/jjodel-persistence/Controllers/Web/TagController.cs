using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.Web {

    [Route("tag")]
    [Controller]
    public class TagController : Controller {


        private readonly TagService _tagService;
        private readonly ILogger<TagController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public TagController(UserManager<ApplicationUser> userManager, ILogger<TagController> logger, TagService tagService) {

            this._userManager = userManager;
            this._logger = logger;
            this._tagService = tagService;  
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

                //List<Tag?> tags = await this._tagService.GetsIncludeProjectsAsNoTracking();
                //return PartialView("~/Views/Shared/UC_TagList.cshtml", tags);
            } 
            catch (Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return PartialView("~/Views/Shared/UC_TagList.cshtml", new List<Tag>());
        }

        [HttpGet]
        [Route("delete/{Id:guid}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid Id) {
            try {
                if (Guid.Empty == Id) {
                    return Json(new { Success = false, Message = "Error deleting tag." });
                }

                if (await this._tagService.Delete(Id)) {
                    return Json(new { Success = true, Message = "Operation completed successfully." });
                }
            }
            catch (Exception ex) {
                this._logger.LogError(ex.Message);
            }
            return Json(new { success = false, message = "Error deleting tag." });
        }

        [HttpGet]
        [Route("add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Add() {
            Tag tag = new Tag();

            return PartialView("~/Views/Shared/UC_TagForm.cshtml", tag);
        }


        [HttpGet]
        [Route("t-add")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> T_Add() {
            return View();
        }
           



    }


}
