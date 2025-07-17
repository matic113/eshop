namespace eshop.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public const string JwtOptionsKey = "JwtOptions";
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}
