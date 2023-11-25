using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class Tag
    {
        readonly HashSet<string> uniqueNames = new();
        private string name;

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Tag name
        /// </summary>
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Name
        {
            get => name;
            set
            {
                if (!uniqueNames.Add(value))
                {
                    throw new InvalidOperationException($"The name '{value}' already exists and cannot be added.");
                }
                name = value;
            }
        }


        //public ICollection<NewsTag> NewsTags { get; set; }
    }
}
