namespace eshop.Application.Contracts.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(Guid userId);
        Task<bool> ValidateOtpAsync(Guid userId, string otp);
        Task<string> GenerateNewOtpAsync(Guid userId);
        Task DeleteTokenByUserIdAsync(Guid userId);
    }
}