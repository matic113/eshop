namespace eshop.Application.Contracts
{
    public interface IFileService
    {
        Task<string> GeneratePresignedUrlAsync(string fileName, int expiresAfterM, string contentType);
    }
}
