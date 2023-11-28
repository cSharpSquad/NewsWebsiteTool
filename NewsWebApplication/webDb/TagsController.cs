﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NewDb
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TagsController(ApplicationDbContext context) => this.context = context;

        // GET: api/Tags or api/Tags?pageNumber=1&pageSize=10
        // Combined GET for tags with optional pagination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = context.Tags.AsQueryable();
            var paginatedList = await PaginatedList<Tag>.CreateAsync(query, pageNumber, pageSize);
            return Ok(paginatedList);
        }

        // GET: api/Tags/5
        [HttpGet("{id:long}")] // Ensure id is of type long
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
        // Merged PutTag and UpdateTag methods into one
        [HttpPut("{id:long}")] // Ensure id is of type long
        public async Task<IActionResult> PutTag(long id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }
            context.Entry(tag).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        // DELETE: api/Tags/5
        [HttpDelete("{id:long}")] // Ensure id is of type long
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

        // GET: api/Tags/bynews/5
        // This method is fine since it has a unique route
        [HttpGet("bynews/{newsId:long}")] // Ensure newsId is of type long
        public async Task<ActionResult<IEnumerable<Tag>>> GetTagsByNews(long newsId)
        {
            var tags = await context.NewsTags
                                    .Where(nt => nt.NewsId == newsId)
                                    .Select(nt => nt.Tag)
                                    .ToListAsync();
            return tags;
        }

        private bool TagExists(long id)
        {
            return context.Tags.Any(e => e.Id == id);
        }
    }
}

