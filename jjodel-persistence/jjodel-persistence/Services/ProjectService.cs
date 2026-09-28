using jjodel_persistence.Models.Dto;
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
                FirstOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Project> GetByJJodelId(string Id) {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                ThenInclude(p => p.Author).
                FirstOrDefaultAsync(m => m._Id == Id);
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
                Include(p=> p.Author).
                ToListAsync();
        }

        public async Task<List<Project>> GetsAsNoTracking() {
            return await this._applicationDbContext.
                Projects.
                Include(p => p.Collaborators).
                Include(p => p.Author).
                AsNoTracking().
                ToListAsync();
        }

        public async Task<List<ProjectTemplate>> GetTemplatesAsNoTracking() {
            return await this._applicationDbContext.
                ProjectTemplates.
                AsNoTracking().
                ToListAsync();
        }

        public async Task<ProjectTemplate> GetTemplateById(Guid Id) {
            return await this._applicationDbContext
                .ProjectTemplates
                .FirstOrDefaultAsync(p => p.Id == Id);
        }

        // project statistics for the backoffice dashboard. fromMonth = first day (UTC) of the 12-month window.
        public async Task<ProjectStatsResponse> GetProjectStats(DateTime fromMonth) {
            ProjectStatsResponse res = new ProjectStatsResponse();

            DateTime last30Days = DateTime.UtcNow.AddDays(-30);
            IQueryable<Project> projects = this._applicationDbContext.Projects.AsNoTracking();

            // counters.
            res.Total = await projects.CountAsync();
            res.NewLast30Days = await projects.CountAsync(p => p.Creation >= last30Days);

            // new projects per month (grouped in SQL).
            var perMonth = await projects
                .Where(p => p.Creation >= fromMonth)
                .GroupBy(p => new { p.Creation.Year, p.Creation.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            res.NewPerMonth = Enumerable.Range(0, 12)
                .Select(i => fromMonth.AddMonths(i))
                .Select(m => perMonth.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Count ?? 0)
                .ToArray();

            // public / private (anything that is not "public" counts as private).
            res.Public = await projects.CountAsync(p => p.Type.ToLower() == "public");
            res.Private = res.Total - res.Public;

            // latest modified projects, without loading the State.
            res.Latest = await projects
                .OrderByDescending(p => p.LastModified)
                .Take(5)
                .Select(p => new LatestProjectResponse {
                    Id = p.Id,
                    Name = p.Name,
                    Author = p.Author != null ? p.Author.UserName : null,
                    Type = p.Type,
                    LastModified = p.LastModified
                })
                .ToListAsync();

            return res;
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
