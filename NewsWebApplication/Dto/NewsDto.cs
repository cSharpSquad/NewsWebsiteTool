namespace NewsWebApplication.DTO
{
    public class NewsDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long AuthorId { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        // HATEOAS links 
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

}
