using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SignUpController : ControllerBase
	{
		private readonly IUserDbRepository _userDbRepository;

		public SignUpController(IUserDbRepository userDbRepository)
		{
			_userDbRepository = userDbRepository;
		}

		[HttpPost]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> SignUp([FromBody] UserDto userDto)
		{
			if (userDto == null)
			{
				return BadRequest("Please provide username and password to sign up.");
			}

			var userExist = await _userDbRepository.CheckIfUserNameExists(userDto);

			if (userExist)
			{
				return BadRequest(new { Error = $"Username {userDto.UserName} already exists." });
			}

			var user = new User
			{
				UserName = userDto.UserName,
				Password = PasswordEncryption.GetSHA256Encryption(userDto.Password)
			};
			await _userDbRepository.AddUser(user);

			return Ok($"Hello {userDto.UserName}, your signup was successful!");
		}
	}
}
