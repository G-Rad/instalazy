using System;
using System.Collections.Generic;
using System.Linq;
using Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Core
{
	public interface IUserRepository
	{
		User GetUserById(int id);
		IEnumerable<User> GetAll();
		void Save(User user);
		void Update(User user);
		void Delete(int userId);
	}

	public class UserRepository : IUserRepository
	{
		private readonly ITableService _tableService;

		public static readonly string TableName = "instalazyme";

		public UserRepository(ITableService tableService)
		{
			_tableService = tableService;
		}

		public User GetUserById(int id)
		{
			var operation =  TableOperation.Retrieve<UserTableEntity>("", id.ToString());

			var retrievedResult = _tableService.Execute(operation);

			if (retrievedResult.Result == null)
				return null;

			var user = (UserTableEntity) retrievedResult.Result;

			var result = user.ToUser();

			return result;
		}

		public IEnumerable<User> GetAll()
		{
			var userRecords = _tableService.ExecuteQuery(new TableQuery<UserTableEntity>());

			var users = userRecords.Select(x => x.ToUser());

			return users;
		}

		public void Save(User user)
		{
			user.RegistrationDate = DateTime.UtcNow;

			var insertOperation = TableOperation.InsertOrReplace(user.ToTableEntity());

			_tableService.Execute(insertOperation);
		}

		public void Update(User user)
		{
			var operation = TableOperation.Retrieve<UserTableEntity>("", user.Id.ToString());
			var retrievedResult = _tableService.Execute(operation);

			if (retrievedResult.Result == null)
				return;

			var mergeOperation = TableOperation.Merge(user.ToTableEntity(retrievedResult.Etag));

			_tableService.Execute(mergeOperation);
		}

		public void Delete(int userId)
		{
			var operation = TableOperation.Retrieve<UserTableEntity>("", userId.ToString());

			var retrievedResult = _tableService.Execute(operation);

			if (retrievedResult.Result == null)
				return;

			var insertOperation = TableOperation.Delete(new UserTableEntity
			{
				PartitionKey = "",
				RowKey = userId.ToString(),
				ETag = retrievedResult.Etag
			});

			_tableService.Execute(insertOperation);
		}
	}
}