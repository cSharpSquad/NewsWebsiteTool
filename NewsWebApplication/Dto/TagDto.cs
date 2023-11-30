using System.ComponentModel.DataAnnotations;

namespace NewsWebApplication.DTO
{
    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

	public class TagUpdateDto
	{
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(15, MinimumLength = 3)]
		public string Name { get; set; }
	}

}
