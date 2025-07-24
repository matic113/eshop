
using eshop.Application.Dtos.Paymob;

namespace eshop.Application.Contracts
{
    public interface IPaymobHmacValidator
    {
        bool IsValid(TransactionObjectDto transaction, string receivedHmac);
    }
}
