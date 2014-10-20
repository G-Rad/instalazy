using System.Web.Mvc;
using Core;
using Website.Models.Layout;

namespace Website.Controllers
{
	public class LayoutController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public LayoutController(IUserRepository userRepository, IConfiguration configuration)
			: base(userRepository)
		{
			_configuration = configuration;
		}

		[ChildActionOnly]
		public ActionResult Menu()
		{
			var menu = new MenuModel
			{
				InstagramClientId = _configuration.InstagramClientId,
				RedirectUri = _configuration.InstagramAuthRedirectUrl,
				ShowLoginButton = !IsUserLoggedin
			};

			return View(menu);
		}
	}
}