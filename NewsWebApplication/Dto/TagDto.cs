namespace NewsWebApplication.DTO
{
    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // HATEOAS links 
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
