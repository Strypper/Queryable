using Microsoft.Extensions.DependencyInjection;
using Queryable.Models;

namespace Queryable.Demos
{
    /// <summary>
    /// Combined demo showing both Campaign and Message functionality
    /// Demonstrates clean Context pattern for complete API access
    /// </summary>
    public class CombinedDemo
    {
        public static async Task RunCombinedDemoAsync()
        {
            DemoConfiguration.PrintDemoHeader(
                "Combined API Demo",
                "Clean Context pattern for both Campaigns and Messages",
                "Multiple endpoints"
            );

            var serviceProvider = DemoConfiguration.ConfigureServices();

            try
            {
                Console.WriteLine("🔗 Connected to Lightning Lanes API");
                Console.WriteLine();

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LightningLanesApiContext>();

                // === CAMPAIGNS SECTION ===
                Console.WriteLine("🎯 === CAMPAIGNS SECTION ===");

                // Get all campaigns using context
                Console.WriteLine("📊 Getting all campaigns...");
                var allCampaigns = context.Campaigns.ToList();
                Console.WriteLine($"✅ Total campaigns found: {allCampaigns.Count}");

                // Active campaigns using direct LINQ
                Console.WriteLine("🎯 Getting active campaigns using direct LINQ...");
                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .OrderBy(c => c.StartDate)
                    .ToList();
                Console.WriteLine($"✅ Active campaigns: {activeCampaigns.Count}");
                
                Console.WriteLine("🎯 Sample active campaigns:");
                foreach (var campaign in activeCampaigns.Take(3))
                {
                    Console.WriteLine($"   💼 {campaign.Name}: {campaign.Budget:C} ({campaign.Status})");
                }
                Console.WriteLine();

                // === MESSAGES SECTION ===
                Console.WriteLine("📨 === MESSAGES SECTION ===");

                // Get all messages using context
                Console.WriteLine("📨 Getting all messages...");
                var allMessages = context.Messages.ToList();
                Console.WriteLine($"✅ Total messages found: {allMessages.Count}");

                // Recent messages using direct LINQ
                Console.WriteLine("⏰ Getting recent messages using direct LINQ...");
                var recentMessages = context.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList();
                Console.WriteLine($"✅ Recent messages: {recentMessages.Count}");

                Console.WriteLine("📝 Sample recent messages:");
                foreach (var message in recentMessages.Take(3))
                {
                    var userName = message.User?.Name ?? "Unknown User";
                    Console.WriteLine($"   📅 {message.CreatedAt:yyyy-MM-dd HH:mm} | {userName}: \"{message.Content}\"");
                }
                Console.WriteLine();

                // === CROSS-ENTITY OPERATIONS ===
                Console.WriteLine("🔄 === CROSS-ENTITY OPERATIONS ===");

                // Analyze first campaign
                if (allCampaigns.Any())
                {
                    var firstCampaign = allCampaigns.First();
                    Console.WriteLine($"🔍 Analyzing campaign: {firstCampaign.Name}");

                    // Get messages for this campaign using direct LINQ
                    var campaignMessages = context.Messages
                        .Where(m => m.CampaignId == firstCampaign.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(10)
                        .ToList();
                    Console.WriteLine($"   📨 Messages in campaign: {campaignMessages.Count}");

                    // Get latest message using direct LINQ
                    var latestMessage = context.Messages
                        .Where(m => m.CampaignId == firstCampaign.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .ToList()
                        .FirstOrDefault();
                    
                    if (latestMessage != null)
                    {
                        var userName = latestMessage.User?.Name ?? "Unknown User";
                        Console.WriteLine($"   📝 Latest message: \"{latestMessage.Content}\" by {userName}");
                    }
                }

                // Search capabilities demonstration
                Console.WriteLine("🔍 Search capabilities demonstration:");
                var messagesContaining2 = context.Messages
                    .Where(m => m.Content.Contains("2"))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();
                Console.WriteLine($"   🔍 Messages containing '2': {messagesContaining2.Count}");
                Console.WriteLine();

                // === ADVANCED CONTEXT FEATURES ===
                Console.WriteLine("⚙️ === ADVANCED CONTEXT FEATURES ===");

                // Budget range campaigns
                var budgetRangeCampaigns = context.Campaigns
                    .Where(c => c.Budget >= 100000 && c.Budget <= 500000)
                    .OrderByDescending(c => c.Budget)
                    .ToList();
                Console.WriteLine($"💰 Campaigns in $100K-$500K range: {budgetRangeCampaigns.Count}");

                // Search campaigns by name
                var luxuryCampaigns = context.Campaigns
                    .Where(c => c.Name.Contains("Luxury") || c.Description.Contains("Luxury"))
                    .OrderBy(c => c.Name)
                    .ToList();
                Console.WriteLine($"🔍 Campaigns matching 'Luxury': {luxuryCampaigns.Count}");

                // Messages by specific user (if any)
                if (allMessages.Any())
                {
                    var firstUserId = allMessages.First().CreatedById;
                    var userMessages = context.Messages
                        .Where(m => m.CreatedById == firstUserId)
                        .OrderByDescending(m => m.CreatedAt)
                        .ToList();
                    Console.WriteLine($"👤 Messages by user {firstUserId}: {userMessages.Count}");
                }
                Console.WriteLine();

                // Prepare comprehensive stats
                var stats = new Dictionary<string, object>
                {
                    ["📊 Total campaigns"] = allCampaigns.Count,
                    ["🎯 Active campaigns"] = activeCampaigns.Count,
                    ["💰 Budget range campaigns"] = budgetRangeCampaigns.Count,
                    ["📨 Total messages"] = allMessages.Count,
                    ["⏰ Recent messages"] = recentMessages.Count,
                    ["🔍 Search results"] = luxuryCampaigns.Count
                };

                DemoConfiguration.PrintDemoFooter("Combined API Demo", stats);
                Console.WriteLine("   🎯 Clean Context pattern across multiple entities!");
                Console.WriteLine("   ✨ No unnecessary wrapper layers - direct LINQ access!");

            }
            catch (Exception ex)
            {
                DemoConfiguration.HandleDemoException(ex, "Combined API Demo");
            }
            finally
            {
                await serviceProvider.DisposeAsync();
            }
        }
    }
}

