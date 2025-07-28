using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public record CreatePaymentIntentResponse(string ClientSecret, string UnifiedChechoutUrl);
    public interface IPaymobService
    {
        Task<CreatePaymentIntentResponse?> CreatePaymentIntentAsync(Order order);
    }
}
