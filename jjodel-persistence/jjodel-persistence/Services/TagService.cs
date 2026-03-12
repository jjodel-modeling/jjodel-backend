using jjodel_persistence.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace jjodel_persistence.Services {
    public class TagService {

        private readonly ILogger<TagService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public TagService(ILogger<TagService> logger, ApplicationDbContext applicationDbContext) {

            this._logger = logger;
            this._applicationDbContext = applicationDbContext;
        }

        public async Task<bool> Add(Tag tag) {
            try {

                bool nameAlreadyExists = await this._applicationDbContext.Tags.AnyAsync(t  => t.Name == tag.Name);
                if (nameAlreadyExists) {

                    this._logger.LogWarning("Attempt to create a Tag with an existing Name: {TagName}", tag.Name);
                    return false;
                }

                await this._applicationDbContext.Tags.AddAsync(tag);
                return await this.Save();
            }
            catch (Exception ex) {
                this._logger.LogError("Error saving tag: " + ex.Message);
                return false;
            }
        }

        public async Task<Tag?> GetById(Guid id) {
            // get tag by id.
            return await this._applicationDbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
        }

        // modify
        public async Task<Tag?> GetByIdAsNoTrackingInludeAuthor(Guid id) {
            // get tag by id including author and associated project  
            return await this._applicationDbContext.Tags.Include(a => a.Author).Include(t => t.Projects).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tag?> GetByName(string name) {
            // get tag by name.
            return await this._applicationDbContext.Tags.FirstOrDefaultAsync(tag => tag.Name == name);
        }

        public async Task<List<Tag>> GetsIncludeProjectsAndAuthorAsNoTracking() {
            // get all tags with projects as no tracking include Author for prevent NullReferenceException 
            return await this._applicationDbContext.
                Tags.
                Include(t => t.Projects).
                Include(t => t.Author).
                AsNoTracking().
                ToListAsync();
        }

        public async Task<List<Tag>> GetsIncludeProjectsThenAuthorAsNoTracking() {
            // get all tags with projects and author as no tracking.
            return await this._applicationDbContext.
                Tags.
                Include(t => t.Projects).
                ThenInclude(p => p.Author).
                AsNoTracking().
                ToListAsync();
        }

        public async Task<List<Tag>> GetsAsNoTrackingByName(string name) {
            // get tags by name as no tracking.
            return await this._applicationDbContext.Tags.AsNoTracking().Where(tag => tag.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        }

        public async Task<List<Tag>> GetsIncludeAuthorAsNoTrackingByName(string name) {

            var n = name.Trim().ToLowerInvariant();

            return await _applicationDbContext.Tags
                .Include(t => t.Author)
                .AsNoTracking()
                .Where(t => t.Name != null && t.Name.ToLower().Contains(n))
                .ToListAsync();
        }




        // seconda opzione
        //public async Task<List<Tag>> GetsAsNoTrackingByName(string name) {

        //    // get tags by name as no tracking.
        //    if (string.IsNullOrWhiteSpace(name)) {
        //        return new List<Tag>();
        //    }

        //    var tags = await _applicationDbContext.Tags.

        //        AsNoTracking().
        //        ToListAsync();

        //    return tags
        //        .Where(t => !string.IsNullOrEmpty(t.Name) &&
        //                    t.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
        //        .ToList();
        //}


        // terza opzione
        //public async Task<List<Tag>> GetsAsNoTrackingByName(string name) {
        //    if (string.IsNullOrWhiteSpace(name))
        //        return new List<Tag>();

        //    var pattern = $"%{name.Trim()}%"; // contiene

        //    return await _applicationDbContext.Tags
        //        .AsNoTracking()
        //        .Where(t => t.Name != null && EF.Functions.ILike(t.Name, pattern))
        //        .ToListAsync();
        //}

        public async Task<List<Tag>> GetsByNames(List<string> names) {
            // get tags by name as no tracking.
            return await this._applicationDbContext.Tags.Where(tag => names.Contains(tag.Name)).ToListAsync();
        }

        public async Task<List<Tag>> GetsAsNoTrackingIncludeProjectAndAuthorByNames(List<string> names) {
            // get tags by name as no tracking and include projects.
            return await this._applicationDbContext.Tags.AsNoTrackingWithIdentityResolution().Include(t => t.Projects).ThenInclude(p=> p.Author).Where(tag => names.Contains(tag.Name)).ToListAsync();
        }

        public async Task<bool> Delete(Guid id) {
            // delete tag by id.
            try {
                Tag? tag = await this.GetById(id);

                if (tag is null) {
                    this._logger.LogWarning("Attempting to delete non-existent tag: {TagId}", id);
                    return false;
                }
                this._applicationDbContext.Tags.Remove(tag);
                return await this.Save();
            }
            catch (Exception ex) {
                this._logger.LogError("Delete tag by id error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> Save() {
            // save changes.
            try {
                return await this._applicationDbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex) {
                this._logger.LogError(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
            return false;
        }
    }
}
