using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Core.Azure
{
	public class UserTableEntity : TableEntity
	{
		public string AccessToken { get; set; }

		public DateTime DateRegistration { get; set; }

		public string LastMediaId { get; set; }
		
		public string Email { get; set; }

		public int LikesNumber { get; set; }

		public string LikePosts { get; set; }

		public User ToUser()
		{
			var likePosts = new Dictionary<DateTime, string>();
			if (!string.IsNullOrWhiteSpace(LikePosts))
				likePosts = JsonConvert.DeserializeObject<Dictionary<DateTime, string>>(LikePosts);

			return new User
						{
							Id = int.Parse(this.RowKey),
							Email = Email,
							AccessToken = AccessToken,
							LastMediaId = LastMediaId,
							RegistrationDate = DateRegistration,
							LikesNumber = LikesNumber,
							LikePosts = likePosts
						};
		}
	}
}