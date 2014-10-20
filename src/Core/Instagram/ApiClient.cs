using System;
using System.Collections.Generic;
using System.Linq;
using InstaSharp;
using InstaSharp.Model;
using InstaSharp.Model.Responses;

namespace Core.Instagram
{
	public interface IApiClient
	{
		AuthInfo Auth(string code);
		InstaSharp.Model.User GetUserInfo(int userId, string userAccessToken);
		IList<Media> GetUserFeedMedia(string userAccessToken, DateTime addedAfterDate, string addedAfterId);
		bool LikeMedia(string userAccessToken, string mediaId);
		bool DidUserLikedMedia(int userId, string userAccessToken, string mediaId);
	}

	public class ApiClient : IApiClient
	{
		private readonly InstagramConfig _config;

		public ApiClient(IConfiguration configuration)
		{
			_config = new InstagramConfig(
				"https://api.instagram.com/v1",
				"https://api.instagram.com/oauth",
				configuration.InstagramClientId,
				configuration.InstagramAppSecret,
				configuration.InstagramAuthRedirectUrl);
		}

		public AuthInfo Auth(string code)
		{
			var redirectUri = _config.RedirectURI;

			var config = new InstagramConfig(
				_config.APIURI,
				_config.OAuthURI,
				_config.ClientId,
				_config.ClientSecret,
				redirectUri);

			var authInfo = new Auth(config).RequestToken(code);

			return authInfo;
		}

		public InstaSharp.Model.User GetUserInfo(int userId, string userAccessToken)
		{
			var userEndpoint = new InstaSharp.Endpoints.Users.Authenticated(
				_config,
				new AuthInfo {
					Access_Token = userAccessToken,
					User = new UserInfo{Id = userId}
				}
			);

			var userResponse = userEndpoint.Get();

			return userResponse.Data;
		}

		public IList<Media> GetUserFeedMedia(string userAccessToken, DateTime addedAfterDate, string addedAfterId)
		{
			var apiUsersEndpoint = new UsersAuthenticated
						(_config, new AuthInfo { Access_Token = userAccessToken });

			MediasResponse response;

			if (!string.IsNullOrWhiteSpace(addedAfterId))
				response = apiUsersEndpoint.RecentFeed("self", addedAfterId);
			else
				response = apiUsersEndpoint.Feed("self");

			var resultMedia = response.Data.Where(x => x.CreatedTime > addedAfterDate).ToList();

			return resultMedia;
		}

		public bool DidUserLikedMedia(int userId, string userAccessToken, string mediaId)
		{
			var likes = new InstaSharp.Endpoints.Likes.Authenticated(
				_config,
				new AuthInfo { Access_Token = userAccessToken }
				);

			var getLikesResponse = likes.Get(mediaId);

			var result = getLikesResponse.Data.Any(x => x.Id == userId.ToString());

			return result;
		}

		public bool LikeMedia(string userAccessToken, string mediaId)
		{
			var likes = new InstaSharp.Endpoints.Likes.Authenticated(
				_config,
				new AuthInfo {Access_Token = userAccessToken}
				);

			var response = likes.Post(mediaId);
			
			var successful = response.Meta.Code == 200;

			return successful;
		}
	}
}