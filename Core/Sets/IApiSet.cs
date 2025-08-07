using System.Linq.Expressions;

namespace Queryable.Core.Sets
{
    /// <summary>
    /// DbSet-like interface for API entities, combining IQueryable with CRUD operations
    /// Similar to EF Core's DbSet but for HTTP API calls
    /// </summary>
    public interface IApiSet<T> : IQueryable<T>
    {
        /// <summary>
        /// Add a new entity (POST)
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Update an existing entity (PUT)
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Delete an entity by ID (DELETE)
        /// </summary>
        Task DeleteAsync(string id);

        /// <summary>
        /// Find an entity by ID (GET by ID)
        /// </summary>
        Task<T?> FindAsync(string id);

        /// <summary>
        /// Get entity by ID (throws if not found)
        /// </summary>
        Task<T> GetAsync(string id);
    }
}
