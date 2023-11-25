using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальное ограничение для поля Name в модели Tag
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Определение связей между моделями
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsId, nt.TagId });

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.News)
                .WithMany(n => n.NewsTags)
                .HasForeignKey(nt => nt.NewsId);

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(nt => nt.TagId);

            // Дополнительные настройки могут быть добавлены по необходимости

            base.OnModelCreating(modelBuilder);
        }

        // Дополнительные методы и настройки контекста данных, если необходимо
    }

}
