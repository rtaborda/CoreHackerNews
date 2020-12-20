using CoreHackerNews.Models;
using System;

namespace CoreHackerNews.Dtos
{
    public class NewsItemDto
    {
        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string PostedBy { get; set; }

        public DateTimeOffset Time { get; set; }

        public uint Score { get; set; }

        public uint CommentCount { get; set; }

        public NewsItemDto(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            Title = newsItem.Title;
            Uri = newsItem.Url;
            PostedBy = newsItem.By;
            Time = DateTimeOffset.FromUnixTimeSeconds(newsItem.UnixTime);
            Score = newsItem.Score;
            CommentCount = newsItem.Descendants;
        }
    }
}
