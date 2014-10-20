using System.Net.Mail;
using System.Web.Mvc;
using Core;
using Website.Models.Contact;

namespace Website.Controllers
{
	public class ContactController : Controller
	{
		private readonly IConfiguration _configuration;

		public ContactController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public ActionResult Index(bool? send)
		{
			TempData["EmailSent"] = send;

			return View();
		}

		[HttpPost]
		public ActionResult Send(ContactUsModel model)
		{
			if (!ModelState.IsValid)
				return View("Index");

			var email = new MailMessage(
				_configuration.EmailFrom,
				_configuration.EmailTo,
				"Contact from instalazy",
				string.Format("Email - {0}\r\nMesage: {1}", model.Email, model.Message)
			);

			using (var client = new SmtpClient())
			{
				client.Send(email);
			}

			return RedirectToAction("Index",new {send = true});
		}
	}
}