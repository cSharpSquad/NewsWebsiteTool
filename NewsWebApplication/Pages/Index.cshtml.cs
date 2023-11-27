using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NewDb;
using NewsWebsite.Models;
using NewsWebApplication.Pagination;

namespace NewsWebApplication.Pages
{
    //public class IndexModel : PageModel
    //{
    //	private readonly ILogger<IndexModel> _logger;

    //	public IndexModel(ILogger<IndexModel> logger)
    //	{
    //		_logger = logger;
    //	}

    //	public void OnGet()
    //	{

    //	}
    //}

    //public class IndexModel : PageModel
    //{
    //    private readonly ILogger<IndexModel> _logger;
    //    private readonly ApplicationDbContext _dbContext;

    //    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext dbContext)
    //    {
    //        _logger = logger;
    //        _dbContext = dbContext;
    //    }

    //    public List<News> NewsList { get; set; }

    //    public void OnGet()
    //    {
    //        // Retrieve the latest news items directly from the database context
    //        NewsList = _dbContext.News.ToList();
    //    }
    //}

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<News> NewsList { get; set; }

        public void OnGet(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                NewsList = _context.News
                    .OrderByDescending(news => news.Created)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting news.");
                // Handle the error appropriately, e.g., return an error page
            }
        }
    }

}
