using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;

namespace NewDb
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CommentsController(ApplicationDbContext context) => this.context = context;

        // GET: api/Comments or api/Comments?newsId=5 or api/Comments?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(
            [FromQuery] long? newsId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            IQueryable<Comment> query = context.Comments;

            // Filter by newsId if it's provided
            if (newsId.HasValue)
            {
                query = query.Where(c => c.NewsId == newsId.Value);
            }

            // Apply pagination
            var paginatedList = await PaginatedList<Comment>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        // GET: api/Comments/5 
        [HttpGet("{id:long}")] // Adding ":long" ensures that the id parameter is of type long.
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
            var comment = await context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // POST: api/Comments 
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        // PUT: api/Comments/5 
        [HttpPut("{id:long}")] // Adding ":long" for consistency.
        public async Task<IActionResult> PutComment(long id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            context.Entry(comment).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // DELETE: api/Comments/5 
        [HttpDelete("{id:long}")] // Adding ":long" for consistency.
        public async Task<IActionResult> DeleteComment(long id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(long id)
        {
            return context.Comments.Any(e => e.Id == id);
        }
    }
}

