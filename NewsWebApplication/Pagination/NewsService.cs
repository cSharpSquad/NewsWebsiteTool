using NewDb;
using NewsWebsite.Models;

namespace NewsWebApplication.Pagination
{
    public class NewsService
    {
        private readonly ApplicationDbContext _dbContext;

        public NewsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PaginatedList<News> GetNews(NewsPaginationModel paginationModel)
        {
            var query = _dbContext.News.AsQueryable(); // Adjust based on your actual query logic

            var paginatedList = PaginatedList<News>.Create(query, paginationModel.PageNumber, paginationModel.PageSize);

            return paginatedList;
        }
    }
}
