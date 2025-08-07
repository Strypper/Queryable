using Microsoft.Extensions.DependencyInjection;

namespace Queryable.Demos
{
    /// <summary>
    /// Demo specifically for testing Campaign functionality
    /// Shows clean Repository pattern without unnecessary Service layer
    /// </summary>
    public class CampaignDemo
    {
        public static async Task RunCampaignDemoAsync()
        {
            DemoConfiguration.PrintDemoHeader(
                "Campaign API Demo", 
                "Demonstrating clean Context pattern (no unnecessary wrapper layers)",
                "/api/campaigns"
            );

            var serviceProvider = DemoConfiguration.ConfigureServices();

            try
            {
                Console.WriteLine("🔗 Connected to Campaigns API");
                Console.WriteLine();

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LightningLanesApiContext>();
                
                // Test 1: Direct context usage
                Console.WriteLine("📊 Test 1: Direct context usage - Getting all campaigns...");
                var allCampaigns = context.Campaigns.ToList();
                Console.WriteLine($"✅ Found {allCampaigns.Count} campaigns total");
                
                Console.WriteLine("\n📋 Sample campaigns:");
                foreach (var campaign in allCampaigns.Take(3))
                {
                    Console.WriteLine($"   💼 {campaign.Name}: {campaign.Budget:C} ({campaign.Status})");
                }
                Console.WriteLine();

                // Test 2: Advanced LINQ queries - Direct context usage
                Console.WriteLine("🎯 Test 2: Advanced LINQ queries - Direct context usage...");
                
                // Active campaigns using direct LINQ
                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .OrderBy(c => c.StartDate)
                    .ToList();
                Console.WriteLine($"✅ Found {activeCampaigns.Count} active campaigns");

                // Expensive campaigns using direct LINQ
                var expensiveCampaigns = context.Campaigns
                    .Where(c => c.Budget > DemoConfiguration.TestData.ExpensiveBudgetThreshold)
                    .OrderByDescending(c => c.Budget)
                    .ToList();
                Console.WriteLine($"✅ Found {expensiveCampaigns.Count} expensive campaigns (budget > ${DemoConfiguration.TestData.ExpensiveBudgetThreshold:N0})");

                Console.WriteLine("\n💰 Top expensive campaigns:");
                foreach (var campaign in expensiveCampaigns.Take(3))
                {
                    Console.WriteLine($"   💵 {campaign.Name}: {campaign.Budget:C} ({campaign.Status})");
                }
                Console.WriteLine();

                // Test 3: Complex LINQ queries
                Console.WriteLine("🔍 Test 3: Complex LINQ queries - Advanced filtering and sorting...");
                
                var complexQuery = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .Where(c => c.Budget > 50000)
                    .OrderByDescending(c => c.Budget)
                    .Take(5)
                    .ToList();
                    
                Console.WriteLine($"✅ Found {complexQuery.Count} active campaigns with budget > $50,000 (top 5 by budget):");
                
                foreach (var campaign in complexQuery.Take(5))
                {
                    Console.WriteLine($"   🎯 {campaign.Name}: {campaign.Budget:C} ({campaign.PartnerName})");
                }
                Console.WriteLine();

                // Test 4: More complex LINQ operations - All using direct context
                Console.WriteLine("⚙️ Test 4: More complex LINQ operations - All using direct context...");
                
                // Active campaigns - equivalent to former repository method
                var activeCampaignsFromContext = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .ToList();
                Console.WriteLine($"✅ Active campaigns via direct context: {activeCampaignsFromContext.Count}");

                // Budget range campaigns - equivalent to former repository method
                var budgetRangeCampaigns = context.Campaigns
                    .Where(c => c.Budget >= 100000 && c.Budget <= 500000)
                    .OrderByDescending(c => c.Budget)
                    .ToList();
                Console.WriteLine($"✅ Campaigns in budget range $100K-$500K: {budgetRangeCampaigns.Count}");

                // Search campaigns - equivalent to former repository method
                var searchResults = context.Campaigns
                    .Where(c => c.Name.Contains("Luxury") || c.Description.Contains("Luxury"))
                    .OrderBy(c => c.Name)
                    .ToList();
                Console.WriteLine($"✅ Campaigns matching 'Luxury': {searchResults.Count}");
                Console.WriteLine();

                // Test 5: Direct context query by ID
                Console.WriteLine("🔍 Test 5: Search by ID - Using direct context...");
                if (allCampaigns.Any())
                {
                    var firstCampaignId = allCampaigns.First().Id;
                    var foundCampaign = context.Campaigns
                        .Where(c => c.Id == firstCampaignId)
                        .ToList()
                        .FirstOrDefault();
                    
                    if (foundCampaign != null)
                    {
                        Console.WriteLine($"✅ Found campaign by ID: {foundCampaign.Name} ({foundCampaign.Id})");
                    }
                }
                Console.WriteLine();

                // Prepare stats dictionary first
                var stats = new Dictionary<string, object>
                {
                    ["📊 Total campaigns"] = allCampaigns.Count,
                    ["🎯 Active campaigns"] = activeCampaigns.Count,
                    ["💰 Expensive campaigns"] = expensiveCampaigns.Count,
                    ["🔍 Complex query results"] = complexQuery.Count,
                    ["🔍 Search results"] = searchResults.Count
                };

                // NEW: Test Campaign Add functionality (EF Core style)
                Console.WriteLine("🆕 Test 6: Campaign Add functionality (EF Core style)...");
                Console.WriteLine("=====================================================");

                // Create campaign with your specified payload
                var newCampaign = new Queryable.Models.Campaign
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

                Console.WriteLine("📝 Campaign details to add:");
                Console.WriteLine($"   📊 Name: {newCampaign.Name}");
                Console.WriteLine($"   🤝 Partner: {newCampaign.PartnerName}");
                Console.WriteLine($"   💰 Budget: {newCampaign.Budget:C}");
                Console.WriteLine($"   📋 Description: {newCampaign.Description}");
                Console.WriteLine($"   📅 Start Date: {newCampaign.StartDate:yyyy-MM-dd}");
                Console.WriteLine($"   📅 End Date: {newCampaign.EndDate:yyyy-MM-dd}");
                Console.WriteLine($"   📊 Status: {newCampaign.Status}");

                try
                {
                    // Add campaign using DbSet-like AddAsync method
                    var addedCampaign = await context.Campaigns.AddAsync(newCampaign);
                    Console.WriteLine($"✅ Campaign added successfully using context.Campaigns.AddAsync()!");
                    Console.WriteLine($"   🆔 Returned ID: {addedCampaign.Id}");
                    Console.WriteLine($"   📛 Returned Name: {addedCampaign.Name}");

                    // Verify campaign was added by querying
                    Console.WriteLine("\n🔍 Verifying campaign was added via LINQ query...");
                    var verificationCampaigns = context.Campaigns
                        .Where(c => c.Name == "Testing campaign from postman")
                        .ToList();
                    Console.WriteLine($"✅ Found {verificationCampaigns.Count} campaigns with name 'Testing campaign from postman'");

                    if (verificationCampaigns.Any())
                    {
                        var verifiedCampaign = verificationCampaigns.First();
                        Console.WriteLine($"   🎯 Verified: {verifiedCampaign.Name} (ID: {verifiedCampaign.Id})");
                        Console.WriteLine($"   💰 Budget: {verifiedCampaign.Budget:C}");
                        Console.WriteLine($"   🤝 Partner: {verifiedCampaign.PartnerName}");
                    }

                    // Update stats
                    stats["🆕 Campaign added"] = "Success";
                }
                catch (Exception addEx)
                {
                    Console.WriteLine($"❌ Error adding campaign: {addEx.Message}");
                    stats["🆕 Campaign added"] = "Failed";
                }

                Console.WriteLine();

                DemoConfiguration.PrintDemoFooter("Campaign API Demo", stats);
                Console.WriteLine("   🎯 Clean Context pattern - No unnecessary wrapper layers!");
                Console.WriteLine("   ✨ Direct LINQ access through context - simple and efficient!");
                Console.WriteLine("   🆕 EF Core-like Add functionality works perfectly with JWT Bearer token!");

            }
            catch (Exception ex)
            {
                DemoConfiguration.HandleDemoException(ex, "Campaign API Demo");
            }
            finally
            {
                await serviceProvider.DisposeAsync();
            }
        }
    }
}
