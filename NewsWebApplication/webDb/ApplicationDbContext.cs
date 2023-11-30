using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NewsWebsite.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace NewDb
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint for the Name field in the Tag model 
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Defining relationships between models 
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsId, nt.TagId });

			//modelBuilder.Entity<NewsTag>()
			//    .HasOne(nt => nt.News)
			//    .WithMany(n => n.NewsTags)
			//    .HasForeignKey(nt => nt.NewsId);

			//modelBuilder.Entity<NewsTag>()
			//    .HasOne(nt => nt.Tag)
			//    .WithMany(t => t.NewsTags)
			//    .HasForeignKey(nt => nt.TagId);

			modelBuilder.Entity<NewsTag>()
	            .HasOne<News>()
	            .WithMany()
	            .HasForeignKey(nt => nt.NewsId)
	            .HasPrincipalKey(news => news.Id);

			modelBuilder.Entity<NewsTag>()
				.HasOne<Tag>()
				.WithMany()
				.HasForeignKey(nt => nt.TagId)
				.HasPrincipalKey(tag => tag.Id);

			// Additional configurations can be added as necessary 

			base.OnModelCreating(modelBuilder);
        }
    }
}
