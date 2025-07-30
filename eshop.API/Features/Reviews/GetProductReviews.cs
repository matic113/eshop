using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using FastEndpoints;

namespace eshop.API.Features.Reviews
{
    public class GetProductReviews
    {
        sealed class GetProductReviewsRequest
        {
            public Guid ProductId { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 3;
        }

        sealed class GetProductReviewsResponse
        {
            public string? Message { get; set; }
            public PagedList<UserReviewDto> Reviews { get; set; } = null!;
        }

        sealed class GetProductReviewsEndpoint : Endpoint<GetProductReviewsRequest, GetProductReviewsResponse>
        {
            private readonly IReviewService _reviewService;

            public GetProductReviewsEndpoint(IReviewService reviewService)
            {
                _reviewService = reviewService;
            }

            public override void Configure()
            {
                Get("/api/reviews/{productId}");
                Description(x => x
                    .WithTags("Reviews")
                    .Produces<GetProductReviewsResponse>()
                    .WithDescription("This endpoint allows you to retrieve reviews for a specific product by its ID, with pagination support.")
                );
            }

            public override async Task HandleAsync(GetProductReviewsRequest r, CancellationToken c)
            {
                var result = await _reviewService.GetProductReviewsAsync(r.ProductId, r.Page, r.PageSize);

                if (result.IsError)
                {
                    var message = result.FirstError.Description;
                    await SendAsync(new GetProductReviewsResponse
                    {
                        Message = message
                    }, 400);

                    return;
                }

                var response = new GetProductReviewsResponse
                {
                    Message = "Reviews Retrieved Succesfully",
                    Reviews = result.Value
                };

                await SendOkAsync(response);
            }
        }
    }
}
