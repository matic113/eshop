using eshop.Application.Contracts;

namespace eshop.Application.Services
{
    public class PublicCodeGenerator : IPublicCodeGenerator
    {
        public string Generate(string prefix)
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

            // Taking the first 6 characters of a new GUID
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpper();

            return $"{prefix}-{datePart}-{randomPart}";
        }
    }
}
