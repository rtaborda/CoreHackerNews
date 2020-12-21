using CoreHackerNews.Models;
using FluentAssertions;
using System;
using Xunit;

namespace CoreHackerNewsTests.Models
{
    public class NewsConfigurationTests
    {
        public static readonly TheoryData<ushort, ushort, string, Uri> InvalidStateTestData =
            new TheoryData<ushort, ushort, string, Uri>
            {
                { 0, 10, "cache key", new Uri("https://example.com/hacker-api") },
                { 10, 0, "cache key", new Uri("https://example.com/hacker-api") },
                { 10, 10, null, new Uri("https://example.com/hacker-api") },
                { 10, 10, "cache key", null }
            };

        [Theory]
        [MemberData(nameof(InvalidStateTestData))]
        public void IsValid_GivenInvalidState_ReturnsFalse(
            ushort bestStoriesSize,
            ushort cacheExpirationInSeconds,
            string cacheKey,
            Uri hackerNewsApiUrl)
        {
            // Act
            var sut = new NewsConfiguration
            {
                BestStoriesSize = bestStoriesSize,
                CacheExpirationInSeconds = cacheExpirationInSeconds,
                CacheKey = cacheKey,
                HackerNewsApiUrl = hackerNewsApiUrl
            };

            // Assert
            sut.IsValid.Should().BeFalse();
        }

        [Fact]
        public void IsValid_GivenInvalidState_ReturnsTrue()
        {
            // Act
            var sut = new NewsConfiguration
            {
                BestStoriesSize = 20,
                CacheExpirationInSeconds = 60,
                CacheKey = "Cache Key",
                HackerNewsApiUrl = new Uri("https://hacker-news.firebaseio.com/v0")
            };

            // Assert
            sut.IsValid.Should().BeTrue();
        }
    }
}
