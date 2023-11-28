using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebApplication.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace NewDb.Controllers
{
<<<<<<< HEAD
	[Route("api/v1/news")]
	[ApiController]
	[Produces("application/json")]
	[ApiExplorerSettings(GroupName = "v1")]
	[SwaggerTag("Operations for creating, updating, retrieving, and deleting news in the application")]
	public class NewsController : ControllerBase
	{
		private readonly ApplicationDbContext context;
=======
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
>>>>>>> kigakoko

        public NewsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Combined GET: api/News with optional search parameters and pagination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews(
            [FromQuery] string authorName,
            [FromQuery] List<long> tagIds,
            [FromQuery] string titlePart,
            [FromQuery] string contentPart,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedDesc")
        {
            var query = context.News.AsQueryable();

<<<<<<< HEAD
		/// <summary>
		/// Gets a specific news item by ID.
		/// </summary>
		/// <param name="id">The ID of the news item.</param>
		/// <returns>The requested news item.</returns>
		// GET: api/News/5
		[HttpGet("{id}")]
		public async Task<ActionResult<News>> GetNews(long id)
		{
			var news = await context.News.FindAsync(id);
=======
            // Apply filters if they are provided
            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(n => n.Author.Name.Contains(authorName));
            }
>>>>>>> kigakoko

            if (tagIds != null && tagIds.Count > 0)
            {
                query = query.Where(n => n.NewsTags.Any(nt => tagIds.Contains(nt.TagId)));
            }

            if (!string.IsNullOrEmpty(titlePart))
            {
                query = query.Where(n => n.Title.Contains(titlePart));
            }

            if (!string.IsNullOrEmpty(contentPart))
            {
                query = query.Where(n => n.Content.Contains(contentPart));
            }

            // Apply sorting
            query = sortBy switch
            {
                "CreatedAsc" => query.OrderBy(n => n.Created),
                "ModifiedAsc" => query.OrderBy(n => n.Modified),
                "ModifiedDesc" => query.OrderByDescending(n => n.Modified),
                _ => query.OrderByDescending(n => n.Created),
            };

            // Apply pagination
            var paginatedList = await PaginatedList<News>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        // GET: api/News/5
        [HttpGet("{id:long}")] // Adding ":long" ensures that the id parameter is of type long.
        public async Task<ActionResult<News>> GetNews(long id)
        {
            var news = await context.News.FindAsync(id);

            return news == null ? throw new ResourceNotFoundException($"Item with ID {id} not found.") : (ActionResult<News>)news;
        }

<<<<<<< HEAD
		// DELETE: api/News/5
		[HttpDelete("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		[SwaggerOperation("Deletes specific news with the supplied id")]
		public async Task<IActionResult> DeleteNews(long id)
		{
			var news = await context.News.FindAsync(id);
			if (news == null)
			{
				return NotFound();
			}
=======
        // POST: api/News
        // Removed the duplicated CreateNews method since PostNews does the same job.
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(News news)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
>>>>>>> kigakoko

            context.News.Add(news);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNews), new { id = news.Id }, news);
        }

        // PUT: api/News/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> PutNews(long id, News news)
        {
            if (id != news.Id)
            {
                return BadRequest();
            }

            context.Entry(news).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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

        // DELETE: api/News/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteNews(long id)
        {
            var news = await context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            context.News.Remove(news);
            await context.SaveChangesAsync();

            return NoContent();
        }

<<<<<<< HEAD
		[HttpGet]
		public async Task<ActionResult<IEnumerable<News>>> GetNews([FromQuery] NewsPaginationModel pagination)
		{
			var query = context.News.AsQueryable();

			// Your existing query 
			var paginatedList = await PaginatedList<News>.CreateAsync(query, pagination.PageNumber, pagination.PageSize);
			return Ok(paginatedList);
		}

		/// <summary> 
		/// Creates a piece of news. 
		/// </summary> 
		/// <param name="news">The news item to create.</param> 
		/// <returns>The created news item.</returns> 
		/// <response code="201">Successfully created a piece of news.</response> 
		/// <response code="400">If the news item is null or invalid.</response> 
		[HttpPost]
		[ProducesResponseType(typeof(News), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult CreateNews([FromBody] News news)
		{
			if (!ModelState.IsValid)
			{
				// Return a bad request response with validation errors
				return BadRequest(ModelState);
			}
			// Your logic to handle the valid news item        // For example, save it to the database
			return Ok(news);
		}
	}
}
=======
        private bool NewsExists(long id)
        {
            return context.News.Any(e => e.Id == id);
        }
    }
}
>>>>>>> kigakoko
