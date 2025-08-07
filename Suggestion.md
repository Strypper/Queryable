Tôi thấy vấn đề bạn đang gặp! Khi có nhiều entities và nhiều HTTP methods (GET, POST, PUT, DELETE), code sẽ trở nên rất dài và khó quản lý. Đây là những design patterns tôi đề xuất để cải thiện:

## 🎯 Design Pattern Recommendations

### 1. **Generic Repository Pattern với DbSet-like Interface**

```csharp
// Tạo interface giống DbSet trong EF Core
public interface IApiSet<T> : IQueryable<T>
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(string id);
    Task<T?> FindAsync(string id);
}

// Implementation
public class ApiSet<T> : ApiQueryable<T>, IApiSet<T>
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    
    public async Task<T> AddAsync(T entity) { /* POST logic */ }
    public async Task<T> UpdateAsync(T entity) { /* PUT logic */ }
    public async Task DeleteAsync(string id) { /* DELETE logic */ }
    public async Task<T?> FindAsync(string id) { /* GET by ID logic */ }
}

// ApiContext trở nên clean hơn
public class ApiContext : IDisposable
{
    public IApiSet<Campaign> Campaigns { get; }
    public IApiSet<Message> Messages { get; }
    public IApiSet<User> Users { get; }
    // Chỉ cần khai báo properties, logic nằm trong ApiSet
}
```

### 2. **Repository Factory Pattern**

```csharp
public interface IApiRepository<T>
{
    IQueryable<T> Query { get; }
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(string id);
    Task<T?> GetByIdAsync(string id);
}

public class ApiRepositoryFactory
{
    private readonly HttpClient _httpClient;
    private readonly ApiContextOptions _options;
    
    public IApiRepository<T> CreateRepository<T>(string endpoint)
    {
        return new ApiRepository<T>(_httpClient, $"{_options.BaseUrl}/{endpoint}");
    }
}

// ApiContext sử dụng factory
public class ApiContext : IDisposable
{
    private readonly ApiRepositoryFactory _factory;
    
    public IApiRepository<Campaign> Campaigns => _factory.CreateRepository<Campaign>("campaigns");
    public IApiRepository<Message> Messages => _factory.CreateRepository<Message>("messages");
    public IApiRepository<User> Users => _factory.CreateRepository<User>("users");
}
```

### 3. **Fluent API Pattern với Method Chaining**

```csharp
public class ApiContext : IDisposable
{
    public IApiEntityBuilder<Campaign> Campaigns => CreateEntity<Campaign>("campaigns");
    public IApiEntityBuilder<Message> Messages => CreateEntity<Message>("messages");
    
    private IApiEntityBuilder<T> CreateEntity<T>(string endpoint)
    {
        return new ApiEntityBuilder<T>(_httpClient, $"{_options.BaseUrl}/{endpoint}");
    }
}

public interface IApiEntityBuilder<T> : IQueryable<T>
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(string id);
    Task<T?> FindAsync(string id);
    
    // Fluent methods
    IApiEntityBuilder<T> WithTimeout(TimeSpan timeout);
    IApiEntityBuilder<T> WithHeaders(Dictionary<string, string> headers);
}
```

### 4. **Extension Methods Pattern**

```csharp
// Extension methods cho IQueryable
public static class ApiQueryableExtensions
{
    public static async Task<T> AddAsync<T>(this IQueryable<T> source, T entity)
    {
        if (source is ApiQueryable<T> apiQueryable)
        {
            return await apiQueryable.Provider.AddAsync(entity);
        }
        throw new NotSupportedException();
    }
    
    public static async Task<T> UpdateAsync<T>(this IQueryable<T> source, T entity)
    {
        // Similar implementation
    }
    
    public static async Task DeleteAsync<T>(this IQueryable<T> source, string id)
    {
        // Similar implementation  
    }
}

// Usage trở nên rất clean
var campaign = await context.Campaigns.AddAsync(newCampaign);
var updated = await context.Campaigns.UpdateAsync(existingCampaign);
await context.Campaigns.DeleteAsync("campaign-id");
```

### 5. **Command Pattern với Generic Commands**

```csharp
public interface IApiCommand<T, TResult>
{
    Task<TResult> ExecuteAsync(T input);
}

public class AddEntityCommand<T> : IApiCommand<T, T>
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    
    public async Task<T> ExecuteAsync(T entity)
    {
        // POST logic here
    }
}

public class ApiContext : IDisposable
{
    public IQueryable<Campaign> Campaigns { get; }
    public IQueryable<Message> Messages { get; }
    
    // Generic command executor
    public async Task<T> ExecuteAsync<T>(IApiCommand<T, T> command)
    {
        return await command.ExecuteAsync();
    }
    
    // Or helper methods
    public async Task<T> AddAsync<T>(T entity, string endpoint)
    {
        var command = new AddEntityCommand<T>(_httpClient, endpoint);
        return await command.ExecuteAsync(entity);
    }
}
```

## 🏆 **Recommended Solution: Hybrid Approach**

Tôi khuyến nghị kết hợp **ApiSet Pattern** + **Extension Methods**:

```csharp
public class ApiContext : IDisposable
{
    // Clean property declarations
    public IApiSet<Campaign> Campaigns { get; }
    public IApiSet<Message> Messages { get; }
    public IApiSet<User> Users { get; }
    // ... có thể có 100 entities mà vẫn clean
    
    public ApiContext(ApiContextOptions options)
    {
        // Factory-based initialization
        Campaigns = CreateApiSet<Campaign>("campaigns");
        Messages = CreateApiSet<Message>("messages");
        Users = CreateApiSet<User>("users");
    }
    
    private IApiSet<T> CreateApiSet<T>(string endpoint)
    {
        return new ApiSet<T>(_httpClient, $"{_options.BaseUrl}/{endpoint}");
    }
}
```

## ✅ **Benefits của approach này:**

1. **Scalable**: Dễ dàng thêm entities mới chỉ với 1 dòng code
2. **Clean**: ApiContext vẫn ngắn gọn dù có nhiều entities
3. **Type-safe**: Compile-time checking
4. **EF Core-like**: Syntax giống hệt EF Core
5. **Maintainable**: Logic tách biệt, dễ test và maintain
6. **Flexible**: Có thể customize per entity nếu cần

## 🎯 **Usage Examples:**

```csharp
// Vẫn giữ syntax hiện tại
var campaigns = await context.Campaigns.Where(c => c.Status == "Active").ToListAsync();

// Thêm các operations khác
var newCampaign = await context.Campaigns.AddAsync(campaign);
var updated = await context.Campaigns.UpdateAsync(campaign);
await context.Campaigns.DeleteAsync("id");
var found = await context.Campaigns.FindAsync("id");
```

Approach này sẽ giải quyết được vấn đề scalability mà vẫn giữ được clean architecture và EF Core-like experience!