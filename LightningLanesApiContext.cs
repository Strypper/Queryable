using Queryable.Core.Context;
using Queryable.Core.Sets;
using Queryable.Models;

namespace Queryable
{
    /// <summary>
    /// Custom ApiContext for Lightning Lanes application.
    /// Uses EndpointBuilder pattern for flexible endpoint configuration.
    /// Clean and simple - no unnecessary Repository wrapper layer.
    /// </summary>
    public class LightningLanesApiContext : ApiContext
    {
        public LightningLanesApiContext(ApiContextOptions options) : base(options)
        {
        }

        public LightningLanesApiContext(ApiContextOptions options, HttpClient httpClient) : base(options, httpClient)
        {
        }

        // Business entities
        public IApiSet<Campaign> Campaigns { get; private set; } = null!;
        public IApiSet<Message> Messages { get; private set; } = null!;

        /// <summary>
        /// Register Lightning Lanes specific endpoints
        /// </summary>
        protected override void OnEndpointRegistering()
        {
            Campaigns = RegisterEndpoint<Campaign>()
                .WithEndpoint("/campaigns")  
                .WithHeader("X-Service", "Lightning-Lanes")
                .Build();

            Messages = RegisterEndpoint<Message>()
                .WithEndpoint("/messages")
                .WithTimeout(TimeSpan.FromSeconds(30))
                .Build();
        }
    }
}
