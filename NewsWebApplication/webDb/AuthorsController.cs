using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;

namespace NewDb
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AuthorsController(ApplicationDbContext context) => this.context = context;

        // GET: api/Authors
        // This route remains unchanged as it's the default for the controller.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await context.Authors.ToListAsync();
        }

        // GET: api/Authors/5
        // The route template here is fine since it specifies an ID.
        [HttpGet("{id:long}")] // Adding ":long" ensures that the id parameter is of type long.
        public async Task<ActionResult<Author>> GetAuthor(long id)
        {
            var author = await context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        // PUT: api/Authors/5
        [HttpPut("{id:long}")] // Adding ":long" for consistency.
        public async Task<IActionResult> PutAuthor(long id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            context.Entry(author).State = EntityState.Modified;

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

        // GET: api/Authors/NewsCount
        // Here we specify a different route to avoid conflict.
        [HttpGet("NewsCount")]
        public async Task<ActionResult<IEnumerable<AuthorNewsCountDto>>> GetAuthorsWithNewsCount()
        {
            var authorsWithCount = await context.Authors
                .Select(author => new AuthorNewsCountDto
                {
                    AuthorId = author.Id,
                    AuthorName = author.Name,
                    NewsCount = author.News.Count
                })
                .OrderByDescending(a => a.NewsCount)
                .ToListAsync();

            return authorsWithCount;
        }

        // This method has been removed since it conflicts with GetAuthors.
        // If you need pagination, integrate it into the existing GetAuthors method.

        private bool AuthorExists(long id)
        {
            return context.Authors.Any(e => e.Id == id);
        }
    }

    public class AuthorNewsCountDto
    {
        public long AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int NewsCount { get; set; }
    }
}

