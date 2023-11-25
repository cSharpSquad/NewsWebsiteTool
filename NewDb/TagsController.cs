using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NewsWebsite.Models;
using System.Linq;

namespace NewDb
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TagsController(ApplicationDbContext context) =>  this.context = context;

        // GET: api/Tags 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await context.Tags.ToListAsync();
        }

        // GET: api/Tags/5 
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // POST: api/Tags 
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }

        // PUT: api/Tags/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(long id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            context.Entry(tag).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Tags/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("bynews/{newsId}")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByNews(long newsId)
        {
            return await context.NewsTags
                                 .Where(nt => nt.NewsId == newsId)
                                 .Select(nt => nt.Tag)
                                 .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(long id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            context.Entry(tag).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
