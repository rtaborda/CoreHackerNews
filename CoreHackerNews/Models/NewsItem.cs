using System;
using System.Collections.Generic;

namespace CoreHackerNews.Models
{
    public class NewsItem
    {
        public int Id { get; set; }

        public bool Deleted { get; set; }

        public NewsItemType Type { get; set; }

        public string By { get; set; }

        public long UnixTime { get; set; }

        public string Text { get; set; }

        public bool Dead { get; set; }

        public int Parent { get; set; }

        public int Pool { get; set; }

        public IEnumerable<int> Kids { get; set; }

        public Uri Url { get; set; }

        public uint Score { get; set; }

        public string Title { get; set; }

        public IEnumerable<int> Parts { get; set; }

        public uint Descendants { get; set; }
    }
}
