namespace PhotoFolder.Infrastructure
{
    public class InfrastructureOptions
    {
        public float RequiredSimilarityForEquality { get; set; } = 0.97f;
        public float RequiredSimilarityForIssue { get; set; } = 0.95f;
        public int LargeFileMargin { get; set; } = 1024 * 1024 * 50; // 100 MiB
    }
}
