namespace eshop.Infrastructure.Payment
{
    public class PaymobOptions
    {
        public const string PaymobOptionsKey = "PaymobOptions";
        public required string ApiKey { get; set; }
        public required string SecretKey { get; set; }
        public required string PublicKey { get; set; }
        public required int CardIntegrationId { get; set; }
    }
}
