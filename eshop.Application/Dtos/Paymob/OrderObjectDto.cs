using System.Text.Json.Serialization;

namespace eshop.Application.Dtos.Paymob
{
    public sealed class OrderObjectDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("merchant_order_id")]
        public string MerchantOrderId { get; set; } // Our OrderId
    }
}
