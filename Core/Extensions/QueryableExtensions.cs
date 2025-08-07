using System.Linq;
using Queryable.Core.Queryable;

namespace Queryable.Core.Extensions
{
    /// <summary>
    /// Extension methods for IQueryable to support query style configuration
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Sets the query style for this queryable (OData or REST)
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="queryable">The queryable to configure</param>
        /// <param name="style">The query style to use</param>
        /// <returns>A new queryable with the specified query style</returns>
        public static IQueryable<T> UseQueryStyle<T>(this IQueryable<T> queryable, QueryStyle style)
        {
            if (queryable is ApiQueryable<T> apiQueryable)
            {
                return apiQueryable.UseQueryStyle(style);
            }
            
            // For other IQueryable implementations, return as-is
            // They won't support query style configuration
            return queryable;
        }
    }
}
