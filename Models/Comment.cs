using System;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class Comment
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Content { get; set; }

        public long NewsId { get; set; }

        public News News { get; set; }

        //public DateTime Created { get; set; }

        //public DateTime? Modified { get; set; }
    }
}