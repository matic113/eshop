using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using eshop.Application.Contracts.Services;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eshop.Infrastructure.Payment
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobOptions _paymobOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PaymobService> _logger;

        public PaymobService(HttpClient httpClient,
            IOptions<PaymobOptions> options,
            ILogger<PaymobService> logger,
            UserManager<ApplicationUser> userManager)
        {
            _httpClient = httpClient;
            _paymobOptions = options.Value;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new Payment Intent with Paymob using their modern, single-call API.
        /// This is the recommended approach.
        /// </summary>
        /// <param name="order">The order object from your domain.</param>
        /// <returns>The client_secret to be used by the Flutter SDK, or null on failure.</returns>
        public async Task<CreatePaymentIntentResponse?> CreatePaymentIntentAsync(Order order)
        {
            const string endpoint = "v1/intention/";

            var user = await _userManager.FindByIdAsync(order.UserId.ToString());

            // 1. Create the request payload from your order data.
            var request = new PaymentIntentRequest
            {
                Amount = (int)(order.TotalPrice * 100), // Convert to cents/piasters
                Currency = "EGP",
                PaymentMethods = [_paymobOptions.CardIntegrationId, _paymobOptions.WalletIntegrationId],
                SpecialReference = order.Id.ToString(), // Use the OrderId as the special reference

                Items = order.OrderItems.Select(oi => new ItemDto
                {
                    Name = oi.Product.Name,
                    Amount = (int)(oi.Price * 100),
                    Description = oi.Product.ProductCode,
                    Quantity = oi.Quantity,
                    Image = oi.Product.CoverPictureUrl
                }).ToList(),

                BillingData = new BillingDataDto
                {
                    FirstName = user?.FirstName ?? "NA",
                    LastName = user?.LastName ?? "NA",
                    Email = user?.Email ?? "NA",
                    PhoneNumber = order.ShippingAddress.PhoneNumber,
                    Street = order.ShippingAddress.Street,
                    Building = "NA",
                    Floor = "NA",
                    Apartment = order.ShippingAddress.Apartment,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    Country = "EGY"
                }
            };

            // 2. Create the HttpRequestMessage to add the Authorization header.
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", _paymobOptions.SecretKey);

            httpRequest.Content = JsonContent.Create(request);

            try
            {
                // 3. Send the request.
                HttpResponseMessage response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var intentResponse = await response.Content.ReadFromJsonAsync<PaymentIntentResponseDto>();

                var clientSecret = intentResponse?.ClientSecret;
                // Extract the first payment key from the array.
                var paymentKey = intentResponse?.PaymentKeys?.FirstOrDefault()?.Key;

                if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(paymentKey))
                {
                    _logger.LogError("Paymob payment intent created, but client_secret or payment_key was empty.");
                    return null;
                }

                _logger.LogInformation("Paymob payment intent created successfully for OrderNumber: {OrderNumber}", order.OrderNumber);

                // 4. Return the new response object with both keys.
                var unifiedCheckoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_paymobOptions.PublicKey}&clientSecret={clientSecret}";

                return new CreatePaymentIntentResponse(clientSecret, unifiedCheckoutUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create Paymob payment intent for OrderNumber: {OrderNumber}", order.OrderNumber);
                return null;
            }
        }

        #region DTOs for Payment Intent API

        private record PaymentIntentRequest
        {
            [JsonPropertyName("amount")]
            public int Amount { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            [JsonPropertyName("payment_methods")]
            public List<int> PaymentMethods { get; set; }

            [JsonPropertyName("special_reference")]
            public string SpecialReference { get; set; } // Our OrderId

            [JsonPropertyName("items")]
            public List<ItemDto> Items { get; set; }

            [JsonPropertyName("billing_data")]
            public BillingDataDto BillingData { get; set; }
        }

        private record ItemDto
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("amount")]
            public int Amount { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }
            [JsonPropertyName("image")]
            public string Image { get; set; }
        }

        private record BillingDataDto
        {
            [JsonPropertyName("first_name")]
            public string FirstName { get; set; }

            [JsonPropertyName("last_name")]
            public string LastName { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("phone_number")]
            public string PhoneNumber { get; set; }

            [JsonPropertyName("street")]
            public string Street { get; set; }

            [JsonPropertyName("building")]
            public string Building { get; set; }

            [JsonPropertyName("floor")]
            public string Floor { get; set; }

            [JsonPropertyName("apartment")]
            public string Apartment { get; set; }

            [JsonPropertyName("city")]
            public string City { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }
        }

        // Renamed to DTO to avoid confusion with the new public response record
        private record PaymentIntentResponseDto
        {
            [JsonPropertyName("client_secret")]
            public string ClientSecret { get; set; }

            // Added this property to capture the payment_keys array
            [JsonPropertyName("payment_keys")]
            public List<PaymentKeyDto> PaymentKeys { get; set; }
        }

        // A new record to represent the object inside the payment_keys array
        private record PaymentKeyDto
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
        }

        #endregion
    }
}
