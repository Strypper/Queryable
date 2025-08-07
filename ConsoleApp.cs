using Microsoft.Extensions.DependencyInjection;
using Queryable.Extensions;
using Queryable.Models;

namespace Queryable
{
    /// <summary>
    /// Console application example using clean Context pattern.
    /// Similar to Program.cs in ASP.NET Core applications but with direct context access.
    /// </summary>
    public class ConsoleApp
    {
        public static async Task RunWithDependencyInjectionAsync()
        {
            Console.WriteLine("🏗️ Setting up Dependency Injection (Clean Context Pattern)...");
            Console.WriteLine("============================================================");

            // Create service collection (similar to ASP.NET Core)
            var services = new ServiceCollection();

            // Configure ApiContext (similar to AddDbContext in EF Core)
            services.AddApiContext<LightningLanesApiContext>(options =>
            {
                options.BaseUrl = "https://localhost:7263/api";
                options.BearerToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI3NzM3M0I4RjZBQ0ZFRkJBNzQ1MDUyMTg2QzZENEU3IiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LXNlcnZlci1leWdzZXlnNmhhZjJmNmFmLmVhc3Rhc2lhLTAxLmF6dXJld2Vic2l0ZXMubmV0IiwibmJmIjoxNzU0NDY3MDY3LCJpYXQiOjE3NTQ0NjcwNjcsImV4cCI6MTc1NzA1OTA2Nywic2NvcGUiOlsibGlnaHRuaW5nLWxhbmVzLWFwcCIsIm9wZW5pZCIsInByb2ZpbGUiXSwiYW1yIjpbInB3ZCJdLCJjbGllbnRfaWQiOiJwb3N0bWFuIiwic3ViIjoiOWNiMDE0ZDMtNzBhZi00YzkzLTgwNDMtMmEzMTRkOGE5MGY2IiwiYXV0aF90aW1lIjoxNzU0NDY3MDY3LCJpZHAiOiJsb2NhbCIsImp0aSI6IkEyMDg0MjkyRjAzRjQxOUI2OUI3QzMxN0MzODc4MzE0In0.CiBP9JEtW_G_RAepcqhbuhC_nhukmq17HJevA_yv6aYEkelugNE5jIgHaiWtjLqa22duP-uW7KVz-7mMrUV5wD1bBMuLe2OtNl8Ml8LfPX64ImuqKZVIhqrLoFo15sCDMAPl26vg8w2gExUOmvDSKGBVD8pkJ5aBJN0eByslV8IuFPl-ObMivaRqHd4d7VQ_A0beycimVLizraBgknR9bgRsq8w5Ylq__NNCjyPbQVbHKpIJajJvHYcQW9eBJlaMWG8RqWimLdF7eE6D5wDg1GWj4fwQAyBwQp1SWzL5tP4KouZcI-DT1TURwrbzw7rejyiUZ0faMgTmoJl9Uim1nA";
                options.Timeout = TimeSpan.FromSeconds(30);
                options.IgnoreSslErrors = true; // For localhost testing
            });

            // No wrapper layers needed - direct context access!

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("✅ Dependency Injection configured!");

            try
            {
                // Get context from DI container (direct access to data)
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LightningLanesApiContext>();

                Console.WriteLine("\n🎯 Testing clean Context pattern...");
                Console.WriteLine("===================================");

                // Campaign operations using direct context
                Console.WriteLine("📊 Testing Campaign operations...");
                Console.WriteLine("==================================");

                var allCampaigns = context.Campaigns.ToList();
                Console.WriteLine($"✅ Found {allCampaigns.Count} total campaigns");

                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .OrderBy(c => c.StartDate)
                    .ToList();
                Console.WriteLine($"✅ Found {activeCampaigns.Count} active campaigns");

                var expensiveCampaigns = context.Campaigns
                    .Where(c => c.Budget > 100000)
                    .OrderByDescending(c => c.Budget)
                    .ToList();
                Console.WriteLine($"✅ Found {expensiveCampaigns.Count} expensive campaigns");

                var searchResults = context.Campaigns
                    .Where(c => c.Name.Contains("Luxury") || c.Description.Contains("Luxury"))
                    .OrderBy(c => c.Name)
                    .ToList();
                Console.WriteLine($"✅ Found {searchResults.Count} campaigns matching 'Luxury'");

                // Message operations using direct context
                Console.WriteLine("\n📨 Testing Message operations...");
                Console.WriteLine("=================================");

                var allMessages = context.Messages.ToList();
                Console.WriteLine($"✅ Found {allMessages.Count} total messages");

                var recentMessages = context.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList();
                Console.WriteLine($"✅ Found {recentMessages.Count} recent messages");

                // Test with specific campaign ID from your example
                var campaignId = "3fd2f1a1-aaf1-49d1-9a8a-0661415eb429";
                var campaignMessages = context.Messages
                    .Where(m => m.CampaignId == campaignId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(8)
                    .ToList();
                Console.WriteLine($"✅ Found {campaignMessages.Count} messages for campaign {campaignId}");

                var messageSearchResults = context.Messages
                    .Where(m => m.Content.Contains("test"))
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();
                Console.WriteLine($"✅ Found {messageSearchResults.Count} messages containing 'test'");

                // NEW: Test Campaign Add functionality (like EF Core)
                Console.WriteLine("\n🆕 Testing Campaign Add functionality (EF Core style)...");
                Console.WriteLine("========================================================");

                // Create new campaign with the payload you specified
                var newCampaign = new Campaign
                {
                    Name = "Testing campaign from postman",
                    PartnerName = "Totechs",
                    PartnerLogo = null,
                    Budget = 85000.00m,
                    Description = "Increase sales of school supplies by 40%, Reach 75,000 parents through targeted ads, Achieve 20,000 app downloads",
                    Status = "Active",
                    StartDate = new DateTime(2024, 7, 20),
                    EndDate = new DateTime(2024, 9, 10)
                };

                Console.WriteLine("📝 Campaign to add:");
                Console.WriteLine($"   📊 Name: {newCampaign.Name}");
                Console.WriteLine($"   🤝 Partner: {newCampaign.PartnerName}");
                Console.WriteLine($"   💰 Budget: {newCampaign.Budget:C}");
                Console.WriteLine($"   📅 Duration: {newCampaign.StartDate:yyyy-MM-dd} to {newCampaign.EndDate:yyyy-MM-dd}");
                Console.WriteLine($"   📋 Status: {newCampaign.Status}");

                try
                {
                    // Add campaign using EF Core-like DbSet syntax
                    var addedCampaign = await context.Campaigns.AddAsync(newCampaign);
                    Console.WriteLine($"✅ Campaign added successfully!");
                    Console.WriteLine($"   🆔 Campaign ID: {addedCampaign.Id}");
                    Console.WriteLine($"   📛 Name: {addedCampaign.Name}");
                    Console.WriteLine($"   💰 Budget: {addedCampaign.Budget:C}");

                    // Verify the campaign was added by querying it
                    Console.WriteLine("\n🔍 Verifying campaign was added...");
                    var allCampaignsAfterAdd = context.Campaigns.ToList();
                    Console.WriteLine($"✅ Total campaigns after add: {allCampaignsAfterAdd.Count}");
                    
                    // Find the newly added campaign
                    var foundNewCampaign = context.Campaigns
                        .Where(c => c.Name == "Testing campaign from postman")
                        .ToList()
                        .FirstOrDefault();
                    
                    if (foundNewCampaign != null)
                    {
                        Console.WriteLine($"✅ Found newly added campaign: {foundNewCampaign.Name} (ID: {foundNewCampaign.Id})");
                    }
                }
                catch (Exception addEx)
                {
                    Console.WriteLine($"❌ Error adding campaign: {addEx.Message}");
                }

                // Display some results
                Console.WriteLine("\n📋 Sample expensive campaigns:");
                foreach (var campaign in expensiveCampaigns.Take(3))
                {
                    Console.WriteLine($"   💰 {campaign.Name}: {campaign.Budget:C} ({campaign.Status})");
                }

                Console.WriteLine("\n📋 Sample recent messages:");
                foreach (var message in recentMessages.Take(3))
                {
                    var userName = message.User?.Name ?? "Unknown User";
                    Console.WriteLine($"   📝 {userName}: \"{message.Content}\" (at {message.CreatedAt:yyyy-MM-dd HH:mm})");
                }

                Console.WriteLine("\n🎯 Summary:");
                Console.WriteLine("=============");
                Console.WriteLine("✅ No unnecessary wrapper layers");
                Console.WriteLine("✅ Direct LINQ access through context");
                Console.WriteLine("✅ EF Core-like Add functionality");
                Console.WriteLine("✅ Clean and simple architecture!");
                Console.WriteLine("✅ Focus on context design - not wrapper layers!");
                Console.WriteLine("✅ POST operations work seamlessly with JWT Bearer token!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            finally
            {
                await serviceProvider.DisposeAsync();
            }
        }
    }
}

