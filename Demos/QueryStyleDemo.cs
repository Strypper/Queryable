using Queryable.Core.Queryable;
using Queryable.Core.Extensions;
using Queryable.Models;

namespace Queryable.Demos
{
    public class QueryStyleDemo
    {
        private readonly LightningLanesApiContext _context;

        public QueryStyleDemo(LightningLanesApiContext context)
        {
            _context = context;
        }

        public Task RunDemo()
        {
            Console.WriteLine("🎯 Query Style Demo - REST vs OData");
            Console.WriteLine("=====================================\n");

            // Demo 1: Default REST style
            Console.WriteLine("📊 Demo 1: Default REST Style");
            Console.WriteLine("Query: campaigns.Where(x => x.Name.Contains(\"test\")).Skip(20).Take(10)");
            
            var restQuery = _context.Campaigns
                .Where(x => x.Name.Contains("test"))
                .Skip(20)
                .Take(10);
            
            Console.WriteLine($"Expected URL: /campaigns?search=test&pageIndex=3&pageSize=10");
            Console.WriteLine();

            // Demo 2: Explicit REST style
            Console.WriteLine("📊 Demo 2: Explicit REST Style");
            Console.WriteLine("Query: campaigns.UseQueryStyle(QueryStyle.Rest).Where(x => x.Status == \"active\").OrderBy(x => x.Name)");
            
            var explicitRestQuery = _context.Campaigns
                .UseQueryStyle(QueryStyle.Rest)
                .Where(x => x.Status == "active")
                .OrderBy(x => x.Name);
            
            Console.WriteLine($"Expected URL: /campaigns?status=active&sort=name_asc");
            Console.WriteLine();

            // Demo 3: OData style
            Console.WriteLine("📊 Demo 3: OData Style");
            Console.WriteLine("Query: campaigns.UseQueryStyle(QueryStyle.OData).Where(x => x.Name.Contains(\"project\")).Skip(40).Take(20)");
            
            var odataQuery = _context.Campaigns
                .UseQueryStyle(QueryStyle.OData)
                .Where(x => x.Name.Contains("project"))
                .Skip(40)
                .Take(20);
            
            Console.WriteLine($"Expected URL: /campaigns?$filter=contains(name,'project')&$skip=40&$top=20");
            Console.WriteLine();

            // Demo 4: Complex OData query
            Console.WriteLine("📊 Demo 4: Complex OData Query");
            Console.WriteLine("Query: campaigns.UseQueryStyle(QueryStyle.OData).Where(x => x.Status == \"active\" && x.Budget > 1000).OrderByDescending(x => x.CreatedDate)");
            
            var complexODataQuery = _context.Campaigns
                .UseQueryStyle(QueryStyle.OData)
                .Where(x => x.Status == "active" && x.Budget > 1000)
                .OrderByDescending(x => x.CreatedDate);
            
            Console.WriteLine($"Expected URL: /campaigns?$filter=status eq 'active' and budget gt 1000&$orderby=createddate desc");
            Console.WriteLine();

            // Demo 5: Mixed styles in same context
            Console.WriteLine("📊 Demo 5: Mixed Styles in Same Context");
            Console.WriteLine("REST for Campaigns, OData for Messages");
            
            var mixedRest = _context.Campaigns
                .UseQueryStyle(QueryStyle.Rest)
                .Where(x => x.Name.Contains("marketing"));
                
            var mixedOData = _context.Messages
                .UseQueryStyle(QueryStyle.OData)
                .Where(x => x.Content.Contains("urgent"));
            
            Console.WriteLine($"Campaign URL: /campaigns?search=marketing");
            Console.WriteLine($"Message URL: /messages?$filter=contains(content,'urgent')");
            Console.WriteLine();

            Console.WriteLine("✅ Query Style Demo completed!");
            Console.WriteLine("Note: URLs are generated but not executed in this demo.");
            
            return Task.CompletedTask;
        }
    }
}
