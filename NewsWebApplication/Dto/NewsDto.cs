namespace NewsWebApplication.DTO
{
    public class NewsDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long AuthorId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        // HATEOAS links 
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

}
