using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Core.Azure
{
	public interface ITableService
	{
		TableResult Execute(TableOperation operation);
		IEnumerable<T> ExecuteQuery<T>(TableQuery<T> query) where T : ITableEntity, new();
	}

	public class TableService : ITableService
	{
		private readonly string _tableName;
		private readonly IConfiguration _configuration;

		private CloudTable _table;

		public TableService(string tableName, IConfiguration configuration)
		{
			_tableName = tableName;
			_configuration = configuration;

			Init();
		}

		public TableResult Execute(TableOperation operation)
		{
			return _table.Execute(operation);
		}

		public IEnumerable<T> ExecuteQuery<T>(TableQuery<T> query) where T : ITableEntity, new()
		{
			return _table.ExecuteQuery(query);
		}

		private void Init()
		{
			var storageAccount = CloudStorageAccount.Parse(
				_configuration.StorageConnectionString);

			var tableClient = storageAccount.CreateCloudTableClient();

			_table = tableClient.GetTableReference(_tableName);

			_table.CreateIfNotExists();
		}
	}
}