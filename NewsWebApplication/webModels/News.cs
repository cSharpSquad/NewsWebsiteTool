using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NewsWebsite.Models
{
    public class News
    {
        readonly HashSet<string> uniqueNames = new();
        private string title;

        public News()
        {
			Created = DateTime.Now;
			Modified = DateTime.Now;
		}


        [Key]
        public long Id { get; set; }

		/// <summary>
		/// News title
		/// </summary>
		[Required(ErrorMessage = "Title is required.")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters.")]
		public string Title
        {
            get => title;
            set
            {
                title = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null.");
                if (!uniqueNames.Add(value))
                {
                    throw new InvalidOperationException($"The title '{value}' already exists and cannot be added.");
                }
                title = value;
            }
        }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        public long AuthorId { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public DateTime? Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public DateTime? Modified { get; set; }
    }
}
