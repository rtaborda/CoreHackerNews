using CoreHackerNews.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreHackerNews
{
    public interface IHackerNewsClient : IDisposable
    {
        Task<ICollection<int>> GetBestStoriesIdsAsync();

        Task<NewsItem> GetStoryDetailsAsync(int storyId);
    }
}
