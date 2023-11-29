using System;
using System.Collections.Generic;
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
    public class TagsControllerTests
    {
        [Test]
        public async Task GetTags_ReturnsOkResult()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                context.Tags.Add(new Tag { Id = 1, Name = "Test Tag 1" });
                context.Tags.Add(new Tag { Id = 2, Name = "Test Tag 2" });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new TagsController(context);

                var result = await controller.GetTags("", 1, 10);

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var resources = okResult?.Value as PaginatedList<Tag>;
                Assert.IsNotNull(resources);
                Assert.That(resources.Items.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task GetTagsWithNoTags_ReturnsEmptyList()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                // No tags added to the database for this test
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new TagsController(context);

                var result = await controller.GetTags("");

                Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
                var okResult = (OkObjectResult)result.Result;
                var resources = okResult?.Value as PaginatedList<Tag>;
                Assert.IsNotNull(resources);
                Assert.That(resources.Items.Count(), Is.EqualTo(0));
            }
        }
    }

}
