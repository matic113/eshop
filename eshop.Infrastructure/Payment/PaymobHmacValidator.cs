using System.Security.Cryptography;
using System.Text;
using eshop.Application.Contracts;
using eshop.Application.Dtos.Paymob;
using Microsoft.Extensions.Options;

namespace eshop.Infrastructure.Payment
{
    public class PaymobHmacValidator : IPaymobHmacValidator
    {
        private readonly string _hmacSecret;
        public PaymobHmacValidator(IOptions<PaymobOptions> paymobOptions)
        {
            _hmacSecret = paymobOptions.Value.HMAC ??
                throw new ArgumentNullException("Paymob HMAC Secret not configured.");
        }

        public bool IsValid(TransactionObjectDto transaction, string receivedHmac)
        {
            // 1. Concatenate the required fields in the exact specified order.
            var concatenatedString =
                $"{transaction.AmountCents}" +
                $"{transaction.CreatedAt}" +
                $"{transaction.Currency}" +
                $"{transaction.ErrorOccured.ToString().ToLower()}" +
                $"{transaction.HasParentTransaction.ToString().ToLower()}" +
                $"{transaction.Id}" +
                $"{transaction.IntegrationId}" +
                $"{transaction.Is3dSecure.ToString().ToLower()}" +
                $"{transaction.IsAuth.ToString().ToLower()}" +
                $"{transaction.IsCapture.ToString().ToLower()}" +
                $"{transaction.IsRefunded.ToString().ToLower()}" +
                $"{transaction.IsStandalonePayment.ToString().ToLower()}" +
                $"{transaction.IsVoided.ToString().ToLower()}" +
                $"{transaction.Order.Id}" +
                $"{transaction.Owner}" +
                $"{transaction.Pending.ToString().ToLower()}" +
                $"{transaction.SourceData.Pan}" +
                $"{transaction.SourceData.SubType}" +
                $"{transaction.SourceData.Type}" +
                $"{transaction.Success.ToString().ToLower()}";

            // 2. Compute the SHA512 HMAC of the concatenated string.
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));

            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            var calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            // 3. Compare the calculated HMAC with the received HMAC from Paymob.
            return calculatedHmac == receivedHmac;
        }
    }
}
