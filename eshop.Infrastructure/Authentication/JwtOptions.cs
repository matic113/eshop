namespace eshop.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public const string JwtOptionsKey = "JwtOptions";
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}
