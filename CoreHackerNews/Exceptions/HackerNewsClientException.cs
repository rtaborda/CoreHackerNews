using System;
using System.Net;
using System.Net.Http;

namespace CoreHackerNews.Exceptions
{
    public class HackerNewsClientException : HttpRequestException
    {
        public HttpStatusCode? StatusCode { get; }

        public HackerNewsClientException()
        {
        }

        public HackerNewsClientException(string message) : base(message)
        {
        }

        public HackerNewsClientException(string message, Exception inner) : base(message, inner)
        {
        }

        public HackerNewsClientException(HttpStatusCode? statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HackerNewsClientException(HttpStatusCode? statusCode, string message, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
