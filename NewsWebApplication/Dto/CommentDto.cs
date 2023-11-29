namespace NewsWebApplication.DTO
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long NewsId { get; set; }
        public string Content { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
