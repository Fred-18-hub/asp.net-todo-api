using System.Security.Cryptography;
using System.Text;

namespace ToDoList.Api.Services
{
	public static class PasswordEncryption
	{
		public static string GetSHA256Encryption(string password)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				var passwordByte = Encoding.UTF8.GetBytes(password);
				var hashByte = sha256.ComputeHash(passwordByte);

				return BitConverter.ToString(hashByte).Replace("-", "").ToLower();
			}
		}
	}
}
