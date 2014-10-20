using System;
using System.Collections.Generic;
using System.Linq;
using Core.Instagram;

namespace Core
{
	public interface IUserService
	{
		void RegisterOrUpdate(int userId, string accessToken);
		void LikeLatestUserFeed(User user);
	}

	public class UserService : IUserService
	{
		private readonly IApiClient _apiClient;
		private readonly IUserRepository _userRepository;

		public UserService(IApiClient apiClient, IUserRepository userRepository)
		{
			_apiClient = apiClient;
			_userRepository = userRepository;
		}

		public void RegisterOrUpdate(int userId, string accessToken)
		{
			var existedUser = _userRepository.GetUserById(userId);

			if (existedUser != null)
			{
				if (existedUser.AccessToken != accessToken)
				{
					existedUser.AccessToken = accessToken;
					_userRepository.Update(existedUser);
				}
			}
			else
			{
				_userRepository.Save(new User
				{
					Id = userId,
					AccessToken = accessToken,
					RegistrationDate = DateTime.UtcNow
				});
			}
		}

		public void LikeLatestUserFeed(User user)
		{
			var likesAvailable = 30 - user.LikesDuringLastHour;

			if (likesAvailable <= 0)
				return;

			var latestMedia =
				_apiClient.GetUserFeedMedia(
								user.AccessToken,
								user.RegistrationDate,
								user.LastMediaId)
							.Reverse()
							.ToList();

			var likesLog = new Dictionary<DateTime, string>();

			for (int mediaIndex = 0; mediaIndex < latestMedia.Count() && likesLog.Count <= likesAvailable; mediaIndex++)
			{
				var media = latestMedia[mediaIndex];

				var didUserLikeMedia = media.Likes.Data.Any(x => x.Id == user.Id);

				if (didUserLikeMedia)
					continue;

				var liked = _apiClient.LikeMedia(user.AccessToken, media.Id);

				if (liked)
				{
					likesLog.Add(DateTime.UtcNow, media.Id);
				}
			}

			if (likesLog.Count > 0 || latestMedia.Any())
			{
				user.LastMediaId = latestMedia.Last().Id;
				user.LikesNumber += likesLog.Count;
				
				foreach(var like in likesLog)
					user.LikePosts.Add(like.Key, like.Value);

				_userRepository.Update(user);
			}
		}
	}
}