namespace eshop.Application.Contracts
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync (Guid userId);
        Task SendOtpEmailAsync(string otp, string email);
        Task SendResetPasswordOtpEmailAsync(string otp, string email);
        Task<bool> ValidateOtpAsync(Guid userId, string otp);
        Task DeleteTokenByUserIdAsync(Guid userId);
    }
}