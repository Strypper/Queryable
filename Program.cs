using Queryable.Core.Context;
using Queryable.Models;
using Queryable.Demos;

namespace Queryable
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Testing Custom LINQ Provider for HTTP APIs");
            Console.WriteLine("===============================================");
            Console.WriteLine("Choose testing approach:");
            Console.WriteLine("1. Manual setup (original approach)");
            Console.WriteLine("2. Campaign API Demo (EF Core style with DI)");
            Console.WriteLine("3. Message API Demo");
            Console.WriteLine("4. Combined Demo (Campaigns + Messages)");
            Console.WriteLine("5. ConsoleApp Demo (includes Add functionality)");
            Console.WriteLine("6. CRUD Demo (DbSet-like interface showcase)");
            Console.WriteLine("7. EndpointBuilder Pattern Demo ⭐ NEW!");
            Console.WriteLine("8. Version Handling Test 🧪 TEST!");
            Console.WriteLine("9. Query Style Demo (REST vs OData) 🆕 NEW!");
            Console.Write("Enter choice (1-9): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunManualSetupAsync();
                    break;
                case "2":
                    Console.WriteLine("🎯 Starting Campaign API Demo...");
                    await CampaignDemo.RunCampaignDemoAsync();
                    break;
                case "3":
                    Console.WriteLine("🎯 Starting Message API Demo...");
                    await MessageDemo.RunMessageDemoAsync();
                    break;
                case "4":
                    Console.WriteLine("🌟 Starting Combined Demo...");
                    await CombinedDemo.RunCombinedDemoAsync();
                    break;
                case "5":
                    Console.WriteLine("🆕 Starting ConsoleApp Demo (with Add functionality)...");
                    await ConsoleApp.RunWithDependencyInjectionAsync();
                    break;
                case "6":
                    Console.WriteLine("🔧 Starting CRUD Demo (DbSet-like interface)...");
                    await CrudDemo.RunCrudDemoAsync();
                    break;
                case "7":
                    Console.WriteLine("⭐ Starting EndpointBuilder Pattern Demo...");
                    EndpointBuilderDemo.RunDemo();
                    break;
                case "8":
                    Console.WriteLine("🧪 Starting Version Handling Test...");
                    VersionHandlingTest.TestVersionHandling();
                    break;
                case "9":
                    Console.WriteLine("🆕 Starting Query Style Demo...");
                    await RunQueryStyleDemo();
                    break;
                default:
                    Console.WriteLine("❌ Invalid choice. Running CRUD Demo by default...");
                    await CrudDemo.RunCrudDemoAsync();
                    break;
            }

            Console.WriteLine("\n✨ Demo completed! Press any key to exit...");
            
            // Handle both interactive and piped input scenarios
            try
            {
                if (Console.IsInputRedirected)
                {
                    // Input is redirected (like echo "2" | dotnet run)
                    Console.WriteLine("✅ Demo finished successfully!");
                }
                else
                {
                    // Interactive mode - wait for key press
                    Console.ReadKey();
                }
            }
            catch (InvalidOperationException)
            {
                // Fallback for environments where ReadKey is not supported
                Console.WriteLine("✅ Demo finished successfully!");
            }
        }

        private static void RunManualSetupAsync()
        {
            Console.WriteLine("\n🔧 Manual Setup Approach - Direct Context Usage");
            Console.WriteLine("===============================================");

            // Create HTTP client with SSL bypass for localhost testing
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            // Add Bearer token for authentication
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjI3NzM3M0I4RjZBQ0ZFRkJBNzQ1MDUyMTg2QzZENEU3IiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2lkZW50aXR5LXNlcnZlci1leWdzZXlnNmhhZjJmNmFmLmVhc3Rhc2lhLTAxLmF6dXJld2Vic2l0ZXMubmV0IiwibmJmIjoxNzU0NDY3MDY3LCJpYXQiOjE3NTQ0NjcwNjcsImV4cCI6MTc1NzA1OTA2Nywic2NvcGUiOlsibGlnaHRuaW5nLWxhbmVzLWFwcCIsIm9wZW5pZCIsInByb2ZpbGUiXSwiYW1yIjpbInB3ZCJdLCJjbGllbnRfaWQiOiJwb3N0bWFuIiwic3ViIjoiOWNiMDE0ZDMtNzBhZi00YzkzLTgwNDMtMmEzMTRkOGE5MGY2IiwiYXV0aF90aW1lIjoxNzU0NDY3MDY3LCJpZHAiOiJsb2NhbCIsImp0aSI6IkEyMDg0MjkyRjAzRjQxOUI2OUI3QzMxN0MzODc4MzE0In0.CiBP9JEtW_G_RAepcqhbuhC_nhukmq17HJevA_yv6aYEkelugNE5jIgHaiWtjLqa22duP-uW7KVz-7mMrUV5wD1bBMuLe2OtNl8Ml8LfPX64ImuqKZVIhqrLoFo15sCDMAPl26vg8w2gExUOmvDSKGBVD8pkJ5aBJN0eByslV8IuFPl-ObMivaRqHd4d7VQ_A0beycimVLizraBgknR9bgRsq8w5Ylq__NNCjyPbQVbHKpIJajJvHYcQW9eBJlaMWG8RqWimLdF7eE6D5wDg1GWj4fwQAyBwQp1SWzL5tP4KouZcI-DT1TURwrbzw7rejyiUZ0faMgTmoJl9Uim1nA";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Create API context with options
            var options = new ApiContextOptions
            {
                BaseUrl = "https://localhost:7263/api",
                BearerToken = token,
                Timeout = TimeSpan.FromSeconds(30),
                IgnoreSslErrors = true
            };

            using var context = new LightningLanesApiContext(options, httpClient);

            try
            {
                Console.WriteLine("\n1️⃣ Testing basic query (get all campaigns):");
                var allCampaigns = context.Campaigns.ToList();
                Console.WriteLine($"   Found {allCampaigns.Count} campaigns");
                
                // Display first few campaigns
                foreach (var campaign in allCampaigns.Take(3))
                {
                    Console.WriteLine($"   - {campaign.Name}: {campaign.Budget:C} ({campaign.Status})");
                }

                Console.WriteLine("\n2️⃣ Testing filtered query (active campaigns):");
                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .ToList();
                Console.WriteLine($"   Found {activeCampaigns.Count} active campaigns");

                Console.WriteLine("\n3️⃣ Testing complex query (expensive campaigns):");
                var expensiveCampaigns = context.Campaigns
                    .Where(c => c.Budget > 100000)
                    .OrderByDescending(c => c.Budget)
                    .Take(5)
                    .ToList();
                Console.WriteLine($"   Found {expensiveCampaigns.Count} expensive campaigns (>$100K):");
                foreach (var campaign in expensiveCampaigns)
                {
                    Console.WriteLine($"   💰 {campaign.Name}: {campaign.Budget:C}");
                }

                Console.WriteLine("\n4️⃣ Testing message queries:");
                var allMessages = context.Messages.ToList();
                Console.WriteLine($"   Found {allMessages.Count} total messages");

                var recentMessages = context.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(3)
                    .ToList();
                Console.WriteLine($"   Recent messages:");
                foreach (var message in recentMessages)
                {
                    var userName = message.User?.Name ?? "Unknown User";
                    Console.WriteLine($"   📝 {userName}: \"{message.Content}\"");
                }

                Console.WriteLine("\n✨ Manual setup completed successfully!");
                Console.WriteLine("🎯 Clean Context pattern - Direct LINQ access!");
                Console.WriteLine("✨ No unnecessary wrapper layers!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during manual setup: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
            }
        }

        public static async Task RunQueryStyleDemo()
        {
            try
            {
                // Tạo context với options
                var options = new ApiContextOptions
                {
                    BaseUrl = "https://api.lightninglanes.com/api"
                };

                // Create demo context (không gọi API thật)
                using var httpClient = new HttpClient();
                var context = new LightningLanesApiContext(options, httpClient);

                // Tạo và chạy demo
                var demo = new QueryStyleDemo(context);
                await demo.RunDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Query Style Demo: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
            }
        }
    }
}

