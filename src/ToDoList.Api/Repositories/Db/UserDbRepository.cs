using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Services;

namespace ToDoList.Api.Repositories.Db
{
	public class UserDbRepository : IUserDbRepository
	{
		private readonly DataContext _context;

		public UserDbRepository(DataContext context)
        {
			_context = context;
		}

		public async Task<User?> ValidateUser(UserDto userDto)
		{
			var response = await _context.Users.FirstOrDefaultAsync(u =>
							u.UserName.ToLower() == userDto.UserName.ToLower() && 
							u.Password == PasswordEncryption.GetSHA256Encryption(userDto.Password)
							);

			return response;
		}

        public async Task<bool> CheckIfUserNameExists(UserDto userDto)
		{
			var usersWithSameUserName = await _context.Users.Where(u => u.UserName.ToLower() == userDto.UserName.ToLower()).ToListAsync();

			if (usersWithSameUserName.Count != 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task AddUser(User user)
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
		}

	}
}
