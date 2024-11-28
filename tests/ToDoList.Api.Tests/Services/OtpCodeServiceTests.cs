using FluentAssertions;
using ToDoList.Api.Services;

namespace ToDoList.Api.Tests.Services
{
	public class OtpCodeServiceTests
	{
		private readonly OtpCodeService _sut = new();

		[Fact]
		public void GenerateOTP_ShouldReturnString()
		{
			// Arrange
			var ofExpected = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			// Act
			var result = _sut.GenerateOTP();

			// Assert
			result.Should().NotBeNull().And.BeOfType<string>();
			result.Should().HaveLength(6);
		}

		[Fact]
		public void CheckOTPExpiry_ShouldReturnTrue_WhenOtpHasExpired()
		{
			// Arrange
			DateTime otpDateCreated = DateTime.UtcNow.AddMinutes(-2);

			// Act
			var result = _sut.CheckOTPExpiry(otpDateCreated);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public void CheckOTPExpiry_ShouldReturnFalse_WhenOtpHasNotExpired()
		{
			// Arrange
			DateTime otpDateCreated = DateTime.UtcNow;

			// Act
			var result = _sut.CheckOTPExpiry(otpDateCreated);

			// Assert
			result.Should().BeFalse();
		}
	}
}
