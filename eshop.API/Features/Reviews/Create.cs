using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Extensions;
using FastEndpoints;
using FluentValidation;

namespace eshop.API.Features.Reviews
{
    public class Create
    {
        sealed class CreateReviewRequest
        {
            public Guid ProductId { get; set; }
            public string? Comment { get; set; }
            public int Rating { get; set; }
        }
        sealed class CreateReviewRequestValidator : Validator<CreateReviewRequest>
        {
            public CreateReviewRequestValidator()
            {
                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product ID is required.");
                RuleFor(x => x.Comment)
                    .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
                RuleFor(x => x.Rating)
                    .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            }
        }
        sealed class CreateReviewResponse
        {
            public string Message { get; set; } = "";
            public Guid? Id { get; set; }
            public Guid? ProductId { get; set; }
            public string? Comment { get; set; }
            public int Rating { get; set; }
        }

        sealed class CreateReviewEndpoint : Endpoint<CreateReviewRequest, CreateReviewResponse>
        {
            private readonly IReviewService _reviewService;

            public CreateReviewEndpoint(IReviewService reviewService)
            {
                _reviewService = reviewService;
            }

            public override void Configure()
            {
                Post("/api/reviews/{productId}");
            }

            public override async Task HandleAsync(CreateReviewRequest r, CancellationToken c)
            {
                var userId = User.GetUserId();

                if (userId is null)
                {
                    await SendUnauthorizedAsync();
                    return;
                }

                var result = await _reviewService.AddProductReviewAsync(
                    userId.Value,
                    r.ProductId,
                    r.Comment,
                    r.Rating);

                if (result.IsError)
                {
                    var error = result.FirstError.Description;
                    var badResponse = new CreateReviewResponse
                    {
                        Message = error
                    };
                    await SendAsync(badResponse, StatusCodes.Status400BadRequest);
                    return;
                }

                var review = result.Value;
                var response = new CreateReviewResponse
                {
                    Message = "Review created successfully.",
                    Id = review.ReviewId,
                    ProductId = review.ProductId,
                    Comment = review.Comment,
                    Rating = review.Rating
                };

                await SendOkAsync(response);
            }
        }
    }
}
