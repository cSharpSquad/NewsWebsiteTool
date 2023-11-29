using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NewDb;
using NewDb.Controllers;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using NUnit.Framework;

namespace NewsWebApplication.Tests
{
    [TestFixture]
    public class NewsControllerTests
    {
        [Test]
        public async Task GetNews_ReturnsOkResult()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var author = new Author { Id = 1, Name = "Test Author" };
                context.Authors.Add(author);
                context.News.Add(new News { Id = 1, Title = "Test News 1", Content = "Content 1", Author = author });
                context.News.Add(new News { Id = 2, Title = "Test News 2", Content = "Content 2", Author = author });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new NewsController(context);

                var result = await controller.GetNews(null, null, null, null, null, 1, 10, "CreatedDesc");

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var paginatedList = okResult?.Value as PaginatedList<News>;
                Assert.IsNotNull(paginatedList);
                Assert.That(paginatedList.Items.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetNewsWithNoNews_ReturnsEmptyList()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                // No news added to the database for this test
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new NewsController(context);

                var result = await controller.GetNews(null, null, null, null, null, 1, 10, "CreatedDesc");

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var paginatedList = okResult?.Value as PaginatedList<News>;
                Assert.IsNotNull(paginatedList);
                Assert.That(paginatedList.Items.Count(), Is.EqualTo(0));
            }
        }
    }
}
