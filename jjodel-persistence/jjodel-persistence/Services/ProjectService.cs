using jjodel_persistence.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace jjodel_persistence.Services {
    public class ProjectService {

        private readonly ILogger<ProjectService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public ProjectService(
            ILogger<ProjectService> _logger,
            ApplicationDbContext _applicationDbContext
            ) {
                
            this._logger = _logger;
            this._applicationDbContext = _applicationDbContext;
        }


        public async Task<bool> Add(Project m) {
            try {
                await this._applicationDbContext.Projects.AddAsync(m);
                return await this.Save();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<List<Project>> GetByAuthor(string AuthorName) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Where(m => 
                    m.Author.UserName.Equals(AuthorName, StringComparison.OrdinalIgnoreCase) || 
                    m.Collaborators.Any(c => c.UserName.Equals(AuthorName, StringComparison.OrdinalIgnoreCase))
                    ).ToListAsync();
        }

        public async Task<Project> GetById(Guid Id) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).FirstOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Project> GetByName(string Name) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                FirstOrDefaultAsync(m => m.Name == Name);
        }

        public async Task<List<Project>> Gets() {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p=> p.Author).
                ToListAsync();
        }

        public async Task<bool> Delete(Guid Id) {
            try {
                Project m = await this.GetById(Id);
                this._applicationDbContext.Projects.Remove(m);
                return await this.Save();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> Delete(Project m) {
            try {
                this._applicationDbContext.Projects.Remove(m);
                return await this.Save();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return false;
            }
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
