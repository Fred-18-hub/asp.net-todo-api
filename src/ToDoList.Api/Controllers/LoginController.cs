using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Services;
using ToDoList.Api.Repositories.Db;

namespace ToDoList.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly IUserDbRepository _userDbRepository;
		private readonly IPhoneOtpDbRepository _otpDbRepository;
		private readonly ILoginService _loginService;
		private readonly IOtpCodeService _otpCodeService;

		public LoginController(
            IUserDbRepository userDbRepository,
			IPhoneOtpDbRepository otpDbRepository,
			ILoginService loginService,
			IOtpCodeService otpCodeService
			)
        {
			_userDbRepository = userDbRepository;
			_otpDbRepository = otpDbRepository;
			_loginService = loginService;
			_otpCodeService = otpCodeService;
		}

		[HttpPost]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Login([FromBody] UserDto userDto)
		{
			var validUser = await _userDbRepository.ValidateUser(userDto);

			if (validUser == null)
			{
				return Unauthorized("Invalid Login!");
			}

			var token = _loginService.GenerateToken();

			return Ok($"Token: {token}");
		}

		[HttpGet("GenerateOTP")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GenerateOTP([FromQuery] string phone)
		{
			var validIsValid = _loginService.ValidatePhoneNumber(phone);

			if (!validIsValid)
			{
				return BadRequest(new { Error = "Phone number must be 10 digits with no special characters!" });
			}
			
			var phoneOTPWithSamePhoneNumber = await _otpDbRepository.GetSimilarPhoneNumber(phone);
			
			if (phoneOTPWithSamePhoneNumber == null)
			{
				var newPhoneOtp = new PhoneOtp
				{
					PhoneNumber = phone,
					OTPCode = _otpCodeService.GenerateOTP(),
					CreatedAt = DateTime.UtcNow
				};
				await _otpDbRepository.AddNewPhoneOTP(newPhoneOtp);
				
				return Ok($"OTP: {newPhoneOtp.OTPCode}");
			}

			var otpExpired = _otpCodeService.CheckOTPExpiry(phoneOTPWithSamePhoneNumber.CreatedAt);

			if (otpExpired)
			{
				await _otpDbRepository.RequestNewOTP(phoneOTPWithSamePhoneNumber);
				
				return Ok($"New OTP: {phoneOTPWithSamePhoneNumber.OTPCode}");
			}
			else { return Ok($"Use previously generated OTP [{phoneOTPWithSamePhoneNumber.OTPCode}] as it hasn't expired."); }
		}

		[HttpPost("ByPhone")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> LoginByPhone([FromQuery] PhoneOtpDto phoneOTPdto)
		{
			var validIsValid = _loginService.ValidatePhoneNumber(phoneOTPdto.PhoneNumber);

			if (!validIsValid)
			{
				return BadRequest(new { Error = "Phone number must be 10 digits with no special characters!" });
			}

			var verifiedPhoneOTP = await _otpDbRepository.VerifyPhoneOTP(phoneOTPdto);

			if (verifiedPhoneOTP == null)
			{ 
				return Unauthorized("Invalid OTP. Generate a new one for this number");
			}

			var otpExpired = _otpCodeService.CheckOTPExpiry(verifiedPhoneOTP.CreatedAt);

			if (otpExpired)
			{
				return Unauthorized($"OTP has expired. Generate new one");
			}

			var token = _loginService.GenerateToken();

			return Ok($"Token: {token}");
		}
	}
}
