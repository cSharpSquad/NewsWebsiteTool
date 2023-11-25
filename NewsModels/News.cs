using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NewsWebsite.Models
{
    public class News
    {
        readonly HashSet<string> uniqueNames = new();
        private string title;
        private string modified;
        private string created;

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

        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public string Created
        {
            get => created;
            set => created = DateTime.TryParse(value, out _) ? value : throw new ArgumentException("Invalid DateTime format");
        }

        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public string Modified
        {
            get => modified;
            set => modified = DateTime.TryParse(value, out _) ? value : throw new ArgumentException("Invalid DateTime format");
        }

        public void SetCreated(DateTime created)
        {
            Created = created.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        public void SetModified(DateTime modified)
        {
            Modified = modified.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        //public ICollection<Comment> Comments { get; set; }

        //public ICollection<NewsTag> NewsTags { get; set; }
    }
}
