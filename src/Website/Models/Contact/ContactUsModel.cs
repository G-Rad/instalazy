using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Website.Models.Contact
{
	public class ContactUsModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		[AllowHtml]
		public string Message { get; set; }
	}
}