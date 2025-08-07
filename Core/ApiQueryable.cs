using System.Collections;
using System.Linq.Expressions;

namespace Queryable.Core
{
    public class ApiQueryable<T> : IOrderedQueryable<T>
    {
        public ApiQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public ApiQueryable(IQueryProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = Expression.Constant(this);
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            // Khi GetEnumerator được gọi (ví dụ: ToList(), foreach), 
            // chúng ta yêu cầu Provider thực thi truy vấn
            var result = Provider.Execute<IEnumerable<T>>(Expression);
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
