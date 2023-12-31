using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.DTO;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NewDb
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TagsController(ApplicationDbContext context) => this.context = context;

        // GET: api/Tags with optional name filter and pagination 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags(
            [FromQuery] string namePart = "",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = context.Tags.AsQueryable();

            if (!string.IsNullOrEmpty(namePart))
            {
                query = query.Where(t => t.Name.Contains(namePart));
            }

            var paginatedList = await PaginatedList<Tag>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<TagDto>> GetTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            var tagDto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,

                Links = new List<LinkDto>
                {
                    new(Url.Action(nameof(GetTag), new { id = tag.Id }), "self", "GET"),
                    new(Url.Action(nameof(PutTag), new { id = tag.Id }), "update-tag", "PUT"),
                    new(Url.Action(nameof(DeleteTag), new { id = tag.Id }), "delete-tag", "DELETE")
                }
            };

            return tagDto;
        }

        // GET: api/news/{newsId}/tags with pagination 
        [HttpGet("~/api/v{version:apiVersion}/news/{newsId:long}/tags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByNewsId(
            long newsId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
			var query = context.NewsTags
		        .Where(nt => nt.NewsId == newsId)
		        .Join(context.Tags, nt => nt.TagId, tag => tag.Id, (nt, tag) => tag);
                
			var paginatedList = await PaginatedList<Tag>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        // POST: api/Tags
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }

		// PUT: api/Tags/5
		[HttpPut("{id:long}")]
		public async Task<IActionResult> PutTag(long id, TagUpdateDto tagDto)
		{
			var existingTag = await context.Tags
				.FirstOrDefaultAsync(t => t.Id == id);

			if (existingTag == null)
			{
				return NotFound();
			}

			existingTag.Name = tagDto.Name;

			context.Entry(existingTag).State = EntityState.Modified;

			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TagExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// DELETE: api/Tags/5
		[HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return NoContent();
        }

        private bool TagExists(long id)
        {
            return context.Tags.Any(e => e.Id == id);
        }
    }
}

