using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;

namespace ToDoList.Api.Repositories.Db
{
	public interface IUserDbRepository
	{
		Task<User?> ValidateUser(UserDto userDto);
		Task<bool> CheckIfUserNameExists(UserDto userDto);
		Task AddUser(User user);
	}
}
