using System;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class Comment
    {
        public Comment()
        {
			Created = DateTime.UtcNow;
			Modified = DateTime.UtcNow;
		}

        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Content { get; set; }

        public long NewsId { get; set; }

        public News? News { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public DateTime? Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public DateTime? Modified { get; set; }
    }
}