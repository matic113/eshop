using Microsoft.AspNetCore.Identity;

namespace eshop.Infrastructure.Persistence
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // IdentityUser has PhoneNumber
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }

        public static ApplicationUser Create(string email, string firstName, string lastName)
        {
            return new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName
            };
        }
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
