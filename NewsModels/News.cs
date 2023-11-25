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

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Title
        {
            get => title;
            set
            {
                if (!uniqueNames.Add(value))
                {
                    throw new InvalidOperationException($"Имя '{value}' уже существует и не может быть добавлено.");
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
