using System.Text.Json;
using System.Text.Json.Serialization;
using jjodel_persistence.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace jjodel_persistence.Services {
    public class ClientLogService {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<ClientLogService> _logger;

        public ClientLogService(
            ApplicationDbContext applicationDbContext, 
            ILogger<ClientLogService> logger
            ) {
            this._applicationDbContext = applicationDbContext;
            this._logger = logger;

        }


        public async Task<bool> Add(ClientLog m) {
            try {
                await this._applicationDbContext.ClientLogs.AddAsync(m);
                return await this.Save();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<List<ClientLog>> GetAllAsync() {
            return await this._applicationDbContext.ClientLogs
                .Include(e => e.User)
                .OrderByDescending(e => e.Creation)
                .ToListAsync();
        }

        public async Task<List<ClientLog>> GetAllErrorAsync() {
            return await this._applicationDbContext.ClientLogs
                .Where(c => c.Level == "Error")
                .Include(e => e.User) 
                .OrderByDescending(e => e.Creation)
                .ToListAsync();
        }

        public async Task<List<ClientLog>> GetAllWarningAsync() {
            return await this._applicationDbContext.ClientLogs
                .Where(c => c.Level == "Warning")
                .Include(e => e.User) 
                .OrderByDescending(e => e.Creation)
                .ToListAsync();
        }

        public async Task<List<ClientLog>> GetAllInformationAsync() {
            return await this._applicationDbContext.ClientLogs
                .Where(c => c.Level == "Information")
                .Include(e => e.User)
                .OrderByDescending(e => e.Creation)
                .ToListAsync();
        }

        public async Task<bool> Save() {
            try {
                if(await this._applicationDbContext.SaveChangesAsync() > 0) {
                    return true;
                }
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
            return false;

        }

    }

}
