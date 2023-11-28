using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NewDb
{
	// Versioning applied to the controller 
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
			[FromQuery] string namePart,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = context.Tags.AsQueryable();

			if (!string.IsNullOrEmpty(namePart))
			{
				query = query.Where(t => t.Name.Contains(namePart));
			}

			var paginatedList = await PaginatedList<Tag>.CreateAsync(query, pageNumber, pageSize);
			var resources = paginatedList.Items.Select(tag => new
			{
				Tag = tag,
				Links = CreateLinksForTag(tag.Id)
			});
			return Ok(resources);
		}

		// GET: api/Tags/5
		[HttpGet("{id:long}")] // Ensure id is of type long
        public async Task<ActionResult<Tag>> GetTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
			var resource = new
			{
				Tag = tag,
				Links = CreateLinksForTag(id)
			};

			return Ok(resource);
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
				.Select(nt => nt.Tag);

			var paginatedList = await PaginatedList<Tag>.CreateAsync(query, pageNumber, pageSize);
			var resources = paginatedList.Items.Select(tag => new
			{
				Tag = tag,
				Links = CreateLinksForTag(tag.Id)
			});
			return Ok(resources);
		}

		// POST: api/Tags
		[HttpPost]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
			var resource = new
			{
				Tag = tag,
				Links = CreateLinksForTag(tag.Id)
			};

			return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, resource);
		}

        // PUT: api/Tags/5
        // Merged PutTag and UpdateTag methods into one
        [HttpPut("{id:long}")] // Ensure id is of type long
        public async Task<IActionResult> PutTag(long id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }
            context.Entry(tag).State = EntityState.Modified;
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
        [HttpDelete("{id:long}")] // Ensure id is of type long
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

		private IEnumerable<object> CreateLinksForTag(long id)
		{
			var links = new List<object>
			{
				new { rel = "self", href = Url.Link(nameof(GetTag), new { id }), method = "GET" },
				new { rel = "update", href = Url.Link(nameof(PutTag), new { id }), method = "PUT" },
				new { rel = "delete", href = Url.Link(nameof(DeleteTag), new { id }), method = "DELETE" },
            };

			return links;
		}

		private bool TagExists(long id)
        {
            return context.Tags.Any(e => e.Id == id);
        }
    }
}

