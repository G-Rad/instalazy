using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using Core;
using Core.Instagram;
using Website.Models.Home;

namespace Website.Controllers
{
	public class HomeController : ControllerBase
	{
		private readonly IApiClient _apiClient;
		private readonly IConfiguration _configuration;

		//TODO: contact page html/css

		//TODO: js errors handling
		//TODO: js success handling
		//TODO: realtime likes counter?
		//TODO: handle instagram token expiration
		//TODO: white/black lists
		//TODO: randon likes

		public HomeController
			(IUserRepository userRepository,
			IApiClient apiClient,
			IConfiguration configuration)
			: base(userRepository)
		{
			_apiClient = apiClient;
			_configuration = configuration;
		}

		public ActionResult Index()
		{
			ActionResult result;

			var homeModel = new HomeModel
				(_configuration.InstagramAuthRedirectUrl, _configuration.InstagramClientId);

			var userInstalazied = IsUserLoggedin && CurrentUser != null;
			var userNotInstalazied = IsUserLoggedin && CurrentUser == null;

			if (userInstalazied)
			{
				var userData = _apiClient.GetUserInfo(CurrentUser.Id, CurrentUser.AccessToken);
				var userModel = new UserModel(CurrentUser, userData);

				result = View("UserHome", userModel);
			}
			else if (userNotInstalazied)
			{
				result = View("UserNotInstalazied", homeModel);
			}
			else
			{
				result = View(homeModel);
			}

			return result;
		}

		[HttpPost]
		[Authorize]
		public ActionResult ChangeEmail(string email)
		{
			if (!string.IsNullOrEmpty(email) && !EmailValidator.IsValid(email))
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid email address");

			var user = CurrentUser;

			user.Email = email;

			UserRepository.Update(user);

			ActionResult result = new HttpStatusCodeResult(HttpStatusCode.OK);

			return result;
		}

		public ActionResult Stop()
		{
			ActionResult result;

			if (IsUserLoggedin)
			{
				var userId = CurrentUserId.Value;
				UserRepository.Delete(userId);

				FormsAuthentication.SignOut();

				result = View();
			}
			else
			{
				result = RedirectToAction("Index", "Home");
			}

			return result;
		}
	}
}
