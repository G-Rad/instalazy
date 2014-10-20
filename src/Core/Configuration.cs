using Microsoft.WindowsAzure;

namespace Core
{
	public interface IConfiguration
	{
		string InstagramClientId { get; }
		string InstagramAppSecret { get; }
		string InstagramAuthRedirectUrl { get; }
		string StorageConnectionString { get; }
		string EmailFrom { get; }
		string EmailTo { get; }
	}

	public class Configuration : IConfiguration
	{
		public string InstagramClientId
		{
			get { return CloudConfigurationManager.GetSetting("Instagram.ClientId"); }
		}

		public string InstagramAppSecret
		{
			get { return CloudConfigurationManager.GetSetting("Instagram.AppSecret"); }
		}

		public string InstagramAuthRedirectUrl
		{
			get { return CloudConfigurationManager.GetSetting("Instagram.AuthRedirectUrl"); }
		}

		public string StorageConnectionString
		{
			get { return CloudConfigurationManager.GetSetting("StorageConnectionString"); }
		}

		public string EmailTo
		{
			get { return CloudConfigurationManager.GetSetting("Email.ContactTo"); }
		}

		public string EmailFrom
		{
			get { return CloudConfigurationManager.GetSetting("Email.ContactFrom"); }
		}
	}
}