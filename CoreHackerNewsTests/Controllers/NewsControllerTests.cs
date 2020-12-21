using CoreHackerNews;
using CoreHackerNews.Controllers;
using CoreHackerNews.Dtos;
using CoreHackerNews.Exceptions;
using CoreHackerNews.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace CoreHackerNewsTests.Controllers
{
    public class NewsControllerTests
    {
        private readonly Mock<INewsLogic> _newsLogic;

        private NewsController Sut { get; }

        public NewsControllerTests()
        {
            _newsLogic = new Mock<INewsLogic>(MockBehavior.Strict);

            Sut = new NewsController(_newsLogic.Object);
        }

        [Fact]
        public void Constructor_GivenNullParam_ThrowsArgumentNullException()
        {
            // Act
            var result = Record.Exception(() => new NewsController(null));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<ArgumentNullException>();
                assertionResult.Subject.Message.Should().NotBeNullOrWhiteSpace();
            }
        }

        public static readonly TheoryData<ICollection<NewsItem>> EmptyBestStoriesResult =
            new TheoryData<ICollection<NewsItem>>
            {
                null, new NewsItem[0]
            };

        [Theory]
        [MemberData(nameof(EmptyBestStoriesResult))]
        public async void GetAsync_GivenNoBestStories_ReturnsNotFoundResult(ICollection<NewsItem> bestStories)
        {
            // Arrange
            _newsLogic.Setup(m => m.GetBestStoriesAsync())
                .ReturnsAsync(bestStories);

            // Act
            var result = await Sut.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Result.Should().BeOfType<NotFoundResult>();
                result.Value.Should().BeNull();
            }

            _newsLogic.Verify(m => m.GetBestStoriesAsync(), Times.Once);
        }

        [Fact]
        public async void GetAsync_GivenExceptionWithStatusCode_ReturnsStatusCodeResult()
        {
            // Arrange
            const HttpStatusCode errorStatusCode = HttpStatusCode.BadRequest;
            const string exceptionMessage = "Exception message";

            _newsLogic.Setup(m => m.GetBestStoriesAsync())
                .ThrowsAsync(new HackerNewsClientException(errorStatusCode, exceptionMessage));

            // Act
            var result = await Sut.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertedResult = result.Result.Should().BeOfType<ObjectResult>();
                assertedResult.Subject.StatusCode.Should().Be((int)errorStatusCode);
                assertedResult.Subject.Value.Should().Be(exceptionMessage);
            }

            _newsLogic.Verify(m => m.GetBestStoriesAsync(), Times.Once);
        }

        [Fact]
        public async void GetAsync_GivenExceptionWithoutStatusCode_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _newsLogic.Setup(m => m.GetBestStoriesAsync())
                .ThrowsAsync(new HackerNewsClientException());

            // Act
            var result = await Sut.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertedResult = result.Result.Should().BeOfType<StatusCodeResult>();
                assertedResult.Subject.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            }

            _newsLogic.Verify(m => m.GetBestStoriesAsync(), Times.Once);
        }

        [Fact]
        public async void GetAsync_GivenBestStories_ReturnsExpectedResult()
        {
            // Arrange
            var newsItems = new[]
            {
                new NewsItem
                {
                    By = "Test User",
                    Dead = false,
                    Deleted = false,
                    Descendants = 100,
                    Id = 1234,
                    Kids = new [] { 9, 8, 7 },
                    Parent = 999,
                    Parts = new [] { 6, 5 },
                    Pool = 947,
                    Score = 1000,
                    Text = "News Article Text",
                    Title = "News Article",
                    Type = NewsItemType.Story,
                    UnixTime = 1608580776,
                    Url = new Uri("https://example.com/news/article/1234")
                }
            };

            var expectedResult = new[] { new NewsItemDto(newsItems[0]) };

            _newsLogic.Setup(m => m.GetBestStoriesAsync())
                .ReturnsAsync(newsItems);

            // Act
            var result = await Sut.GetAsync();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertedResult = result.Result.Should().BeOfType<OkObjectResult>();
                assertedResult.Subject.StatusCode.Should().Be((int)HttpStatusCode.OK);
                assertedResult.Subject.Value.Should().NotBeNull();
                assertedResult.Subject.Value.Should().BeEquivalentTo(expectedResult);
            }

            _newsLogic.Verify(m => m.GetBestStoriesAsync(), Times.Once);
        }
    }
}
