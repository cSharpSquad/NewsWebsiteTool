using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace ApplicationDbContext
{

    public class ApplicationDbContext : DbContext
    {
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
