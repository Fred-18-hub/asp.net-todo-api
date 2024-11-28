namespace ToDoList.Api.Models
{
	public class PhoneOtp
	{
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string OTPCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
