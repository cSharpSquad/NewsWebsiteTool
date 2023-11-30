using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebApplication.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using NewsWebApplication.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NewDb.Controllers
{
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
		   [FromQuery] List<long> tagIds,
		   [FromQuery] List<string> tagNames,
		   [FromQuery] string authorName = "",
		   [FromQuery] string titlePart = "",
		   [FromQuery] string contentPart = "",
		   [FromQuery] int pageNumber = 1,
		   [FromQuery] int pageSize = 10,
		   [FromQuery] string sortBy = "CreatedDesc")
		{
			var query = context.News.AsQueryable();

			if (!string.IsNullOrEmpty(authorName))
			{
				var authorId = context.Authors.FirstOrDefault(a => a.Name.Contains(authorName))?.Id;
				if (authorId != null)
				{
					query = query.Where(n => n.AuthorId == authorId);
				}
			}

			if (tagIds != null && tagIds.Any())
			{
				query = query.Where(n => context.NewsTags.Any(nt => nt.NewsId == n.Id && tagIds.Contains(nt.TagId)));
			}

			if (tagNames != null && tagNames.Any())
			{
				query = query.Where(n => context.NewsTags
				  .Any(nt => nt.NewsId == n.Id && tagNames.Contains(context.Tags.FirstOrDefault(t => t.Id == nt.TagId).Name)));
			}

			if (!string.IsNullOrEmpty(titlePart))
			{
				query = query.Where(n => n.Title.Contains(titlePart));
			}

			if (!string.IsNullOrEmpty(contentPart))
			{
				query = query.Where(n => n.Content.Contains(contentPart));
			}

			query = sortBy switch
			{
				"CreatedAsc" => query.OrderBy(n => n.Created),
				"ModifiedAsc" => query.OrderBy(n => n.Modified),
				"ModifiedDesc" => query.OrderByDescending(n => n.Modified),
				_ => query.OrderByDescending(n => n.Created),
			};


			var paginatedList = await PaginatedList<News>.CreateAsync(query, pageNumber, pageSize);


			var options = new JsonSerializerOptions
			{
				ReferenceHandler = ReferenceHandler.Preserve,
				WriteIndented = true
			};

			var json = JsonSerializer.Serialize(paginatedList, options);

			return Ok(paginatedList);
		}

		// GET: api/News/5
		[HttpGet("{id:long}")]
		public async Task<ActionResult<NewsDto>> GetNews(long id)
		{
			var newsItem = await context.News.FindAsync(id);

			if (newsItem == null)
			{
				return NotFound();
			}

			var newsDto = new NewsDto
			{
				Id = newsItem.Id,
				Title = newsItem.Title,
				Content = newsItem.Content,
				AuthorId = newsItem.AuthorId,
				Created = newsItem.Created,
				Modified = newsItem.Modified,
				Links = new List<LinkDto>
				{
					new(Url.Action(nameof(GetNews), new { id = newsItem.Id }), "self", "GET"),
					new(Url.Action(nameof(PutNews), new { id = newsItem.Id }), "update-news", "PUT"),
					new(Url.Action(nameof(DeleteNews), new { id = newsItem.Id }), "delete-news", "DELETE") 
				}
			};

			return newsDto;
		}

		[HttpPost]
		public async Task<ActionResult<News>> PostNews([FromBody] NewsCreateDTO newsCreateDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var news = new News
			{
				Title = newsCreateDTO.Title,
				Content = newsCreateDTO.Content,
				AuthorId = newsCreateDTO.AuthorId,
				Created = DateTime.Now,
				Modified = DateTime.Now
			};

			HandleNewAuthorsAndTags(news);

			context.News.Add(news);
			await context.SaveChangesAsync();

			await HandleTagsAssociation(news.Id, newsCreateDTO.Tags);

			return CreatedAtAction(nameof(GetNews), new { id = news.Id }, news);
		}

		[HttpPut("{id:long}")]
		public async Task<IActionResult> PutNews(long id, [FromBody] NewsUpdateDTO newsUpdateDTO)
		{
			var existingNews = await context.News
				.FirstOrDefaultAsync(n => n.Id == id);

			if (existingNews == null)
			{
				return NotFound();
			}

			existingNews.Title = newsUpdateDTO.Title ?? existingNews.Title;
			existingNews.Content = newsUpdateDTO.Content ?? existingNews.Content;
			existingNews.AuthorId = newsUpdateDTO.AuthorId ?? existingNews.AuthorId;
			existingNews.Modified = DateTime.Now;

			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new DbUpdateConcurrencyException();
			}

			await HandleTagsAssociation(existingNews.Id, newsUpdateDTO.Tags);

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

		private void HandleNewAuthorsAndTags(News news)
		{
			if (news.AuthorId == 0)
			{
				context.Authors.Add(new Author { Name = "UNKNOWN AUTHOR" });
			}
		}

		private async Task HandleTagsAssociation(long newsId, List<long> tagIds)
		{
			var existingTags = await context.NewsTags.Where(nt => nt.NewsId == newsId).ToListAsync();
			context.NewsTags.RemoveRange(existingTags);

			var newTags = tagIds.Select(tagId => new NewsTag { NewsId = newsId, TagId = tagId }).ToList();
			context.NewsTags.AddRange(newTags);

			await context.SaveChangesAsync();
		}
	}
}
