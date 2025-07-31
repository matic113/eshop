using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using eshop.Application.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eshop.Infrastructure.ObjectStorage
{
    public class CloudflareFileService : IFileService
    {
        private readonly IAmazonS3 _amazonS3Client;
        private readonly R2Options _r2Options;
        private readonly ILogger<CloudflareFileService> _logger;

        public CloudflareFileService(IAmazonS3 amazonS3Client,
            IOptions<R2Options> options,
            ILogger<CloudflareFileService> logger)
        {
            _amazonS3Client = amazonS3Client;
            _r2Options = options.Value;
            _logger = logger;
        }

        public async Task<string> GeneratePresignedUrlAsync(string fileName ,int expiresAfterM, string contentType = "image/jpeg")
        {
            var objectKey = fileName;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _r2Options.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expiresAfterM)
            };

            try
            {
                var url = await Task.Run(() => _amazonS3Client.GetPreSignedURL(request));
                return url;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                return "";
            }
        }
    }
}
