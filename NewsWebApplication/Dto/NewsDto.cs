using System.ComponentModel.DataAnnotations;

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
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

	public class NewsCreateDTO
	{
		[Required(ErrorMessage = "Title is required.")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters.")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Content is required.")]
		public string Content { get; set; }

		[Required(ErrorMessage = "AuthorId is required.")]
		public long AuthorId { get; set; }

		public List<long>? Tags { get; set; }
	}

	// NewsUpdateDTO.cs
	public class NewsUpdateDTO
	{
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters.")]
		public string Title { get; set; }

		public string Content { get; set; }

		public long? AuthorId { get; set; }

		public List<long>? Tags { get; set; }
	}


}
