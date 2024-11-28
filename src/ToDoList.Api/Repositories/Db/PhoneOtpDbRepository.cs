using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Services;

namespace ToDoList.Api.Repositories.Db
{
	public class PhoneOtpDbRepository : IPhoneOtpDbRepository
	{
		private readonly DataContext _context;
		private readonly IOtpCodeService _codeService;

		public PhoneOtpDbRepository(
			DataContext context,
			IOtpCodeService otpCodeService)
		{
			_context = context;
			_codeService = otpCodeService;
		}

		public async Task AddNewPhoneOTP(PhoneOtp phoneOtp)
		{
			_context.PhoneOTPs.Add(phoneOtp);
			await _context.SaveChangesAsync();
		}

		public async Task RequestNewOTP(PhoneOtp phoneOtp)
		{
			phoneOtp.OTPCode = _codeService.GenerateOTP();
			phoneOtp.CreatedAt = DateTime.UtcNow;

			_context.PhoneOTPs.Update(phoneOtp);
			await _context.SaveChangesAsync();
		}

		public async Task<PhoneOtp?> GetSimilarPhoneNumber(string phoneNumber)
		{
			var samePhoneNumber = await _context.PhoneOTPs.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

			return samePhoneNumber;
		}

		public async Task<PhoneOtp?> VerifyPhoneOTP(PhoneOtpDto phoneOTPdto)
		{
			var verifiedPhoneOTP = await _context.PhoneOTPs.FirstOrDefaultAsync(p =>
									 p.PhoneNumber == phoneOTPdto.PhoneNumber &&
									 p.OTPCode == phoneOTPdto.OTPCode
									 );

			return verifiedPhoneOTP;
		}
	}
}
