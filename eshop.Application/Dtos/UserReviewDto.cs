namespace eshop.Application.Dtos
{
    public class UserReviewDto
    {
        public required string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserName { get; set; }
        public string? UserPicture { get; set; }

    }
}
