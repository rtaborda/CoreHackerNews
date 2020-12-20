using CoreHackerNews.Exceptions;
using CoreHackerNews.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreHackerNews
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private bool disposedValue;

        public HackerNewsClient(NewsConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (!configuration.IsValid)
                throw new ArgumentException($"Invalid {nameof(configuration)}");

            _baseUri = configuration.HackerNewsApiUrl;
            _httpClient = new HttpClient();
        }

        public async Task<ICollection<int>> GetBestStoriesIdsAsync()
            => await SendGetRequestAsync<ICollection<int>>(new Uri(_baseUri, "beststories.json"));

        public async Task<NewsItem> GetStoryDetailsAsync(int storyId)
            => await SendGetRequestAsync<NewsItem>(new Uri(_baseUri, $"item/{storyId}.json"));

        private async Task<T> SendGetRequestAsync<T>(Uri requestUri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            try
            {
                using var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(responseJson))
                    {
                        return default;
                    }

                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
                else
                {
                    // TODO check if this triggers the catch
                    throw new HackerNewsClientException(response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is HttpRequestException)
            {
                throw new HackerNewsClientException(ex.Message, ex);
            }
        }

        #region IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
