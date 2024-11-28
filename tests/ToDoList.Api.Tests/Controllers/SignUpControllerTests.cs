using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Api.Controllers;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;

namespace ToDoList.Api.Tests.Controllers
{
	public class SignUpControllerTests
	{
		private readonly IFixture _fixture;
		private readonly IUserDbRepository _userDbRepository;
		
		private readonly SignUpController _sut;

		public SignUpControllerTests()
		{
			_fixture = new Fixture();
			_userDbRepository = Substitute.For<IUserDbRepository>();
			
			_sut = new SignUpController(_userDbRepository);
		}

		[Fact]
		public async Task SignUp_ShouldReturnBadRequest_WhenUserDtoIsNull()
		{
			// Arrange
			UserDto? userDto = null;

			// Act
			var result = await _sut.SignUp(userDto!);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task SignUp_ShouldReturnBadRequest_IfUsernameExists()
		{
			// Arrange
			var userDto = _fixture.Create<UserDto>();
			_userDbRepository.CheckIfUserNameExists(Arg.Any<UserDto>()).Returns(true);

			// Act
			var result = await _sut.SignUp(userDto);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}

		[Fact]
		public async Task SignUp_ShouldReturnOkResponse_WhenUserDtoIsNotNullAndUsernameDoesNotExist()
		{
			// Arrange
			var userDto = _fixture.Create<UserDto>();
			_userDbRepository.CheckIfUserNameExists(Arg.Any<UserDto>()).Returns(false);

			// Act
			var result = await _sut.SignUp(userDto);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.Should().BeAssignableTo<IActionResult>();
		}
	}
}
