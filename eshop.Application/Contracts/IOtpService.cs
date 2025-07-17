namespace eshop.Application.Contracts
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync (Guid userId);
        Task SendOtpEmailAsync(string otp, string email);
    }
}
