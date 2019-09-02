﻿namespace PhotoFolder.Infrastructure
{
    public class InfrastructureOptions
    {
        public float RequiredSimilarityForEquality { get; set; } = 0.97f;
        public float RequiredSimilarityForIssue { get; set; } = 0.95f;
        public long LargeFileMargin { get; set; } = 1024 * 1024 * 100; // 100 MiB
    }
}
