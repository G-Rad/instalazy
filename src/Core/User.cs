using System;
using System.Collections.Generic;
using System.Linq;
using Core.Azure;
using Newtonsoft.Json;

namespace Core
{
	public class User
	{
		public int Id { get; set; }

		public string AccessToken { get; set; }

		public string LastMediaId { get; set; }

		public DateTime RegistrationDate { get; set; }
		
		public string Email { get; set; }

		public int LikesNumber { get; set; }

		public Dictionary<DateTime,string> LikePosts { get; set; }

		public int LikesDuringLastHour
		{
			get
			{
				if (LikePosts == null || LikePosts.Count == 0)
					return 0;

				var now = DateTime.UtcNow;

				var oneHourAgo = now.AddHours(-1);

				var result = LikePosts.Keys.Count(x => x > oneHourAgo);

				return result;
			}
		}

		public UserTableEntity ToTableEntity(string eTag = null)
		{
			var likePostsJson = JsonConvert.SerializeObject(LikePosts);

			return new UserTableEntity
						{
							PartitionKey = "",
							RowKey = Id.ToString(),
							Email = Email,
							LikesNumber = LikesNumber,
							LastMediaId = LastMediaId,
							AccessToken = AccessToken,
							DateRegistration = RegistrationDate,
							LikePosts = likePostsJson,
							ETag = eTag
						};
		}
	}
}