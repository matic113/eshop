using eshop.Application.Contracts;
using eshop.Application.Contracts.Services;
using eshop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eshop.Application
{
    public static class DependecyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IPublicCodeGenerator, PublicCodeGenerator>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IPaymobWebhookService, PaymobWebhookService>();
        }
    }
}
