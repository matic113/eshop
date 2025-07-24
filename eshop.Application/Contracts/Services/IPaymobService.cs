using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public record CreatePaymentIntentResponse(string ClientSecret, string PaymentKey);
    public interface IPaymobService
    {
        Task<CreatePaymentIntentResponse?> CreatePaymentIntentAsync(Order order);
    }
}
