using CoreHackerNews;
using CoreHackerNews.Exceptions;
using CoreHackerNews.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CoreHackerNewsTests
{
    public class HackerNewsClientTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;

        private HackerNewsClient Sut { get; }

        public HackerNewsClientTests()
        {
            var configuration = new NewsConfiguration
            {
                BestStoriesSize = 20,
                CacheExpirationInSeconds = 3,
                CacheKey = "Cache Key",
                HackerNewsApiUrl = new Uri("https://hacker-news.firebaseio.com/v0")
            };

            // We need to use MockBehavior.Loose otherwise the test will throw an exception about
            // the Dispose method not being mocked. And we can't really mock it as it's a non virtual
            // method, so it can't be overriden.
            _httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var httpClient = new HttpClient(_httpMessageHandler.Object);

            Sut = new HackerNewsClient(configuration, httpClient);
        }

        [Fact]
        public void Constructor_GivenNullParam_ThrowsArgumentNullException()
        {
            // Act
            var result = Record.Exception(() => new HackerNewsClient(null));

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
            var result = Record.Exception(() => new HackerNewsClient(configuration));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<ArgumentException>();
                assertionResult.Subject.Message.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async void GetBestStoriesIdsAsync_ForFailureStatusCode_ThrowsHackerNewsClientException()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request"
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Record.ExceptionAsync(() => Sut.GetBestStoriesIdsAsync());

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<HackerNewsClientException>();
                assertionResult.Subject.StatusCode.Should().Be(response.StatusCode);
                assertionResult.Subject.Message.Should().Be(response.ReasonPhrase);
            }

            _httpMessageHandler.VerifyAll();
        }

        public static readonly TheoryData<Exception> GetBestStoriesIdsAsyncExceptions = new TheoryData<Exception>
        {
            new ArgumentNullException("exception"), new InvalidOperationException("exception"), new HttpRequestException("exception")
        };

        [Theory]
        [MemberData(nameof(GetBestStoriesIdsAsyncExceptions))]
        public async void GetBestStoriesIdsAsync_ForRequestException_ThrowsHackerNewsClientException(Exception ex)
        {
            // Arrange
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(ex);

            // Act
            var result = await Record.ExceptionAsync(() => Sut.GetBestStoriesIdsAsync());

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<HackerNewsClientException>();
                assertionResult.Subject.StatusCode.Should().BeNull();
                assertionResult.Subject.Message.Should().Be(ex.Message);
                assertionResult.Subject.InnerException.Should().BeSameAs(ex);
            }

            _httpMessageHandler.VerifyAll();
        }

        [Fact]
        public async void GetBestStoriesIdsAsync_ForEmptyResponse_ReturnsNull()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Sut.GetBestStoriesIdsAsync();

            // Assert
            result.Should().BeNull();

            _httpMessageHandler.VerifyAll();
        }

        [Fact]
        public async void GetBestStoriesIdsAsync_ForSuccessfulResponse_ReturnsExpectedResult()
        {
            // Arrange
            var storyIds = new[] { 123, 456, 789 };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[123,456,789]")
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Sut.GetBestStoriesIdsAsync();

            // Assert
            result.Should().BeEquivalentTo(storyIds);

            _httpMessageHandler.VerifyAll();
        }

        [Fact]
        public async void GetStoryDetailsAsync_ForFailureStatusCode_ThrowsHackerNewsClientException()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request"
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Record.ExceptionAsync(() => Sut.GetStoryDetailsAsync(1));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<HackerNewsClientException>();
                assertionResult.Subject.StatusCode.Should().Be(response.StatusCode);
                assertionResult.Subject.Message.Should().Be(response.ReasonPhrase);
            }

            _httpMessageHandler.VerifyAll();
        }

        public static readonly TheoryData<Exception> GetStoryDetailsAsyncExceptions = new TheoryData<Exception>
        {
            new ArgumentNullException("exception"), new InvalidOperationException("exception"), new HttpRequestException("exception")
        };

        [Theory]
        [MemberData(nameof(GetStoryDetailsAsyncExceptions))]
        public async void GetStoryDetailsAsync_ForRequestException_ThrowsHackerNewsClientException(Exception ex)
        {
            // Arrange
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(ex);

            // Act
            var result = await Record.ExceptionAsync(() => Sut.GetStoryDetailsAsync(1));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var assertionResult = result.Should().BeOfType<HackerNewsClientException>();
                assertionResult.Subject.StatusCode.Should().BeNull();
                assertionResult.Subject.Message.Should().Be(ex.Message);
                assertionResult.Subject.InnerException.Should().BeSameAs(ex);
            }

            _httpMessageHandler.VerifyAll();
        }

        [Fact]
        public async void GetStoryDetailsAsync_ForEmptyResponse_ReturnsNull()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Sut.GetStoryDetailsAsync(1);

            // Assert
            result.Should().BeNull();

            _httpMessageHandler.VerifyAll();
        }

        [Fact]
        public async void GetStoryDetailsAsync_ForSuccessfulResponse_ReturnsExpectedResult()
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

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(newsItem))
            };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await Sut.GetStoryDetailsAsync(1234);

            // Assert
            result.Should().BeEquivalentTo(newsItem);

            _httpMessageHandler.VerifyAll();
        }
    }
}
