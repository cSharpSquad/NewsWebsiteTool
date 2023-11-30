using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NewDb;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using NUnit.Framework;

namespace NewsWebApplication.Tests
{
    [TestFixture]
    public class AuthorsControllerTests
    {
        [Test]
        public async Task GetAuthors_ReturnsOkResult()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                context.Authors.Add(new Author { Id = 1, Name = "Test Author 1" });
                context.Authors.Add(new Author { Id = 2, Name = "Test Author 2" });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new AuthorsController(context);

                var result = await controller.GetAuthors(null);

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var paginatedList = okResult?.Value as PaginatedList<Author>;
                Assert.IsNotNull(paginatedList);
                Assert.That(paginatedList.Items.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetAuthorsWithNoAuthors_ReturnsEmptyList()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                // No authors added to the database for this test
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new AuthorsController(context);

                var result = await controller.GetAuthors(null);

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var paginatedList = okResult?.Value as PaginatedList<Author>;
                Assert.IsNotNull(paginatedList);
                Assert.That(paginatedList.Items.Count(), Is.EqualTo(0));
            }
        }
    }
}