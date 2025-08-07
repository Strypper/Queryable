using Microsoft.Extensions.DependencyInjection;
using Queryable.Models;

namespace Queryable.Demos
{
    /// <summary>
    /// Demo showcasing DbSet-like CRUD operations
    /// Demonstrates the power of IApiSet interface
    /// </summary>
    public class CrudDemo
    {
        public static async Task RunCrudDemoAsync()
        {
            DemoConfiguration.PrintDemoHeader(
                "CRUD Operations Demo (DbSet-like)", 
                "Showcasing all CRUD operations using IApiSet interface",
                "/api/campaigns (All HTTP Methods)"
            );

            var serviceProvider = DemoConfiguration.ConfigureServices();

            try
            {
                Console.WriteLine("🔗 Connected to API with DbSet-like interface");
                Console.WriteLine();

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<LightningLanesApiContext>();

                // === CREATE OPERATION ===
                Console.WriteLine("🆕 === CREATE OPERATION (POST) ===");
                Console.WriteLine("Testing context.Campaigns.AddAsync()...");

                var newCampaign = new Campaign
                {
                    Name = "DbSet Demo Campaign",
                    PartnerName = "ApiSet Testing",
                    Budget = 75000.00m,
                    Description = "Testing DbSet-like interface for Campaign CRUD operations",
                    Status = "Active",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(60)
                };

                var createdCampaign = await context.Campaigns.AddAsync(newCampaign);
                Console.WriteLine($"✅ Campaign created: {createdCampaign.Name} (ID: {createdCampaign.Id})");
                Console.WriteLine();

                // === READ OPERATIONS ===
                Console.WriteLine("📖 === READ OPERATIONS (GET) ===");
                
                // 1. LINQ Queries (IQueryable functionality)
                Console.WriteLine("1️⃣ LINQ Queries via IQueryable:");
                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .Take(3)
                    .ToList();
                Console.WriteLine($"   ✅ Found {activeCampaigns.Count} active campaigns via LINQ");

                // 2. Find by ID (returns null if not found)
                if (!string.IsNullOrEmpty(createdCampaign.Id))
                {
                    Console.WriteLine("2️⃣ Find by ID (returns null if not found):");
                    var foundCampaign = await context.Campaigns.FindAsync(createdCampaign.Id);
                    if (foundCampaign != null)
                    {
                        Console.WriteLine($"   ✅ Found campaign: {foundCampaign.Name}");
                    }
                    else
                    {
                        Console.WriteLine("   ⚠️ Campaign not found");
                    }

                    // 3. Get by ID (throws if not found)
                    Console.WriteLine("3️⃣ Get by ID (throws if not found):");
                    try
                    {
                        var campaign = await context.Campaigns.FindAsync(createdCampaign.Id);
                        Console.WriteLine($"   ✅ Got campaign: {campaign!.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ Error: {ex.Message}");
                    }
                }
                Console.WriteLine();

                // === UPDATE OPERATION ===
                Console.WriteLine("📝 === UPDATE OPERATION (PUT) ===");
                Console.WriteLine("Testing context.Campaigns.UpdateAsync()...");

                if (!string.IsNullOrEmpty(createdCampaign.Id))
                {
                    // Modify the campaign
                    createdCampaign.Budget = 90000.00m;
                    createdCampaign.Description = "Updated description via DbSet-like interface";
                    createdCampaign.Status = "Draft";

                    try
                    {
                        var updatedCampaign = await context.Campaigns.UpdateAsync(createdCampaign);
                        Console.WriteLine($"✅ Campaign updated: {updatedCampaign.Name}");
                        Console.WriteLine($"   💰 New Budget: {updatedCampaign.Budget:C}");
                        Console.WriteLine($"   📊 New Status: {updatedCampaign.Status}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Update failed: {ex.Message}");
                    }
                }
                Console.WriteLine();

                // === DELETE OPERATION ===
                Console.WriteLine("🗑️ === DELETE OPERATION (DELETE) ===");
                Console.WriteLine("Testing context.Campaigns.DeleteAsync()...");

                if (!string.IsNullOrEmpty(createdCampaign.Id))
                {
                    try
                    {
                        await context.Campaigns.DeleteAsync(createdCampaign.Id);
                        Console.WriteLine($"✅ Campaign deleted successfully");

                        // Verify deletion
                        var deletedCampaign = await context.Campaigns.FindAsync(createdCampaign.Id);
                        if (deletedCampaign == null)
                        {
                            Console.WriteLine("   ✅ Deletion verified - campaign no longer exists");
                        }
                        else
                        {
                            Console.WriteLine("   ⚠️ Campaign still exists after deletion");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Delete failed: {ex.Message}");
                    }
                }
                Console.WriteLine();

                // === SUMMARY ===
                Console.WriteLine("📊 === SUMMARY ===");
                Console.WriteLine("✅ All CRUD operations available through DbSet-like interface:");
                Console.WriteLine("   📖 Read: LINQ queries + FindAsync() + GetAsync()");
                Console.WriteLine("   🆕 Create: AddAsync()");
                Console.WriteLine("   📝 Update: UpdateAsync()");
                Console.WriteLine("   🗑️ Delete: DeleteAsync()");
                Console.WriteLine();
                Console.WriteLine("🎯 Benefits of DbSet-like Interface:");
                Console.WriteLine("   ✅ Familiar EF Core syntax");
                Console.WriteLine("   ✅ Type-safe operations");
                Console.WriteLine("   ✅ Single interface for all operations");
                Console.WriteLine("   ✅ Scalable - easy to add new entities");
                Console.WriteLine("   ✅ Clean and maintainable code");

                var stats = new Dictionary<string, object>
                {
                    ["🆕 Create"] = "✅ Supported",
                    ["📖 Read (LINQ)"] = "✅ Supported", 
                    ["📖 Read (Find)"] = "✅ Supported",
                    ["📖 Read (Get)"] = "✅ Supported",
                    ["📝 Update"] = "✅ Supported",
                    ["🗑️ Delete"] = "✅ Supported"
                };

                DemoConfiguration.PrintDemoFooter("CRUD Operations Demo", stats);
                Console.WriteLine("   🎯 DbSet-like interface provides complete CRUD functionality!");
                Console.WriteLine("   ✨ EF Core experience for HTTP APIs!");

            }
            catch (Exception ex)
            {
                DemoConfiguration.HandleDemoException(ex, "CRUD Operations Demo");
            }
            finally
            {
                await serviceProvider.DisposeAsync();
            }
        }
    }
}
