using jjodel_persistence.Models.Dto;
using jjodel_persistence.Models.Entity;
using jjodel_persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace jjodel_persistence.Controllers.API {
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase {


        private readonly ILogger<TagController> _logger;
        private readonly TagService _tagService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;


        public TagController(ILogger<TagController> logger, TagService tagService, UserManager<ApplicationUser> userManager, IMemoryCache cache) {

            this._logger = logger;
            this._tagService = tagService;
            this._userManager = userManager;
            this._cache = cache; 
            
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTagRequest createTagRequest) {

            try {
                this._logger.LogInformation("Add Tag request: " + createTagRequest.Name);
                if (ModelState.IsValid) {

                    Tag tag = new Tag() {

                        Id = Guid.NewGuid(),
                        Name = createTagRequest.Name,
                        Description = createTagRequest.Description,
                        Color = createTagRequest.Color,
                        Creation = DateTime.UtcNow,
                        Author = await this._userManager.FindByNameAsync(User.Identity.Name)
                    };

                    if (await this._tagService.Add(tag)) {
                        InvalidateTagCache();
                        return Ok();
                    }
                }
            }
            catch (Exception ex) {
                this._logger.LogError("Add tag error: " + ex.ToString());
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id:guid}")]
        public async Task<IActionResult> Delete(Guid Id) {
            // delete tag by id.
            try {
                this._logger.LogWarning("Delete tag by id: " + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }

                if(await this._tagService.Delete(Id)) {
                    InvalidateTagCache();
                    return Ok();
                }
            }
            catch(Exception ex) {
                this._logger.LogError("Delete tag error: " + ex.Message);
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() {
            // get all tags with project.
            try {

                List<Tag> tags = await this._tagService.GetsIncludeProjectsAndAuthorAsNoTracking();
                
                return Ok(Convert(tags));
            }
            catch (Exception ex) {
                this._logger.LogError("Get all tags error: " +ex.ToString());
            }

            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Gets() {
            // get all tags with project.
            try {
                List<Tag>? tags = await this._cache.GetOrCreateAsync("all_tags", async entry => {

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                    return await this._tagService.GetsIncludeProjectsAndAuthorAsNoTracking();
                });

                return tags is null ? BadRequest() : Ok(Convert(tags));
            }
            catch (Exception ex) {
                this._logger.LogError("Gets tag error: " + ex.ToString());
            }
            return BadRequest();
        }


        [Authorize(Roles = "User")]
        [HttpGet("{Id:guid}")]
        public async Task<IActionResult> GetById(Guid Id) {
            // get tag by id.
            try {

                this._logger.LogInformation("Get tag by id request: " + Id);

                if(Guid.Empty == Id) {
                    return BadRequest();
                }

                Tag? result = await this._tagService.GetByIdAsNoTrackingInludeAuthor(Id);

                if(result == null) {
                    return BadRequest();
                }

                return Ok(Convert(result));
            }
            catch(Exception ex) {

                this._logger.LogError("Get tag by id error:" + ex.Message.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpGet("search/{Name}")]
        public async Task<IActionResult> GetByName(string Name) {
            // get tag by id.
            try {

                this._logger.LogInformation("Get tag by name request: " + Name);


                List<Tag> tags = await this._tagService.GetsIncludeAuthorAsNoTrackingByName(Name);

                return Ok(Convert(tags));
            }
            catch(Exception ex) {

                this._logger.LogError("Get tag by id error:" + ex.Message.ToString());
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTagRequest updateTagRequest) {
            // update tag.
            try {
                if(ModelState.IsValid) {

                    this._logger.LogInformation("Edit tag request: " + updateTagRequest.Id);

                    Tag? tagToUpdate = await this._tagService.GetByName(updateTagRequest.Name);

                    if(tagToUpdate is null) {
                        return BadRequest();
                    }

                
                    tagToUpdate.Description = updateTagRequest.Description;
                    tagToUpdate.Color = updateTagRequest.Color;

                    if(await this._tagService.Save()) {
                        InvalidateTagCache();
                        return Ok();
                    }
                }
            }
            catch(Exception ex) {
                this._logger.LogError("Update Tag erro: " + ex.Message.ToString());
            }
            return BadRequest();
        }

        

        private void InvalidateTagCache() {
            // invalidate cache.
            this._cache.Remove("all_tags");
        }

        #region Convert

        public static TagResponse Convert(Tag tag) {

            TagResponse response = new TagResponse() 
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                Color = tag.Color,
                Creation = tag.Creation,
                Author = tag.Author.Name,
                Projects = tag.Projects?
                    .Select(p => new ProjectShortResponse {
                        Id = p.Id,
                        _Id = p._Id,
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        ViewpointsNumber = p.ViewpointsNumber,
                        MetamodelsNumber = p.MetamodelsNumber,
                        ModelsNumber = p.ModelsNumber,
                        Creation = p.Creation,
                        LastModified = p.LastModified,
                        IsFavorite = p.IsFavorite,
                        Author = (p.Author != null) ? p.Author.UserName : "",
                        Collaborators = (p.Collaborators != null) ? p.Collaborators.Select(c => c.UserName).ToList() : new List<string?>(),
                    })
                    .ToList()
                    ?? new List<ProjectShortResponse>()
        
            };
            return response;

        }

        public static List<TagResponse> Convert(List<Tag> tags) {

            List<TagResponse> result = new List<TagResponse>();

            foreach (Tag tag in tags) {

                result.Add(Convert(tag));
            }
            return result;

         
        }

        #endregion
    }
}

