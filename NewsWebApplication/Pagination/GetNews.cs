//using Microsoft.AspNetCore.Mvc;

//namespace NewsWebApplication.Pagination
//{
//    [HttpGet]
//    public IActionResult GetNews([FromQuery] NewsPaginationModel paginationModel)
//    {
//        try
//        {
//            var result = _newsService.GetNews(paginationModel);
//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error while getting news.");
//            return StatusCode(500, "Internal Server Error");
//        }
//    }
//}
