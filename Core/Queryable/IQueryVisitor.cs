namespace Queryable.Core.Queryable
{
    /// <summary>
    /// Interface for query visitors that convert LINQ expressions to HTTP query strings
    /// </summary>
    public interface IQueryVisitor
    {
        /// <summary>
        /// Converts the visited expression tree to a query string
        /// </summary>
        /// <returns>HTTP query string parameters</returns>
        string ToQueryString();
    }

    /// <summary>
    /// Supported query string styles for API calls
    /// </summary>
    public enum QueryStyle
    {
        /// <summary>
        /// REST API standard with search, sort, pageIndex, pageSize parameters (Default)
        /// </summary>
        Rest,
        
        /// <summary>
        /// OData standard with $filter, $orderby, $skip, $top parameters
        /// </summary>
        OData
    }
}
