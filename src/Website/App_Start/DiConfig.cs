using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Core;
using Core.Azure;
using Core.Instagram;

namespace Website
{
	public class DiConfig
	{
		public static void RegisterDependencies()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			var tableService = new TableService(UserRepository.TableName, new Configuration());

			//repositories
			builder.RegisterType<UserRepository>().As<IUserRepository>();

			builder.RegisterInstance(tableService).As<ITableService>().SingleInstance();
			builder.RegisterType<UserService>().As<IUserService>();
			builder.RegisterType<ApiClient>().As<IApiClient>();
			builder.RegisterType<Configuration>().As<IConfiguration>();

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}