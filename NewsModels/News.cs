using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NewsModels
{
    public class News
    {
        readonly HashSet<string> uniqueNames = new();
        private string title;

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// News title
        /// </summary>
        [StringLength(30, MinimumLength = 5)]
        public required string Title
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

        [Required]
        [StringLength(255, MinimumLength = 5)]
        public string Content { get; set; }

        public long AuthorId { get; set; }

        //public Author Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        //public ICollection<Comment> Comments { get; set; }

        //public ICollection<NewsTag> NewsTags { get; set; }
    }
}
