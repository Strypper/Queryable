using System;
using System.Collections.Generic;
using Queryable.Core.Sets;

namespace Queryable.Core.Context
{
    public class EndpointBuilder<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly ApiContextOptions _options;
        private string? _endpoint;
        private string? _apiVersion; // ✅ Make nullable để detect có set hay không
        private Dictionary<string, string> _customHeaders = new();
        private TimeSpan? _customTimeout;
        private Func<HttpClient, IApiSet<T>>? _customFactory;

        internal EndpointBuilder(HttpClient httpClient, ApiContextOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Specify the endpoint path
        /// </summary>
        public EndpointBuilder<T> WithEndpoint(string endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            return this;
        }

        /// <summary>
        /// Specify API version - nếu không gọi thì sẽ không có version
        /// </summary>
        public EndpointBuilder<T> WithVersion(string version)
        {
            _apiVersion = version ?? throw new ArgumentNullException(nameof(version));
            return this;
        }

        /// <summary>
        /// Add custom header for this endpoint
        /// </summary>
        public EndpointBuilder<T> WithHeader(string name, string value)
        {
            _customHeaders[name] = value;
            return this;
        }

        /// <summary>
        /// Set custom timeout for this endpoint
        /// </summary>
        public EndpointBuilder<T> WithTimeout(TimeSpan timeout)
        {
            _customTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Use convention-based endpoint naming with smart version handling
        /// </summary>
        public EndpointBuilder<T> WithConventionNaming()
        {
            var entityName = typeof(T).Name.ToLower();
            
            // ✅ Check if version was set
            if (!string.IsNullOrEmpty(_apiVersion))
            {
                _endpoint = $"/api/{_apiVersion}/{entityName}s";  // With version
            }
            else
            {
                _endpoint = $"/api/{entityName}s";  // Without version
            }
            
            return this;
        }

        /// <summary>
        /// Use custom factory for ApiSet creation
        /// </summary>
        public EndpointBuilder<T> WithCustomFactory(Func<HttpClient, IApiSet<T>> factory)
        {
            _customFactory = factory;
            return this;
        }

        /// <summary>
        /// Build the configured ApiSet with smart version handling
        /// </summary>
        public IApiSet<T> Build()
        {
            if (string.IsNullOrEmpty(_endpoint))
            {
                throw new InvalidOperationException($"Endpoint not specified for {typeof(T).Name}");
            }

            // Use custom factory if provided
            if (_customFactory != null)
            {
                return _customFactory(_httpClient);
            }

            // ✅ Smart version handling logic
            string fullEndpoint = BuildVersionedEndpoint();

            // Create ApiSet
            var apiSet = new ApiSet<T>(_httpClient, fullEndpoint);

            // Apply custom configurations if needed
            ApplyCustomConfigurations(apiSet);

            return apiSet;
        }

        private string BuildVersionedEndpoint()
        {
            // Case 1: No version specified - simple concatenation
            if (string.IsNullOrEmpty(_apiVersion))
            {
                return _endpoint!.StartsWith("/") 
                    ? $"{_options.BaseUrl}{_endpoint}"     // No version
                    : $"{_options.BaseUrl}/{_endpoint}";   // No version
            }

            // Case 2: Version specified - smart version handling
            
            // Check if endpoint already has /api/ prefix
            if (_endpoint!.StartsWith("/api/"))
            {
                // Already has /api/, use as-is (user manages their own versioning)
                return $"{_options.BaseUrl}{_endpoint}";
            }

            // Check if BaseUrl already ends with /api
            var baseUrl = _options.BaseUrl.TrimEnd('/');
            var hasApiInBase = baseUrl.EndsWith("/api", StringComparison.OrdinalIgnoreCase);
            
            if (hasApiInBase)
            {
                // BaseUrl has /api, add version directly
                return _endpoint.StartsWith("/") 
                    ? $"{baseUrl}/{_apiVersion}{_endpoint}"      // BaseUrl/v1/endpoint
                    : $"{baseUrl}/{_apiVersion}/{_endpoint}";    // BaseUrl/v1/endpoint
            }
            else
            {
                // BaseUrl doesn't have /api, add full path
                return _endpoint.StartsWith("/") 
                    ? $"{baseUrl}/api/{_apiVersion}{_endpoint}"  // BaseUrl/api/v1/endpoint  
                    : $"{baseUrl}/api/{_apiVersion}/{_endpoint}"; // BaseUrl/api/v1/endpoint
            }
        }

        private void ApplyCustomConfigurations(IApiSet<T> apiSet)
        {
            // Apply custom headers, timeout, etc.
            // This would require extending ApiSet to support these features
            
            // For now, this is a placeholder for future enhancements
            foreach (var header in _customHeaders)
            {
                // TODO: Apply custom headers to ApiSet
                // apiSet.AddHeader(header.Key, header.Value);
            }

            if (_customTimeout.HasValue)
            {
                // TODO: Apply custom timeout to ApiSet
                // apiSet.SetTimeout(_customTimeout.Value);
            }
        }
    }
}
