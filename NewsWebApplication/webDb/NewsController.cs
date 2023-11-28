using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebApplication.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;

namespace NewDb.Controllers
{
	// Versioning applied to the controller 
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class NewsController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public NewsController(ApplicationDbContext context)
		{
			this.context = context;
		}

		// Combined GET: api/News with optional search parameters and pagination
		[HttpGet]
		public async Task<ActionResult<IEnumerable<News>>> GetNews(
		   [FromQuery] string authorName,
		   [FromQuery] List<long> tagIds,
		   [FromQuery] List<string> tagNames, // Added to search by tag names 
		   [FromQuery] string titlePart,
		   [FromQuery] string contentPart,
		   [FromQuery] int pageNumber = 1,
		   [FromQuery] int pageSize = 10,
		   [FromQuery] string sortBy = "CreatedDesc")
		{
			var query = context.News
				.Include(n => n.Author)
				.Include(n => n.NewsTags)
				.ThenInclude(nt => nt.Tag)
				.AsQueryable();

			// Apply filters if they are provided 
			if (!string.IsNullOrEmpty(authorName))
			{
				query = query.Where(n => n.Author.Name.Contains(authorName));
			}

			if (tagIds != null && tagIds.Any())
			{
				query = query.Where(n => n.NewsTags.Any(nt => tagIds.Contains(nt.TagId)));
			}

			// Search by tag names 
			if (tagNames != null && tagNames.Any())
			{
				query = query.Where(n => n.NewsTags.Any(nt => tagNames.Contains(nt.Tag.Name)));
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
				_ => query.OrderByDescending(n => n.Created), // Default is Created Desc 
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


		[HttpPost]
		public async Task<ActionResult<News>> PostNews(News news)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Check and add new authors or tags if passed 
			HandleNewAuthorsAndTags(news);

			context.News.Add(news);
			await context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetNews), new { id = news.Id }, news);
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> PutNews(long id, News updatedNews)
		{
			var existingNews = await context.News
				.Include(n => n.NewsTags) // Include related tags 
				.FirstOrDefaultAsync(n => n.Id == id);

			if (existingNews == null)
			{
				return NotFound();
			}

			// Update fields from updatedNews to existingNews 
			UpdateNewsFields(existingNews, updatedNews);

			// Check and add new authors or tags if passed 
			HandleNewAuthorsAndTags(existingNews);

			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
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

		private bool NewsExists(long id)
		{
			return context.News.Any(e => e.Id == id);
		}

		private void HandleNewAuthorsAndTags(News news)
		{
			// Handle new author
			if (news.Author != null && news.Author.Id == 0)
			{
				context.Authors.Add(news.Author);
			}
			// Handle new tags
			if (news.NewsTags != null)
			{
				foreach (var newsTag in news.NewsTags)
				{
					if (newsTag.Tag != null && newsTag.Tag.Id == 0)
					{
						context.Tags.Add(newsTag.Tag);
					}
				}
			}
		}

		private void UpdateNewsFields(News existingNews, News updatedNews)
		{
			// Update fields that are present in the updatedNews
			if (!string.IsNullOrEmpty(updatedNews.Title))
			{
				existingNews.Title = updatedNews.Title;
			}
			if (!string.IsNullOrEmpty(updatedNews.Content))
			{
				existingNews.Content = updatedNews.Content;
			}
		}
	}
}
