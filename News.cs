using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class News
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5)]
        public string Content { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<NewsTag> NewsTags { get; set; }
    }
}
