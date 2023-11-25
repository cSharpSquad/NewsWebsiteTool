using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

        // GET: api/Comments 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await context.Comments.ToListAsync();
        }

        // GET: api/Comments/5 
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(long id)
        {
            var comment = await context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // GET: api/Comments/bynews/5 
        [HttpGet("bynews/{newsId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByNews(long newsId)
        {
            return await context.Comments
                                 .Where(c => c.NewsId == newsId)
                                 .ToListAsync();
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
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
