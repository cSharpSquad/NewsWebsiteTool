using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NewsWebApplication.Pagination;
using NewsWebsite.Models;
using NewDb;
using NewsWebApplication.DTO;
using Microsoft.AspNetCore.Mvc.Routing;

namespace NewsWebApplication.Tests
{
    [TestFixture]
    public class CommentsControllerTests
    {
        [Test]
        public async Task GetComment_ReturnsNotFoundResult()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                // No comments added to the database
            }

            using (var context = new ApplicationDbContext(dbContextOptions))
            {
                var controller = new CommentsController(context);

                var result = await controller.GetComment(1);

                Assert.IsInstanceOf<NotFoundResult>(result.Result);
            }
        }
    }
}
