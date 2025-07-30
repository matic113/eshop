using eshop.Application.Contracts;
using eshop.Application.Dtos.Paymob;
using FastEndpoints;

namespace eshop.API.Features.Webhooks
{
    public class Paymob
    {
        sealed class PaymobWebhookEndpoint : Endpoint<TransactionProcessedCallbackDto>
        {
            private readonly IPaymobWebhookService _paymobWebhookService;

            public PaymobWebhookEndpoint(IPaymobWebhookService paymobWebhookService)
            {
                _paymobWebhookService = paymobWebhookService;
            }

            public override void Configure()
            {
                Post("/api/webhooks/paymob");
                AllowAnonymous();
                Description(x =>
                {
                    x.WithTags("Webhooks");
                });
            }

            public override async Task HandleAsync(TransactionProcessedCallbackDto r, CancellationToken c)
            {
                var hmac = HttpContext.Request.Query["hmac"].ToString();

                if (string.IsNullOrEmpty(hmac))
                {
                    await SendForbiddenAsync();
                    return;
                }

                var result = await _paymobWebhookService.ProcessTransactionAsync(r, hmac);

                if (result.IsError && result.FirstError.Type == ErrorOr.ErrorType.Unauthorized)
                {
                    await SendForbiddenAsync();
                    return;
                }

                await SendOkAsync();
            }
        }
    }
}
