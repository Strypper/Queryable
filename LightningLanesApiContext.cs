using System.Net.Http;
using Queryable.Core;
using Queryable.Models;

namespace Queryable
{
    /// <summary>
    /// Custom ApiContext for Lightning Lanes application.
    /// Similar to DbContext in Entity Framework Core.
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

        // Direct access to base IQueryable - pure LINQ functionality
        // These provide all the LINQ functionality needed without any wrapper
        // public IQueryable<Campaign> Campaigns => base.Campaigns;
        // public IQueryable<Message> Messages => base.Messages;
    }
}
