using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models
{
    public class NewsTag
    {
        [Key]
        public long Id { get; set; }

        public long NewsId { get; set; }

        public long TagId { get; set; }

    }
}
