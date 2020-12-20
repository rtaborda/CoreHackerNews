using CoreHackerNews.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreHackerNews
{
    public class NewsLogic : INewsLogic
    {
        private readonly IMemoryCache _cache;
        private readonly IHackerNewsClient _hackerNewsClient;
        private readonly NewsConfiguration _configuration;

        public NewsLogic(
            IMemoryCache cache,
            IHackerNewsClient hackerNewsClient,
            NewsConfiguration configuration)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _hackerNewsClient = hackerNewsClient ?? throw new ArgumentNullException(nameof(hackerNewsClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (!_configuration.IsValid)
                throw new ArgumentException($"Invalid {nameof(configuration)}");
        }

        public async Task<ICollection<NewsItem>> GetBestStoriesAsync()
        {
            return await _cache.GetOrCreateAsync(_configuration.CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_configuration.CacheExpirationInSeconds);
                return await GetAsync();
            });

            async Task<List<NewsItem>> GetAsync()
            {
                var bestStoryIds = await _hackerNewsClient.GetBestStoriesIdsAsync();
                if (bestStoryIds == null || !bestStoryIds.Any())
                    return null;

                var topCountStoryIds = bestStoryIds.Take(_configuration.BestStoriesSize);
                var results = new List<NewsItem>(_configuration.BestStoriesSize);

                foreach (var storyId in topCountStoryIds)
                {
                    var storyDetails = await _hackerNewsClient.GetStoryDetailsAsync(storyId);
                    if (storyDetails == null)
                        continue;

                    results.Add(storyDetails);
                }

                return results.OrderByDescending(r => r.Score).ToList();
            }
        }
    }
}
