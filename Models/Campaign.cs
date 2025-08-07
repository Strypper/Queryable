using System;
using Newtonsoft.Json;

namespace Queryable.Models
{
    public class Campaign
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("partnerName")]
        public string PartnerName { get; set; } = string.Empty;

        [JsonProperty("partnerLogo")]
        public string? PartnerLogo { get; set; }

        [JsonProperty("budget")]
        public decimal Budget { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("progress")]
        public int Progress { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("tasks")]
        public object[] Tasks { get; set; } = Array.Empty<object>();

        /// <summary>
        /// Computed property to check if campaign is active
        /// </summary>
        public bool IsActive => Status == "Active" && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

        public override string ToString()
        {
            return $"Campaign {{ Id: {Id}, Name: '{Name}', Partner: '{PartnerName}', Budget: {Budget:C}, Status: {Status} }}";
        }
    }
}
