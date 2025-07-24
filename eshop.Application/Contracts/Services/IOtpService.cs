namespace eshop.Application.Contracts.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync (Guid userId);
        Task SendOtpEmailAsync(string otp, string email);
        Task SendResetPasswordOtpEmailAsync(string otp, string email);
        Task<bool> ValidateOtpAsync(Guid userId, string otp);
        Task GenerateAndSendNewOtpAsync(Guid userId, string email);
        Task DeleteTokenByUserIdAsync(Guid userId);
    }
}