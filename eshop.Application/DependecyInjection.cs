using eshop.Application.Contracts;
using eshop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eshop.Application
{
    public static class DependecyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOtpService, OtpService>();
        }
    }
}
