namespace eshop.Infrastructure.Payment
{
    public class PaymobOptions
    {
        public const string PaymobOptionsKey = "PaymobOptions";
        public required string ApiKey { get; set; }
        public required string SecretKey { get; set; }
        public required string PublicKey { get; set; }
        public required string HMAC { get; set; }
        public required int CardIntegrationId { get; set; }
        public required int WalletIntegrationId { get; set; }
        public required string DiscountImageUrl { get; set; }
        public required string ShippingImageUrl { get; set; }
    }
}
