namespace eshop.Infrastructure.ObjectStorage
{
    public class R2Options
    {
        public const string SectionName = "CloudflareR2";
        public string AccountId { get; set; }
        public string BucketName { get; set; }
        public string PublicDomain { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
