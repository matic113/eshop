namespace eshop.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailUsingTemplateAsync(string to, string subject, string templateName, object viewModel);
        Task SendOtpEmailAsync(string email, string otp, string userName);
    }
}
