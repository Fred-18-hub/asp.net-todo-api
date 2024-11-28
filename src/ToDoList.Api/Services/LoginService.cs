using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Text;

namespace ToDoList.Api.Services
{
	public class LoginService(IConfiguration config) : ILoginService
	{
		private readonly IConfiguration _config = config;

		public string GenerateToken()
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				expires: DateTime.UtcNow.AddMinutes(1),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public bool ValidatePhoneNumber(string phoneNumber)
		{
			if (
				!int.TryParse(phoneNumber, out int parsedPhone) ||
				phoneNumber.Any(c => (new char[] { '+', '@', '-' }).Contains(c)) ||
				phoneNumber.Length != 10
				)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
