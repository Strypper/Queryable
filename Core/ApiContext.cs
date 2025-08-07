using Queryable.Models;

namespace Queryable.Core
{
    public class ApiContext : IDisposable
    {
        private bool _disposed = false;
        private readonly HttpClient _httpClient;
        protected HttpClient HttpClient => _httpClient;
        private readonly ApiContextOptions _options;
        protected ApiContextOptions Options => _options;

        public ApiContext(ApiContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = CreateHttpClient(options);
            
            // Initialize ApiSets with DbSet-like interface
            Campaigns = new ApiSet<Campaign>(_httpClient, $"{options.BaseUrl}/campaigns");
            Messages = new ApiSet<Message>(_httpClient, $"{options.BaseUrl}/messages");
        }

        /// <summary>
        /// Constructor for dependency injection with external HttpClient
        /// </summary>
        public ApiContext(ApiContextOptions options, HttpClient httpClient)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            // Initialize ApiSets with DbSet-like interface
            Campaigns = new ApiSet<Campaign>(_httpClient, $"{options.BaseUrl}/campaigns");
            Messages = new ApiSet<Message>(_httpClient, $"{options.BaseUrl}/messages");
        }

        // DbSet-like access - clean and simple like EF Core DbContext
        public IApiSet<Campaign> Campaigns { get; }
        public IApiSet<Message> Messages { get; }

        private static HttpClient CreateHttpClient(ApiContextOptions options)
        {
            HttpClientHandler? handler = null;
            
            if (options.IgnoreSslErrors)
            {
                handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            }

            var httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
            httpClient.Timeout = options.Timeout;

            // Set base URL
            if (!string.IsNullOrEmpty(options.BaseUrl))
            {
                httpClient.BaseAddress = new Uri(options.BaseUrl);
            }

            // Set Bearer token
            if (!string.IsNullOrEmpty(options.BearerToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.BearerToken);
            }

            // Custom configuration
            options.ConfigureHttpClient?.Invoke(httpClient);

            return httpClient;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}
