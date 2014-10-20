namespace Website.Models.Home
{
	public class HomeModel
	{
		public string RedirectUri { get; set; }

		public string InstagramClientId { get; set; }

		public virtual string InstalazyLinkUrl
		{
			get
			{
				return
					string.Format(
						"https://api.instagram.com/oauth/authorize/?client_id={0}&response_type=code&scope=likes&redirect_uri={1}",
						InstagramClientId, RedirectUri);
			}
		}

		public HomeModel()
		{
		}

		public HomeModel(string redirectUri, string instagramClientId)
		{
			RedirectUri = redirectUri;
			InstagramClientId = instagramClientId;
		}
	}
}