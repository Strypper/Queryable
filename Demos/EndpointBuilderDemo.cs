using Queryable.Core.Context;
using Queryable.Core.Sets;
using Queryable.Models;

namespace Queryable.Demos
{
    /// <summary>
    /// Demo showcasing the new EndpointBuilder pattern in action
    /// </summary>
    public class EndpointBuilderDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== EndpointBuilder Pattern Demo ===\n");

            // Setup options
            var options = new ApiContextOptions
            {
                BaseUrl = "https://api.lightninglanes.com",
                BearerToken = "your-api-token-here",
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Create context using the new pattern
            using var context = new LightningLanesApiContext(options);

            Console.WriteLine("✅ LightningLanesApiContext created with EndpointBuilder pattern");
            Console.WriteLine("✅ Endpoints registered via OnEndpointRegistering() hook");
            Console.WriteLine();

            // Demonstrate LINQ functionality - still works the same!
            Console.WriteLine("🔍 LINQ Queries work exactly the same:");
            Console.WriteLine("📋 Campaigns endpoint (no version): Expected URL like /campaigns");
            Console.WriteLine("📨 Messages endpoint (with v1): Expected URL like /api/v1/messages");
            Console.WriteLine();
            
            try
            {
                // These would make actual HTTP calls if the API was available
                var activeCampaigns = context.Campaigns
                    .Where(c => c.Status == "Active")
                    .OrderBy(c => c.CreatedDate)  // ✅ Campaign has CreatedDate
                    .Take(10);

                var recentMessages = context.Messages
                    .Where(m => m.CreatedAt > DateTime.Now.AddDays(-7))  // ✅ Message has CreatedAt
                    .OrderByDescending(m => m.CreatedAt);

                Console.WriteLine("   📋 Active campaigns query prepared");
                Console.WriteLine("   📨 Recent messages query prepared");
                Console.WriteLine("   ⚡ Ready to execute with .ToList(), .FirstOrDefault(), etc.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ℹ️  Queries prepared successfully (API not available: {ex.GetType().Name})");
            }

            Console.WriteLine();
            Console.WriteLine("🏗️ Smart Version Handling Demo:");
            Console.WriteLine("   ✅ Campaigns: No .WithVersion() → /campaigns");
            Console.WriteLine("   ✅ Messages: With .WithVersion('v1') → /api/v1/messages");
            Console.WriteLine("   ✅ Backwards compatible with existing APIs");
            Console.WriteLine("   ✅ Future-ready for versioned APIs");
            Console.WriteLine();
            Console.WriteLine("🏗️ Other Benefits of EndpointBuilder Pattern:");
            Console.WriteLine("   ✅ Separation of concerns (Base vs Derived)");
            Console.WriteLine("   ✅ Fluent API configuration");
            Console.WriteLine("   ✅ Per-endpoint customization");
            Console.WriteLine("   ✅ Easy to extend and maintain");
            Console.WriteLine("   ✅ Follows EF Core conventions");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Example of creating a different API context for another domain
    /// </summary>
    public class ECommerceApiContext : ApiContext
    {
        public ECommerceApiContext(ApiContextOptions options) : base(options) { }

        // Different business entities
        public IApiSet<Product> Products { get; private set; } = null!;
        public IApiSet<Order> Orders { get; private set; } = null!;
        public IApiSet<Customer> Customers { get; private set; } = null!;

        protected override void OnEndpointRegistering()
        {
            // Different endpoint configuration for e-commerce domain
            Products = RegisterEndpoint<Product>()
                .WithEndpoint("/api/v2/products")          // ✅ Full path with version
                .WithHeader("X-Store-Id", "main-store")
                .WithTimeout(TimeSpan.FromMinutes(1))
                .Build();
            // Result: /api/v2/products (user controls full path)

            Orders = RegisterEndpoint<Order>()
                .WithVersion("v1")                         // ✅ Version first
                .WithConventionNaming()                    // Then convention
                .WithHeader("X-Priority", "high")
                .Build();
            // Result: /api/v1/orders (smart convention + version)

            Customers = CreateApiSet<Customer>("/customers"); // Simple approach, no version
            // Result: /customers (simple legacy style)
        }
    }

    // Mock entities for demo
    public class Product { public int Id { get; set; } public string Name { get; set; } = ""; }
    public class Order { public int Id { get; set; } public DateTime CreatedDate { get; set; } }
    public class Customer { public int Id { get; set; } public string Email { get; set; } = ""; }
}
