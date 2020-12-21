using System;

namespace CoreHackerNews.Models
{
    public class NewsConfiguration
    {
        public string CacheKey { get; set; }

        public ushort CacheExpirationInSeconds { get; set; }

        public ushort BestStoriesSize { get; set; }

        public Uri HackerNewsApiUrl { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(CacheKey) &&
                               CacheExpirationInSeconds > 0 &&
                               BestStoriesSize > 0 &&
                               HackerNewsApiUrl != null;
    }
}
