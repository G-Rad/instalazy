using System.Web.Mvc;
using Core;

namespace Website.Controllers
{
	public class ControllerBase : Controller
	{
		private readonly IUserRepository _userRepository;

		public IUserRepository UserRepository
		{
			get { return _userRepository; }
		}

		public ControllerBase(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public bool IsUserLoggedin
		{
			get { return CurrentUserId.HasValue; }
		}

		public int? CurrentUserId
		{
			get
			{
				if (!User.Identity.IsAuthenticated)
					return null;

				var userId = int.Parse(User.Identity.Name);

				return userId;
			}
		}

		private User _currentUser;

		public User CurrentUser
		{
			get
			{
				if (_currentUser == null)
				{
					var currentUserId = CurrentUserId;

					if (currentUserId == null)
						return null;

					_currentUser = _userRepository.GetUserById(currentUserId.Value);
				}

				return _currentUser;
			}
		}
	}
}