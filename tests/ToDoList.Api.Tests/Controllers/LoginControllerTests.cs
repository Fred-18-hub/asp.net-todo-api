using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Api.Controllers;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;
using ToDoList.Api.Services;

namespace ToDoList.Api.Tests.Controllers
{
	public class LoginControllerTests
	{
        private readonly IFixture _fixture;
        private readonly IUserDbRepository _userDbRepository;
		private readonly IPhoneOtpDbRepository _otpDbRepository;
        private readonly ILoginService _loginService;
		private readonly IOtpCodeService _otpCodeService;

        private readonly LoginController _sut;

        public LoginControllerTests()
        {
            _fixture = new Fixture();
            _userDbRepository = Substitute.For<IUserDbRepository>();
            _otpDbRepository = Substitute.For<IPhoneOtpDbRepository>();
            _loginService = Substitute.For<ILoginService>();
			_otpCodeService = Substitute.For<IOtpCodeService>();

            _sut = new LoginController(_userDbRepository, _otpDbRepository, _loginService, _otpCodeService);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorizedResponse_WhenValidateUserIsNull()
        {
            // Arrange
            var userDto = _fixture.Create<UserDto>();
            User? user = null;
            _userDbRepository.ValidateUser(Arg.Any<UserDto>()).Returns(user);

            // Act
            var result = await _sut.Login(userDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            result.Should().BeAssignableTo<IActionResult>();
        }

		[Fact]
		public async Task Login_ShouldReturnOkResponse_WhenValidateUserIsNotNull()
		{
			// Arrange
			var userDto = _fixture.Create<UserDto>();
			var user = _fixture.Create<User>();
			_userDbRepository.ValidateUser(Arg.Any<UserDto>()).Returns(user);

			// Act
			var result = await _sut.Login(userDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task GenerateOTP_ShouldReturnBadRequest_WhenPhoneNumberIsNotValid()
		{
			// Arrange
			var phone = _fixture.Create<string>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(false);

			// Act
			var result = await _sut.GenerateOTP(phone);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task GenerateOTP_ShouldReturnOkResponse_WhenSamePhoneOTPDoesNotExist()
		{
			// Arrange
			var phone = _fixture.Create<string>();
			PhoneOtp? samePhoneOTP = null;
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.GetSimilarPhoneNumber(Arg.Any<string>()).Returns(samePhoneOTP);

			// Act
			var result = await _sut.GenerateOTP(phone);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task GenerateOTP_ShouldReturnOkResponse_WhenSamePhoneOTPExistsButOTPHasExpired()
		{
			// Arrange
			var phone = _fixture.Create<string>();
			var samePhoneOTP = _fixture.Create<PhoneOtp>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.GetSimilarPhoneNumber(Arg.Any<string>()).Returns(samePhoneOTP);
			_otpCodeService.CheckOTPExpiry(Arg.Any<DateTime>()).Returns(true);

			// Act
			var result = await _sut.GenerateOTP(phone);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task GenerateOTP_ShouldReturnOkResponse_WhenSamePhoneOTPExistsAndOTPHasNotExpired()
		{
			// Arrange
			var phone = _fixture.Create<string>();
			var samePhoneOTP = _fixture.Create<PhoneOtp>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.GetSimilarPhoneNumber(Arg.Any<string>()).Returns(samePhoneOTP);
			_otpCodeService.CheckOTPExpiry(Arg.Any<DateTime>()).Returns(false);

			// Act
			var result = await _sut.GenerateOTP(phone);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task LoginByPhone_ShouldReturnBadRequest_WhenPhoneNumberIsNotValid()
		{
			// Arrange
			var phoneOTPdto = _fixture.Create<PhoneOtpDto>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(false);

			// Act
			var result = await _sut.LoginByPhone(phoneOTPdto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task LoginByPhone_ShouldReturnUnauthorizedResponse_WhenPhoneOTPIsNotVerified()
		{
			// Arrange
			var phoneOTPdto = _fixture.Create<PhoneOtpDto>();
			PhoneOtp? verifiedPhoneOTP = null;
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.VerifyPhoneOTP(Arg.Any<PhoneOtpDto>()).Returns(verifiedPhoneOTP);

			// Act
			var result = await _sut.LoginByPhone(phoneOTPdto);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task LoginByPhone_ShouldReturnUnauthorizedResponse_WhenPhoneOTPIsVerifiedButOTPHasExpired()
		{
			// Arrange
			var phoneOTPdto = _fixture.Create<PhoneOtpDto>();
			var verifiedPhoneOTP = _fixture.Create<PhoneOtp>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.VerifyPhoneOTP(Arg.Any<PhoneOtpDto>()).Returns(verifiedPhoneOTP);
			_otpCodeService.CheckOTPExpiry(Arg.Any<DateTime>()).Returns(true);

			// Act
			var result = await _sut.LoginByPhone(phoneOTPdto);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task LoginByPhone_ShouldReturnOkResponse_WhenPhoneOTPIsVerifiedAndOTPHasNotExpired()
		{
			// Arrange
			var phoneOTPdto = _fixture.Create<PhoneOtpDto>();
			var verifiedPhoneOTP = _fixture.Create<PhoneOtp>();
			_loginService.ValidatePhoneNumber(Arg.Any<string>()).Returns(true);
			_otpDbRepository.VerifyPhoneOTP(Arg.Any<PhoneOtpDto>()).Returns(verifiedPhoneOTP);
			_otpCodeService.CheckOTPExpiry(Arg.Any<DateTime>()).Returns(false);

			// Act
			var result = await _sut.LoginByPhone(phoneOTPdto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}
	}
}
