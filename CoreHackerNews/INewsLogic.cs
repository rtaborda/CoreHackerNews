using CoreHackerNews.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreHackerNews
{
    public interface INewsLogic
    {
        Task<ICollection<NewsItem>> GetBestStoriesAsync();
    }
}
