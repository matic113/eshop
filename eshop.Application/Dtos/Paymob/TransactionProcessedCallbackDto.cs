using System.Text.Json.Serialization;

namespace eshop.Application.Dtos.Paymob
{
    public sealed class TransactionProcessedCallbackDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("obj")]
        public TransactionObjectDto Obj { get; set; }
    }
}
