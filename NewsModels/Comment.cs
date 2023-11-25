using System;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class Comment
    {
        private string modified;
        private string created;

        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        //public string Content { get; set; }

        public long NewsId { get; set; }

        //public News News { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public string Created
        {
            get => created;
            set => created = value ?? throw new ArgumentNullException(nameof(value), "Created cannot be null.");
        }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}", ApplyFormatInEditMode = true)]
        public string Modified
        {
            get => modified;
            set => modified = value ?? throw new ArgumentNullException(nameof(value), "Modified cannot be null.");
        }

        public void SetCreated(DateTime created)
        {
            Created = created.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        public void SetModified(DateTime modified)
        {
            Modified = modified.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }
    }
}
