using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.DTO;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;

namespace NewDb
{
	// Versioning applied to the controller 
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class AuthorsController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public AuthorsController(ApplicationDbContext context) => this.context = context;

		// GET: api/Authors with optional name filter and pagination 
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Author>>> GetAuthors(
			[FromQuery] string namePart = "",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return BadRequest("Invalid page number or page size.");
			}

			var query = context.Authors.AsQueryable();

			if (!string.IsNullOrEmpty(namePart))
			{
				query = query.Where(a => a.Name.Contains(namePart));
			}

			var paginatedList = await PaginatedList<Author>.CreateAsync(query, pageNumber, pageSize);
			return Ok(paginatedList);
		}

		[HttpGet("{id:long}")]
		public async Task<ActionResult<AuthorDto>> GetAuthor(long id)
		{
			var author = await context.Authors.FindAsync(id);
			if (author == null)
			{
				return NotFound();
			}

			var authorDto = new AuthorDto
			{
				Id = author.Id,
				Name = author.Name,

				Links = new List<LinkDto>
				{
					new(Url.Action(nameof(GetAuthor), new { id = author.Id }), "self", "GET"),
					new(Url.Action(nameof(PutAuthor), new { id = author.Id }), "update-author", "PUT"),
					new(Url.Action(nameof(DeleteAuthor), new { id = author.Id }), "delete-author", "DELETE")
				}
			};

			return authorDto;
		}

		// GET: api/news/{newsId}/authors  
		[HttpGet("~/api/v{version:apiVersion}/news/{newsId:long}/authors")]
		public async Task<ActionResult<Author>> GetAuthorByNewsId(long newsId)
		{
			var author = await context.Authors
					.Join(
						context.News,
						author => author.Id,
						news => news.AuthorId,
						(author, news) => new { Author = author, NewsId = news.Id }
					)
					.Where(joinResult => joinResult.NewsId == newsId)
					.Select(joinResult => joinResult.Author)
					.FirstOrDefaultAsync();

			if (author == null)
			{
				return NotFound();
			}

			return author;
		}

		// GET: api/Authors/NewsCount with pagination 
		[HttpGet("NewsCount")]
		public async Task<ActionResult<IEnumerable<AuthorNewsCountDto>>> GetAuthorsWithNewsCount(
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			if (pageNumber < 1 || pageSize < 1)
			{
				return BadRequest("Invalid page number or page size.");
			}

			var query = context.Authors
				.Join(
					context.News,
					author => author.Id,
					news => news.AuthorId,
					(author, news) => new { Author = author, NewsId = news.Id }
				)
				.GroupBy(joinResult => joinResult.Author.Id)
				.Select(group => new AuthorNewsCountDto
				{
					AuthorId = group.Key,
					AuthorName = group.First().Author.Name,
					NewsCount = group.Count()
				})
				.OrderByDescending(a => a.NewsCount);

			var paginatedList = await PaginatedList<AuthorNewsCountDto>.CreateAsync(query, pageNumber, pageSize);
			return Ok(paginatedList);
		}

		// POST: api/Authors
		[HttpPost]
		public async Task<ActionResult<Author>> PostAuthor(Author author)
		{
			context.Authors.Add(author);
			await context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
		}

		//// PUT: api/Authors/5
		//[HttpPut("{id:long}")] // Adding ":long" for consistency.
		//public async Task<IActionResult> PutAuthor(long id, Author author)
		//{
		//	if (id != author.Id)
		//	{
		//		return BadRequest();
		//	}

		//	context.Entry(author).State = EntityState.Modified;

		//	try
		//	{
		//		await context.SaveChangesAsync();
		//	}
		//	catch (DbUpdateConcurrencyException)
		//	{
		//		if (!AuthorExists(id))
		//		{
		//			return NotFound();
		//		}
		//		else
		//		{
		//			throw;
		//		}
		//	}

		//	return NoContent();
		//}

		// PUT: api/Authors/5
		[HttpPut("{id:long}")]
		public async Task<IActionResult> PutAuthor(long id, AuthorUpdateDto authorDto)
		{
			var existingAuthor = await context.Authors
				.FirstOrDefaultAsync(a => a.Id == id);

			if (existingAuthor == null)
			{
				return NotFound();
			}

			// Update properties of existingAuthor based on authorDto
			existingAuthor.Name = authorDto.Name;

			context.Entry(existingAuthor).State = EntityState.Modified;

			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!AuthorExists(id))
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

		// DELETE: api/Authors/5
		[HttpDelete("{id:long}")] // Adding ":long" for consistency.
		public async Task<IActionResult> DeleteAuthor(long id)
		{
			var author = await context.Authors.FindAsync(id);
			if (author == null)
			{
				return NotFound();
			}

			context.Authors.Remove(author);
			await context.SaveChangesAsync();

			return NoContent();
		}

		private bool AuthorExists(long id)
		{
			return context.Authors.Any(e => e.Id == id);
		}
	}
}

