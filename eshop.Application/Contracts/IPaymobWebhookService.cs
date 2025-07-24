using ErrorOr;
using eshop.Application.Dtos.Paymob;

namespace eshop.Application.Contracts
{
    public interface IPaymobWebhookService
    {
        Task<ErrorOr<Success>> ProcessTransactionAsync(TransactionProcessedCallbackDto transactionDto,
            string recievedHmac);
    }
}
