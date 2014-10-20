using System;
using System.Web.Mvc;
using System.Web.Security;
using Core;
using Core.Instagram;

namespace Website.Controllers
{
	public class CallbackController : Controller
	{
		private readonly IUserService _userService;
		private readonly IApiClient _apiClient;

		public CallbackController(IUserService userService, IApiClient apiClient)
		{
			_userService = userService;
			_apiClient = apiClient;
		}

		public ActionResult Index
			(string code, string error, string error_reason, string error_description)
		{
			if (code == null && error == "access_denied" && error_reason == "user_denied")
				return RedirectToAction("Index", "Home");
			
			if (code == null && string.IsNullOrEmpty(error))
				throw new Exception(string.Format(
					"Unable to retrieve auth code. Error:{0} ErrorReason:{1} ErrorDescripton:{2}",
					error, error_reason, error_description));

			var authResonse = _apiClient.Auth(code);

			if (authResonse.Access_Token == null)
				throw new Exception("Unable to retrieve access token");

			var userId = authResonse.User.Id;

			_userService.RegisterOrUpdate(userId, authResonse.Access_Token);

			FormsAuthentication.SetAuthCookie(userId.ToString(), true);

			return RedirectToAction("Index", "Home");
		}
	}
}
