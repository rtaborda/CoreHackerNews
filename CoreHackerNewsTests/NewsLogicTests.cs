using CoreHackerNews;
using CoreHackerNews.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoreHackerNewsTests
{
    public class NewsLogicTests
    {
        private readonly Mock<IMemoryCache> _cache;
        private readonly Mock<IHackerNewsClient> _hackerNewsClient;
        private readonly NewsConfiguration _configuration;

        private NewsLogic Sut { get; }

        public NewsLogicTests()
        {
            _cache = new Mock<IMemoryCache>(MockBehavior.Strict);
            _hackerNewsClient = new Mock<IHackerNewsClient>(MockBehavior.Strict);
            _configuration = new NewsConfiguration
            {
                BestStoriesSize = 20,
                CacheExpirationInSeconds = 3,
                CacheKey = "Cache Key",
                HackerNewsApiUrl = new Uri("https://hacker-news.firebaseio.com/v0")
            };

            Sut = new NewsLogic(_cache.Object, _hackerNewsClient.Object, _configuration);
        }

        public static readonly TheoryData<IMemoryCache, IHackerNewsClient, NewsConfiguration>
            ConstructorNullParamsTestData = new TheoryData<IMemoryCache, IHackerNewsClient, NewsConfiguration>
            {
                { null, new Mock<IHackerNewsClient>(MockBehavior.Loose).Object, new NewsConfiguration() },
                { new Mock<IMemoryCache>(MockBehavior.Loose).Object, null, new NewsConfiguration() },
                { new Mock<IMemoryCache>(MockBehavior.Loose).Object, new Mock<IHackerNewsClient>(MockBehavior.Loose).Object, null }
            };

        [Theory]
        [MemberData(nameof(ConstructorNullParamsTestData))]
        public void Constructor_GivenNullParam_ThrowsArgumentNullException(
            IMemoryCache cache, IHackerNewsClient hackerNewsClient, NewsConfiguration configuration)
        {
            // Act
            var result = Record.Exception(() => new NewsLogic(cache, hackerNewsClient, configuration));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<ArgumentNullException>();
                assertionResult.Subject.Message.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void Constructor_GivenInvalidConfiguration_ThrowsArgumentException()
        {
            // Arrange
            var configuration = new NewsConfiguration();

            // Act
            var result = Record.Exception(() => new NewsLogic(_cache.Object, _hackerNewsClient.Object, configuration));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<ArgumentException>();
                assertionResult.Subject.Message.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async void GetBestStoriesAsync_Respects_CacheHitsAndMisses()
        {
            // Arrange
            var storyIds = new[] { 1, 2 };
            var newsItem1 = new NewsItem { Score = 10 };
            var newsItem2 = new NewsItem { Score = 5000 };

            var newsItems = new List<NewsItem>(2) { newsItem2, newsItem1 };

            _hackerNewsClient.Setup(m => m.GetBestStoriesIdsAsync())
                .ReturnsAsync(storyIds);

            _hackerNewsClient.Setup(m => m.GetStoryDetailsAsync(storyIds[0]))
                .ReturnsAsync(newsItem1);

            _hackerNewsClient.Setup(m => m.GetStoryDetailsAsync(storyIds[1]))
                .ReturnsAsync(newsItem2);

            var cacheEntry = new Mock<ICacheEntry>(MockBehavior.Loose);

            object cachedValue = newsItems;

            _cache.SetupSequence(m => m.TryGetValue(It.IsAny<object>(), out cachedValue))
                .Returns(false)
                .Returns(true)
                .Returns(false);

            _cache.Setup(m => m.CreateEntry(_configuration.CacheKey))
                .Returns(cacheEntry.Object);

            // Act
            var result1 = await Sut.GetBestStoriesAsync();
            cachedValue = result1;
            var result2 = await Sut.GetBestStoriesAsync();
            var result3 = await Sut.GetBestStoriesAsync();

            // Assert
            using (new AssertionScope())
            {
                result1.Should().BeEquivalentTo(newsItems);
                result2.Should().BeEquivalentTo(result1);
                result3.Should().BeEquivalentTo(result1);
            }

            _hackerNewsClient.Verify(m => m.GetBestStoriesIdsAsync(), Times.Exactly(2));
            _hackerNewsClient.Verify(m => m.GetStoryDetailsAsync(storyIds[0]), Times.Exactly(2));
            _hackerNewsClient.Verify(m => m.GetStoryDetailsAsync(storyIds[1]), Times.Exactly(2));
            _cache.Verify(m => m.TryGetValue(It.IsAny<object>(), out cachedValue), Times.Exactly(3));
            _cache.Verify(m => m.CreateEntry(_configuration.CacheKey), Times.Exactly(2));
        }
    }
}
