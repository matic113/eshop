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
        private readonly IEmailService _emailService;

        public OtpService(IVerificationTokensRepository tokensRepository, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _tokensRepository = tokensRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateOtpAsync(Guid userId)
        {
            var otp = RandomNumberGenerator.GetInt32(0, 999999);
            var otpString =  otp.ToString("D6");

            var verificationToken = VerificationToken.Create(userId, otpString, 10);

            await _tokensRepository.AddAsync(verificationToken);
            await _unitOfWork.SaveChangesAsync();

            return otpString;
        }
        public async Task SendOtpEmailAsync(string otp, string email)
        {
            var message = "Thank you for registering with us! " +
                          "Please use the following OTP to complete your verify you email: " + otp;

            await _emailService.SendEmailAsync(email, "Email Verification: Eshop", message);
            return;
        }
        public async Task SendResetPasswordOtpEmailAsync(string otp, string email)
        {
            var message = "We received a request for a password change on your eshop account " +
                          "if it was you please use the following otp to change your password " + otp;

            await _emailService.SendEmailAsync(email, "Password Reset: Eshop", message);
            return;
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

        public async Task GenerateAndSendNewOtpAsync(Guid userId, string email)
        {
            // 1. Delete Old Tokens
            await DeleteTokenByUserIdAsync(userId);

            // 2. Generate New OTP
            var otp = await GenerateOtpAsync(userId);
            await _unitOfWork.SaveChangesAsync();

            // 3. Send OTP Email
            await SendOtpEmailAsync(otp, email);
            return;
        }
    }
}
