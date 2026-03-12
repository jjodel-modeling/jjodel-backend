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

        public async Task<bool> AddProjectTemplate(ProjectTemplate m) {
            try {
                await this._applicationDbContext.ProjectTemplates.AddAsync(m);
                return await this.Save();
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message);
                return false;
            }
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

        public async Task<List<Project>> GetByAuthorId(Guid AuthorId) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Include(p => p.Tags).
                Where(m =>
                    m.Author.Id.Equals(AuthorId) ||
                    m.Collaborators.Any(c => c.Id.Equals(AuthorId))
                    ).ToListAsync();
        }

        public async Task<List<Project>> GetByAuthor(string AuthorName) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Include(p => p.Tags).
                Where(m => 
                    m.Author.UserName.Equals(AuthorName) || 
                    m.Collaborators.Any(c => c.UserName.Equals(AuthorName))
                    ).ToListAsync();
        }

        public async Task<Project> GetById(Guid Id) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Include(p => p.Tags).
                FirstOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Project> GetByJJodelId(string Id) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Include(p => p.Tags).
                FirstOrDefaultAsync(m => m._Id == Id);
        }

        public async Task<Project> GetByName(string Name) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                Include(p => p.Tags).
                FirstOrDefaultAsync(m => m.Name == Name);
        }

        public async Task<List<Project>> Gets() {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                Include(p=> p.Author).
                Include(p => p.Tags).
                ToListAsync();
        }

        public async Task<List<Project>> GetsAsNoTracking() {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                Include(p => p.Author).
                Include(p => p.Tags).
                AsNoTrackingWithIdentityResolution().
                ToListAsync();
        }

        public async Task<List<Project>> GetsAsNoTrackingByTagName(string TagName) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Author).
                Include(p => p.Tags).
                AsNoTrackingWithIdentityResolution().
                Where(p=> p.Tags.Any(t=> t.Name.ToLower().Contains(TagName.ToLower()))).
                ToListAsync();
        }

        public async Task<List<ProjectTemplate>> GetTemplatesAsNoTracking() {
            return await this._applicationDbContext.
                ProjectTemplates.
                AsNoTracking().
                ToListAsync();
        }

        public async Task<List<ProjectTemplate>> GetTemplatesAsNoTrackingWithPagination(int Skip, int Take) {
            return await this._applicationDbContext.
                ProjectTemplates.
                AsNoTracking().
                OrderBy(pt => pt.Name).
                Skip(Skip).
                Take(Take).
                ToListAsync();
        }

        public async Task<ProjectTemplate> GetTemplateById(Guid Id) {
            return await this._applicationDbContext
                .ProjectTemplates
                .FirstOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<bool> Save() {
            try {
                return await this._applicationDbContext.SaveChangesAsync() > 0;
            }
            catch(Exception ex) {
                this._logger.LogError(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
            return false;

        }

    }
}
