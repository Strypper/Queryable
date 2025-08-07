using Queryable.Core.Context;
using Queryable.Core.Sets;
using Queryable.Models;

namespace Queryable.Demos
{
    /// <summary>
    /// Test to verify smart version handling URLs
    /// </summary>
    public class VersionHandlingTest
    {
        public static void TestVersionHandling()
        {
            Console.WriteLine("=== Smart Version Handling Test ===\n");

            var options = new ApiContextOptions
            {
                BaseUrl = "https://api.example.com",
                BearerToken = "test-token"
            };

            using var context = new TestApiContext(options);

            Console.WriteLine("✅ TestApiContext created with different versioning scenarios");
            Console.WriteLine("📋 Check the URLs generated in ApiSet constructor");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Test context to demonstrate different versioning scenarios
    /// </summary>
    public class TestApiContext : ApiContext
    {
        public TestApiContext(ApiContextOptions options) : base(options) { }

        public IApiSet<Campaign> NoVersionCampaigns { get; private set; } = null!;
        public IApiSet<Message> VersionedMessages { get; private set; } = null!;
        public IApiSet<Campaign> ConventionNoVersion { get; private set; } = null!;
        public IApiSet<Message> ConventionWithVersion { get; private set; } = null!;
        public IApiSet<Campaign> FullPathControl { get; private set; } = null!;

        protected override void OnEndpointRegistering()
        {
            Console.WriteLine("🔧 Registering endpoints with different version scenarios:");

            // Test 1: No version
            NoVersionCampaigns = RegisterEndpoint<Campaign>()
                .WithEndpoint("/campaigns")  // ❌ No WithVersion()
                .Build();
            Console.WriteLine("   1️⃣ No version: /campaigns");

            // Test 2: With version
            VersionedMessages = RegisterEndpoint<Message>()
                .WithEndpoint("/messages")
                .WithVersion("v1")  // ✅ With version
                .Build();
            Console.WriteLine("   2️⃣ With version: /messages + v1 → /api/v1/messages");

            // Test 3: Convention without version
            ConventionNoVersion = RegisterEndpoint<Campaign>()
                .WithConventionNaming()  // ❌ No WithVersion()
                .Build();
            Console.WriteLine("   3️⃣ Convention no version: → /api/campaigns");

            // Test 4: Convention with version
            ConventionWithVersion = RegisterEndpoint<Message>()
                .WithVersion("v2")  // ✅ With version
                .WithConventionNaming()
                .Build();
            Console.WriteLine("   4️⃣ Convention with version: v2 + convention → /api/v2/messages");

            // Test 5: Full path control
            FullPathControl = RegisterEndpoint<Campaign>()
                .WithEndpoint("/api/v3/special/campaigns")  // ✅ Full path
                .WithVersion("v1")  // ❌ Should be ignored
                .Build();
            Console.WriteLine("   5️⃣ Full path control: /api/v3/special/campaigns (version ignored)");

            Console.WriteLine();
        }
    }
}
