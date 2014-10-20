using System.ComponentModel.DataAnnotations;

namespace Core
{
	public class EmailValidator
	{
		public static bool IsValid(string email)
		{
			var validationAttribute = new EmailAddressAttribute();
			var isValid = validationAttribute.IsValid(email);
			return isValid;
		}
	}
}