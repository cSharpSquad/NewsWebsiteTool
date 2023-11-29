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
        //[Test]
        //public async Task GetComment_ReturnsOkResult()
        //{
        //    var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;

        //    using (var context = new ApplicationDbContext(dbContextOptions))
        //    {
        //        context.Comments.Add(new Comment { Id = 1, Content = "Test Comment 1", Created = DateTime.Now, Modified = DateTime.Now });
        //        context.SaveChanges();
        //    }

        //    using (var context = new ApplicationDbContext(dbContextOptions))
        //    {
        //        var urlHelperMock = new Mock<IUrlHelper>();
        //        urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("/fake-url");

        //        var controller = new CommentsController(context);
        //        controller.Url = urlHelperMock.Object;

        //        var result = await controller.GetComment(1);

        //        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        //        var okResult = (OkObjectResult)result.Result;
        //        var commentDto = okResult?.Value as CommentDto;

        //        Assert.IsNotNull(commentDto);
        //        Assert.That(commentDto.Id, Is.EqualTo(1));
        //    }
        //}

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
