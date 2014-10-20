
using Core;

namespace Website.Models.Home
{
	public class UserModel
	{
		public string Fullname { get; set; }

		public string Email { get; set; }

		public string ProfilePicture { get; set; }

		public int LikesNumber { get; set; }

		public string EmailUpdateButtonText
		{
			get { return string.IsNullOrEmpty(Email) ? "Save" : "Update"; }
		}

		public bool HasEmail
		{
			get { return !string.IsNullOrEmpty(Email); }
		}

		public UserModel(User user, InstaSharp.Model.User instagramProfile)
		{
			Fullname = instagramProfile.FullName;
			Email = user.Email;
			LikesNumber = user.LikesNumber;
			ProfilePicture = instagramProfile.ProfilePicture;
		}
	}
}