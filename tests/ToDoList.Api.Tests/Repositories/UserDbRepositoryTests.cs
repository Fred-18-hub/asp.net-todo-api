using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NSubstitute;
using ToDoList.Api.Data;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;

namespace ToDoList.Api.Tests.Repositories
{
	public class UserDbRepositoryTests
	{
		private readonly IFixture _fixture;
		private readonly DataContext _context;

		private readonly UserDbRepository _sut;

		public UserDbRepositoryTests()
		{
			_fixture = new Fixture();

			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json") // Add configuration sources as needed
			.Build();

			var options = new DbContextOptionsBuilder<DataContext>()
				.UseNpgsql(configuration.GetConnectionString("DefaultConnectionPG"))
			.Options;

			_context = new DataContext(options);

			_sut = new UserDbRepository(_context);
		}

		[Fact]
		public async Task ValidateUser_ShouldReturnNull()
		{
			// Arrange
			var userDto = _fixture.Create<UserDto>();

			// Act
			var result = await _sut.ValidateUser(userDto);

			// Assert
			result.Should().BeNull();
		}

	}
}
