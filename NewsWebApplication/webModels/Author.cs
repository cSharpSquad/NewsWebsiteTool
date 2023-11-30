using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class Author
    {
        readonly HashSet<string> uniqueNames = new();
        private string name;

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// author name
        /// </summary>
        [StringLength(15, MinimumLength = 3)]
        public required string Name
        {
            get => name;
            set
            {
                name = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null.");
                if (!uniqueNames.Add(value))
                {
                    throw new InvalidOperationException($"The name '{value}' already exists and cannot be added.");
                }
                name = value;
            }
        }
    }
}
