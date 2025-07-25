using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Email.ViewModels;
using FluentEmail.Core;

namespace eshop.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }

        public async Task SendEmailUsingTemplateAsync(string to, string subject, string templateName, object viewModel)
        {
            // Get the path relative to the application's base directory
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email", "Templates", templateName);

            await _fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromFile(templatePath, viewModel)
                .SendAsync();
        }

        public async Task SendOtpEmailAsync(string email, string otp, string userName)
        {
            var viewModel = new OtpViewModel
            {
                FirstName = userName,
                Otp = otp
            };

            await SendEmailUsingTemplateAsync(email, "Email Verification: Eshop", "SignUpOtp.cshtml", viewModel);
        }
    }
}
