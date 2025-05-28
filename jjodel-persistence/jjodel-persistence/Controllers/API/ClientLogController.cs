using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace jjodel_persistence.Controllers.API {
    [Route("api/client-log")]
    [ApiController]
    public class ClientLogController : ControllerBase {

        private readonly ILogger<ClientLogController> _logger;
        private readonly ClientLogService _clientLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientLogController(
            ILogger<ClientLogController> logger,
            ClientLogService clientLogService,
            UserManager<ApplicationUser> userManager

            ) {
            this._logger = logger;
            this._clientLogService = clientLogService;
            this._userManager = userManager;

        }



        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SaveUserError([FromBody] CreateClientLog createClientLog) {

            try {
                if(ModelState.IsValid) {
                    ClientLog clientLog = Convert(createClientLog, await this._userManager.FindByNameAsync(User.Identity.Name));

                   
                    if(await this._clientLogService.Add(clientLog)) {
                        return Ok();
                    }
                }               
            }
            catch(Exception ex) {
                this._logger.LogError("An error occurred while saving UserError " + ex.Message);
            }
            return BadRequest();

        }

        #region Convert

        public static ClientLog Convert(CreateClientLog createClientLog, ApplicationUser applicationUser) {
            ClientLog clientLog = new ClientLog() {
                Id = new Guid(),
                User = applicationUser,
                Level = createClientLog.Level,
                Title = createClientLog.Title,
                Version = createClientLog.Version,
                Creation = createClientLog.Creation,
                Message = createClientLog.Message,
                StackTrace = createClientLog.StackTrace,
                CompoStack = createClientLog.CompoStack,
                ContextJson = createClientLog.ContextJson,
                DState = createClientLog.DState,
                TransientJson = createClientLog.TransientJson
            };
            return clientLog;
        }   

        #endregion
    }
}
