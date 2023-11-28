using System.ComponentModel.DataAnnotations;
using System.Linq;
using NewDb;
using NewsWebsite.Models;
using NewDb.Controllers;

namespace Validations
{
	public class UniqueTitleAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext; var title = value as string;
			if (context!.News.Any(n => n.Title == title))
			{
				return new ValidationResult("Title must be unique.");
			}
			return ValidationResult.Success!;
		}
	}
}