using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ToDoList.Api.Services;

namespace ToDoList.Api.Tests.Services
{
	public class LoginServiceTests
	{
		private readonly IConfiguration _config;

		private readonly LoginService _sut;

		public LoginServiceTests()
		{
			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json") // Add configuration sources as needed
			.Build();

			_config = configuration;

			_sut = new LoginService(_config);
		}

		[Fact]
		public void GenerateToken_ShouldReturnString()
		{
			// Arrange

			// Act
			var result = _sut.GenerateToken();

			// Assert
			result.Should().NotBeNull().And.BeOfType<string>();
		}

		[Fact]
		public void ValidatePhoneNumber_ShouldReturnFalse_WhenPhoneNumberFailsValidation()
		{
			// Arrange
			string phoneNumber = "+549766779";

			// Act
			var result = _sut.ValidatePhoneNumber(phoneNumber);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public void ValidatePhoneNumber_ShouldReturnTrue_WhenPhoneNumberPassesValidation()
		{
			// Arrange
			string phoneNumber = "0549766779";

			// Act
			var result = _sut.ValidatePhoneNumber(phoneNumber);

			// Assert
			result.Should().BeTrue();
		}
	}
}
