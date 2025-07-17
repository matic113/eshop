namespace eshop.Domain.Entities
{
    public class VerificationToken : IBaseEntity
    {
        private VerificationToken(Guid userId, string code, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Code = code;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Checks if the token has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        /// <summary>
        /// Factory method to create a new VerificationToken.
        /// This encapsulates the logic for creating a token and setting its expiration.
        /// </summary>
        /// <param name="userId">The ID of the user this token belongs to.</param>
        /// <param name="code">The 6-digit verification code.</param>
        /// <param name="expiresInMinutes">The duration for which the token is valid.</param>
        /// <returns>A new VerificationToken instance.</returns>
        public static VerificationToken Create(Guid userId, string code, int expiresInMinutes = 10)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 6)
            {
                throw new ArgumentException("Verification code must be 6 characters long.", nameof(code));
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);
            return new VerificationToken(userId, code, expiresAt);
        }
    }
}
