using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;

namespace ToDoList.Api.Repositories.Db
{
	public interface IPhoneOtpDbRepository
	{
		Task AddNewPhoneOTP(PhoneOtp phoneOtp);
		Task RequestNewOTP(PhoneOtp phoneOtp);
		Task<PhoneOtp?> GetSimilarPhoneNumber(string phoneNumber);
		Task<PhoneOtp?> VerifyPhoneOTP(PhoneOtpDto phoneOTPdto);
	}
}
