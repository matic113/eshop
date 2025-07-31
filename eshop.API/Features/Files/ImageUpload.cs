using eshop.Application.Contracts;
using eshop.Infrastructure.ObjectStorage;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace eshop.API.Features.Files
{
    public class ImageUpload
    {
        sealed class GetImageUploadUrlResponse
        {
            public required string UploadUrl { get; set; }
            public required string PublicImageUrl { get; set; }
        }

        sealed class GetImageUploadUrlEndpoint : EndpointWithoutRequest<GetImageUploadUrlResponse>
        {
            private readonly IFileService _fileService;
            private readonly R2Options _r2Options;

            public GetImageUploadUrlEndpoint(IFileService fileService, IOptions<R2Options> r2Options)
            {
                _fileService = fileService;
                _r2Options = r2Options.Value;
            }

            public override void Configure()
            {
                Get("/api/files/image");
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString("N") + ".jpg"; 

                var presignedUrl = await _fileService
                    .GeneratePresignedUrlAsync(fileName, 10, "image/jpeg");

                var publicImageUrl = $"{_r2Options.PublicDomain}/{fileName}";

                if (string.IsNullOrEmpty(presignedUrl))
                {
                    await SendNotFoundAsync(c);
                    return;
                }

                var response = new GetImageUploadUrlResponse
                {
                    UploadUrl = presignedUrl,
                    PublicImageUrl = publicImageUrl
                };
                await SendOkAsync(response, c);
            }
        }
    }
}
