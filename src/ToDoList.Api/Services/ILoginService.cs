namespace ToDoList.Api.Services
{
	public interface ILoginService
	{
		string GenerateToken();
		bool ValidatePhoneNumber(string phoneNumber);
	}
}
