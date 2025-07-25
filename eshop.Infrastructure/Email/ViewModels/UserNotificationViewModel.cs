namespace eshop.Infrastructure.Email.ViewModels
{
    public class UserNotificationViewModel
    {
        public required string FirstName { get; set; }
        public string? Otp { get; set; } // Optional - only used when OTP is needed
    }
}
