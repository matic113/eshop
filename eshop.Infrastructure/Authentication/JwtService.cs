using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using eshop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eshop.Infrastructure.Authentication
{
    public class JwtService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager)
        {
            _jwtOptions = options.Value;
            _userManager = userManager;
        }

        public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(ApplicationUser user)
        {
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var userRoles = _userManager.GetRolesAsync(user).Result;

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.ToString()),
            new Claim("roles", string.Join(",", userRoles)),
            new Claim("picture", user.ProfilePicture ?? ""),
        };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtToken, expires);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
