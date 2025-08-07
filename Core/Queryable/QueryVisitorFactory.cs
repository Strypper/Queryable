namespace Queryable.Core.Queryable
{
    /// <summary>
    /// Factory for creating appropriate query visitors based on query style
    /// </summary>
    public static class QueryVisitorFactory
    {
        /// <summary>
        /// Creates a query visitor instance based on the specified style
        /// </summary>
        /// <param name="style">The query style to use</param>
        /// <returns>An appropriate query visitor implementation</returns>
        public static IQueryVisitor Create(QueryStyle style)
        {
            return style switch
            {
                QueryStyle.Rest => new ExpressionToRestQueryVisitor(),
                QueryStyle.OData => new ExpressionToOdataQueryVisitor(),
                _ => new ExpressionToRestQueryVisitor() // Default to REST
            };
        }
    }
}
