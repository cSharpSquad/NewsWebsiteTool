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
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CommentsController(ApplicationDbContext context) => this.context = context;

        //// GET: api/Comments or api/Comments?newsId=5 or api/Comments?pageNumber=1&pageSize=10
        //// Combined GET: api/comments and api/news/5/comments with sorting and pagination 
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Comment>>> GetComments(
        //	[FromQuery] long? newsId,
        //	[FromQuery] string sort = "CreatedDesc",
        //	[FromQuery] int pageNumber = 1,
        //	[FromQuery] int pageSize = 10)
        //{
        //	IQueryable<Comment> query = context.Comments;

        //	if (newsId.HasValue)
        //	{
        //		query = query.Where(c => c.NewsId == newsId);
        //	}

        //	query = sort switch
        //	{
        //		"CreatedAsc" => query.OrderBy(c => c.Created),
        //		"ModifiedAsc" => query.OrderBy(c => c.Modified),
        //		"ModifiedDesc" => query.OrderByDescending(c => c.Modified),
        //		_ => query.OrderByDescending(c => c.Created),
        //	};

        //	var paginatedList = await PaginatedList<Comment>.CreateAsync(query, pageNumber, pageSize);
        //	return Ok(paginatedList);
        //}

        [HttpGet("{id:long}")]
        public async Task<ActionResult<CommentDto>> GetComment(long id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                NewsId = comment.NewsId,
                Content = comment.Content,
                Created = comment.Created,
                Modified = comment.Modified,

                Links = new List<LinkDto>
                {
                    new(Url.Action(nameof(GetComment), new { id = comment.Id }), "self", "GET"),
                    new(Url.Action(nameof(PutComment), new { id = comment.Id }), "update-comment", "PUT"),
                    new(Url.Action(nameof(DeleteComment), new { id = comment.Id }), "delete-comment", "DELETE")
                }
            };

            return commentDto;
        }

        // GET: api/news/5/comments 
        [HttpGet("~/api/v{version:apiVersion}/news/{newsId:long}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByNewsId(
            long newsId,
            [FromQuery] string sort = "CreatedDesc", // Default sort order 
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            IQueryable<Comment> query = context.Comments.Where(c => c.NewsId == newsId);

            // Apply sorting 
            query = sort switch
            {
                "CreatedAsc" => query.OrderBy(c => c.Created),
                "ModifiedAsc" => query.OrderBy(c => c.Modified),
                "ModifiedDesc" => query.OrderByDescending(c => c.Modified),
                _ => query.OrderByDescending(c => c.Created), // Default sort order 
            };

            // Apply pagination 
            var paginatedList = await PaginatedList<Comment>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        // POST: api/Comments 
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> PutComment(long id, Comment updatedComment)
        {
            var existingComment = await context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }

            // Update fields from updatedComment to existingComment 
            UpdateCommentFields(existingComment, updatedComment);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException();
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

        private static void UpdateCommentFields(Comment existingComment, Comment updatedComment)
        {
            // Update fields that are present in the updatedComment
            if (!string.IsNullOrEmpty(updatedComment.Content))
            {
                existingComment.Content = updatedComment.Content;
            }
            // Add similar checks and updates for other fields as needed
        }
    }
}

