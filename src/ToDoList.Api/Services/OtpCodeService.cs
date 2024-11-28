namespace ToDoList.Api.Services
{
	public class OtpCodeService : IOtpCodeService
	{
		public string GenerateOTP()
		{
			string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string otp = "";
			
			Random random = new Random();

			for (int i = 0; i < 6;  i++)
			{
				char otpChar = characters[random.Next(characters.Length)];
				otp += otpChar;
			}
			return otp;
		}

		public bool CheckOTPExpiry(DateTime otpDateCreated)
		{
			TimeSpan timeDifference = DateTime.UtcNow - otpDateCreated;
			int minutesPassed = (int)timeDifference.TotalMinutes;

			if (minutesPassed >= 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
