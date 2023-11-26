using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NewsWebsite.Models;
using System.Linq;

namespace NewDb
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public NewsController(ApplicationDbContext context) => this.context = context;

        // GET: api/News 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
            return await context.News.ToListAsync();
        }

        // GET: api/News/5 
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(long id)
        {
            var news = await context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        // POST: api/News 
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(News news)
        {
            context.News.Add(news);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetNews", new { id = news.Id }, news);
        }

        // PUT: api/News/5 
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
        [HttpGet("search")]

        public async Task<ActionResult<IEnumerable<News>>> SearchNews([FromQuery] string authorName, [FromQuery] List<long> tagIds, [FromQuery] string titlePart, [FromQuery] string contentPart, [FromQuery] string sortBy = "CreatedDesc")
        {
            var query = context.News.AsQueryable();

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(n => n.Author.Name == authorName);
            }

            if (tagIds.Count > 0)
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

            query = sortBy switch
            {
                "CreatedAsc" => query.OrderBy(n => n.Created),
                "ModifiedAsc" => query.OrderBy(n => n.Modified),
                "ModifiedDesc" => query.OrderByDescending(n => n.Modified),
                _ => query.OrderByDescending(n => n.Created),
            };
            return await query.ToListAsync();
        }

        [HttpGet("/news/{newsId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByNewsId(long newsId)
        {
            return await context.Comments.Where(c => c.NewsId == newsId).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<News>>> SearchNews(string title = null, string content = null, long? authorId = null)
        {
            var query = context.News.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(n => n.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                query = query.Where(n => n.Content.Contains(content));
            }

            if (authorId.HasValue)
            {
                query = query.Where(n => n.AuthorId == authorId.Value);
            }

            return await query.ToListAsync();
        }
    }
}