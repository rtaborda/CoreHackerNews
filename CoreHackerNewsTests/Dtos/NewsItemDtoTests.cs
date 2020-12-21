using CoreHackerNews.Dtos;
using CoreHackerNews.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using Xunit;

namespace CoreHackerNewsTests.Dtos
{
    public class NewsItemDtoTests
    {
        [Fact]
        public void Constructor_GivenNullNewsItem_ThrowsArgumentNullException()
        {
            // Act
            var result = Record.Exception(() => new NewsItemDto(null));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<ArgumentNullException>();
                assertionResult.Subject.Message.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void Constructor_GivenNewsItem_ReturnsExpectedResult()
        {
            // Arrange
            var newsItem = new NewsItem
            {
                By = "Test User",
                Dead = false,
                Deleted = false,
                Descendants = 100,
                Id = 1234,
                Kids = new[] { 9, 8, 7 },
                Parent = 999,
                Parts = new[] { 6, 5 },
                Pool = 947,
                Score = 1000,
                Text = "News Article Text",
                Title = "News Article",
                Type = NewsItemType.Story,
                UnixTime = 1608580776,
                Url = new Uri("https://example.com/news/article/1234")
            };

            // Act
            var result = new NewsItemDto(newsItem);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.CommentCount.Should().Be(newsItem.Descendants);
                result.PostedBy.Should().Be(newsItem.By);
                result.Score.Should().Be(newsItem.Score);
                result.Time.Should().Be(DateTimeOffset.FromUnixTimeSeconds(newsItem.UnixTime));
                result.Title.Should().Be(newsItem.Title);
                result.Uri.Should().Be(newsItem.Url);
            }
        }
    }
}
