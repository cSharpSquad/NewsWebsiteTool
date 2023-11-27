using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.ComponentModel.DataAnnotations;
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

            [HttpGet]
            async Task<ActionResult<PaginatedList<News>>> GetPaginatedNews([FromQuery] NewsPaginationModel paginationModel)
            {
                try
                {
                    var query = context.News.AsQueryable();

                    // Apply search and filter conditions here based on paginationModel

                    var paginatedList = await PaginatedList<News>.CreateAsync(query, paginationModel.PageNumber, paginationModel.PageSize);

                    return Ok(paginatedList);
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    return StatusCode(500, "Internal Server Error");
                }
            }

            [HttpGet]
            IActionResult GetNews([FromQuery] string title)
            {
                try
                {
                    if (string.IsNullOrEmpty(title))
                    {
                        return BadRequest("Title is required.");
                    }

                    // Your actual logic here
                    // For example:
                    var news = context.News.FirstOrDefault(n => n.Title == title);

                    if (news == null)
                    {
                        return NotFound("News not found");
                    }

                    return Ok(news);
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    return StatusCode(500, "Internal Server Error");
                }
            }

            [HttpPost]
            IActionResult CreateNews([FromBody] NewsCreateModel model)
            {
                try
                {
                    // Other properties and validations for this code
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    // Your logic here
                    var news = new News
                    {
                        Title = model.Title,
                        Content = model.Content,
                        // Set other properties based on your model
                    };

                    context.News.Add(news);
                    context.SaveChanges();

                    return Ok("News created successfully");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it accordingly
                    return StatusCode(500, "Internal Server Error");
                }
            }

        }
    }

    public class NewsCreateModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [StringLength(100, ErrorMessage = "Author name must be less than 100 characters.")]
        public string AuthorName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? PublishedDate { get; set; }

        // Add other properties and validations as needed
    }

    public class ValidSortByAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string sortBy = value as string;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return ValidationResult.Success; // Allow empty sortBy
            }

            // Your custom validation logic for sortBy
            // For example, let's check if it's a valid value
            if (!IsValidSortBy(sortBy))
            {
                return new ValidationResult("Invalid sortBy value.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidSortBy(string sortBy)
        {
            // Your custom logic to determine if sortBy is valid
            // For example, check if it's one of the allowed values
            return sortBy.Equals("CreatedAsc", StringComparison.OrdinalIgnoreCase) ||
                   sortBy.Equals("CreatedDesc", StringComparison.OrdinalIgnoreCase) ||
                   sortBy.Equals("ModifiedAsc", StringComparison.OrdinalIgnoreCase) ||
                   sortBy.Equals("ModifiedDesc", StringComparison.OrdinalIgnoreCase);
        }
    }
}