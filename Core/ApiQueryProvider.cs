using System.Linq.Expressions;
using Newtonsoft.Json;
using Queryable.Models;

namespace Queryable.Core
{
    public class ApiQueryProvider : IQueryProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiQueryProvider(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = GetElementType(expression.Type);
            try
            {
                var queryableType = typeof(ApiQueryable<>).MakeGenericType(elementType);
                return (IQueryable)Activator.CreateInstance(queryableType, this, expression)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not create query for type {elementType}", ex);
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new ApiQueryable<TElement>(this, expression);
        }

        public object? Execute(Expression expression)
        {
            return Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Console.WriteLine("🔍 Executing LINQ expression...");
            
            // Dịch Expression Tree thành HTTP query string
            var visitor = new ExpressionToHttpQueryVisitor();
            visitor.Visit(expression);
            var queryString = visitor.ToQueryString();

            // Xây dựng URL đầy đủ
            var requestUrl = string.IsNullOrEmpty(queryString) 
                ? _baseUrl 
                : $"{_baseUrl}?{queryString}";

            Console.WriteLine($"🌐 Generated URL: {requestUrl}");

            try
            {
                // Gọi API HTTP
                var response = _httpClient.GetStringAsync(requestUrl).Result;
                Console.WriteLine($"✅ API Response received: {response.Length} characters");

                // Check if TResult is IEnumerable<T> (typical case for LINQ queries)
                var resultType = typeof(TResult);
                if (resultType.IsGenericType && 
                    typeof(IEnumerable<>).IsAssignableFrom(resultType.GetGenericTypeDefinition()))
                {
                    // Get the element type (T in IEnumerable<T>)
                    var elementType = resultType.GetGenericArguments()[0];
                    
                    // Create ApiResponse<T> type
                    var apiResponseType = typeof(ApiResponse<>).MakeGenericType(elementType);
                    
                    // Deserialize to ApiResponse<T>
                    var apiResponse = JsonConvert.DeserializeObject(response, apiResponseType);
                    
                    // Get the Data property
                    var dataProperty = apiResponseType.GetProperty("Data");
                    var data = dataProperty?.GetValue(apiResponse);
                    
                    Console.WriteLine($"📦 Extracted data from ApiResponse: {data?.GetType().Name}");
                    
                    // Cast to TResult
                    return (TResult)data!;
                }
                else
                {
                    // Direct deserialization for other types
                    var result = JsonConvert.DeserializeObject<TResult>(response);
                    Console.WriteLine($"📦 Deserialized to: {typeof(TResult).Name}");
                    return result!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling API: {ex.Message}");
                throw new InvalidOperationException($"Failed to execute query against API: {ex.Message}", ex);
            }
        }

        private static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            return ienum?.GetGenericArguments()[0] ?? seqType;
        }

        private static Type? FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType()!);

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                        return ienum;
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type? ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                return FindIEnumerable(seqType.BaseType);

            return null;
        }
    }
}
