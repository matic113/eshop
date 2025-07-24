using System.Text.Json.Serialization;

namespace eshop.Application.Dtos.Paymob
{
    public sealed class SourceDataDto
    {
        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("sub_type")]
        public string SubType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
