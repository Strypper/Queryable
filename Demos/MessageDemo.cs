using Microsoft.Extensions.DependencyInjection;

namespace Queryable.Demos
{
    /// <summary>
    /// Demo specifically for testing Message functionality
    /// Shows clean Context pattern for Messages
    /// </summary>
    public class MessageDemo
    {
        public static async Task RunMessageDemoAsync()
        {
            DemoConfiguration.PrintDemoHeader(
                "Message API Demo",
                "Testing Message operations with direct context usage",
                "/api/messages"
            );

            var serviceProvider = DemoConfiguration.ConfigureServices();

            try
            {
                Console.WriteLine("🔗 Connected to Messages API");
                Console.WriteLine();

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LightningLanesApiContext>();

                // Test 1: Get all messages using direct context
                Console.WriteLine("📨 Test 1: Direct context usage - Getting all messages...");
                var allMessages = context.Messages.ToList();
                Console.WriteLine($"✅ Found {allMessages.Count} messages total");
                Console.WriteLine();

                // Test 2: Get messages by campaign using direct LINQ
                Console.WriteLine("🎯 Test 2: LINQ filtering - Getting messages by campaign...");
                if (allMessages.Any())
                {
                    var campaignId = allMessages.First().CampaignId;
                    var campaignMessages = context.Messages
                        .Where(m => m.CampaignId == campaignId)
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(5)
                        .ToList();
                    
                    Console.WriteLine($"✅ Found {campaignMessages.Count} messages for campaign {campaignId}");
                    Console.WriteLine("📝 Sample messages:");
                    foreach (var message in campaignMessages.Take(3))
                    {
                        var userName = message.User?.Name ?? "Unknown User";
                        Console.WriteLine($"   📅 {message.CreatedAt:yyyy-MM-dd HH:mm} | {userName}: \"{message.Content}\"");
                    }
                }
                Console.WriteLine();

                // Test 3: Recent messages using direct LINQ
                Console.WriteLine("⏰ Test 3: LINQ sorting - Getting recent messages...");
                var recentMessages = context.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList();
                Console.WriteLine($"✅ Found {recentMessages.Count} recent messages");
                
                Console.WriteLine("📝 Recent messages:");
                foreach (var message in recentMessages.Take(3))
                {
                    var userName = message.User?.Name ?? "Unknown User";
                    Console.WriteLine($"   📅 {message.CreatedAt:yyyy-MM-dd HH:mm} | {userName}: \"{message.Content}\"");
                }
                Console.WriteLine();

                // Test 4: Search functionality using direct LINQ
                Console.WriteLine("🔍 Test 4: LINQ search - Searching messages...");
                var searchResults = context.Messages
                    .Where(m => m.Content.Contains("2"))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();
                Console.WriteLine($"✅ Messages containing '2': {searchResults.Count}");
                Console.WriteLine();

                // Test 5: Message count using direct LINQ
                Console.WriteLine("📊 Test 5: LINQ aggregation - Counting messages by campaign...");
                if (allMessages.Any())
                {
                    var campaignId = allMessages.First().CampaignId;
                    var messageCount = context.Messages
                        .Where(m => m.CampaignId == campaignId)
                        .ToList()
                        .Count;
                    Console.WriteLine($"✅ Total messages in campaign {campaignId}: {messageCount}");
                }
                Console.WriteLine();

                // Test 6: Latest message using direct LINQ
                Console.WriteLine("📝 Test 6: LINQ query - Latest message by campaign...");
                if (allMessages.Any())
                {
                    var campaignId = allMessages.First().CampaignId;
                    var latestMessage = context.Messages
                        .Where(m => m.CampaignId == campaignId)
                        .OrderByDescending(m => m.CreatedAt)
                        .ToList()
                        .FirstOrDefault();
                    if (latestMessage != null)
                    {
                        var userName = latestMessage.User?.Name ?? "Unknown User";
                        Console.WriteLine($"✅ Latest message in campaign {campaignId}: \"{latestMessage.Content}\" by {userName}");
                    }
                }

                // Prepare stats for footer
                var stats = new Dictionary<string, object>
                {
                    ["📊 Total messages"] = allMessages.Count,
                    ["⏰ Recent messages"] = recentMessages.Count,
                    ["🔍 Search results"] = searchResults.Count
                };

                DemoConfiguration.PrintDemoFooter("Message API Demo", stats);
                Console.WriteLine("   🎯 Clean Context pattern - Direct LINQ access!");
                Console.WriteLine("   ✨ No unnecessary wrapper layers!");

            }
            catch (Exception ex)
            {
                DemoConfiguration.HandleDemoException(ex, "Message API Demo");
            }
            finally
            {
                await serviceProvider.DisposeAsync();
            }
        }
    }
}

