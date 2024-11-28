namespace ToDoList.Api.Services
{
	public interface IOtpCodeService
	{
		string GenerateOTP();
		bool CheckOTPExpiry(DateTime otpDateCreated);
	}
}
