using System;
using Newtonsoft.Json;

namespace Queryable.Models
{
    public class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("campaignId")]
        public string CampaignId { get; set; } = string.Empty;

        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;

        [JsonProperty("createdById")]
        public string CreatedById { get; set; } = string.Empty;

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [JsonProperty("updatedById")]
        public string? UpdatedById { get; set; }

        [JsonProperty("user")]
        public User? User { get; set; }

        public override string ToString()
        {
            return $"Message {{ Id: {Id}, CampaignId: {CampaignId}, Content: '{Content}', CreatedBy: {User?.Name ?? CreatedById}, CreatedAt: {CreatedAt:yyyy-MM-dd HH:mm} }}";
        }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("avatarUrl")]
        public string? AvatarUrl { get; set; }

        public override string ToString()
        {
            return $"User {{ Id: {Id}, Name: '{Name}' }}";
        }
    }
}
