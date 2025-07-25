using System.Security.Cryptography;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Domain.Entities;

namespace eshop.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IVerificationTokensRepository _tokensRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OtpService(IVerificationTokensRepository tokensRepository, IUnitOfWork unitOfWork)
        {
            _tokensRepository = tokensRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateOtpAsync(Guid userId)
        {
            var otp = RandomNumberGenerator.GetInt32(0, 999999);
            var otpString = otp.ToString("D6");

            var verificationToken = VerificationToken.Create(userId, otpString, 10);

            await _tokensRepository.AddAsync(verificationToken);
            await _unitOfWork.SaveChangesAsync();

            return otpString;
        }

        public async Task<bool> ValidateOtpAsync(Guid userId, string otp)
        {
            var token = await _tokensRepository.GetByUserIdAndCodeAsync(userId, otp);

            if (token is null || token.IsExpired)
            {
                return false;
            }

            return true;
        }
        public async Task DeleteTokenByUserIdAsync(Guid userId)
        {
            await _tokensRepository.DeleteUserTokensAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> GenerateNewOtpAsync(Guid userId)
        {
            // 1. Delete Old Tokens
            await DeleteTokenByUserIdAsync(userId);

            // 2. Generate New OTP
            var otp = await GenerateOtpAsync(userId);
            await _unitOfWork.SaveChangesAsync();

            return otp;
        }
    }
}
