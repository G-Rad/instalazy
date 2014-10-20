using System;
using System.Threading;
using Core;
using Core.Azure;
using Core.Instagram;
using NLog;

namespace WebJob
{
	class Program
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		const int IntervalInSec = 20;

		static void Main(string[] args)
		{
			var config = new Configuration();
			var apiClient = new ApiClient(config);
			var userProvider = new UserRepository(
				new TableService(UserRepository.TableName, config));
			
			var userService = new UserService(apiClient, userProvider);

			while (true)
			{
				try
				{
					var lastStartTime = DateTime.Now;

					var users = userProvider.GetAll();

					foreach (var user in users)
						userService.LikeLatestUserFeed(user);

					WaitIfTooFast(lastStartTime);

					Console.WriteLine("Update has been finished at {0:yyyy MM dd - HH:mm:ss}", DateTime.Now);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: {0} {1}", ex.Message, ex.GetType().Name);
					Logger.Error(ex);
				}
			}
		}

		private static void WaitIfTooFast(DateTime lastStartTime)
		{
			var secondsSinceLastStart = (DateTime.Now - lastStartTime).TotalSeconds;
			if (secondsSinceLastStart < IntervalInSec)
				Thread.Sleep((int)(IntervalInSec - secondsSinceLastStart) * 1000);
		}
	}

}
