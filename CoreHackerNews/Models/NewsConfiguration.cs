using System;

namespace CoreHackerNews.Models
{
    public class NewsConfiguration
    {
        public string CacheKey { get; set; }

        public ushort CacheExpirationInSeconds { get; set; }

        public ushort BestStoriesSize { get; set; }

        public Uri HackerNewsApiUrl { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(CacheKey) && BestStoriesSize > 0 &&
            HackerNewsApiUrl != null;
    }
}
