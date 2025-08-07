using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Queryable.Core.Queryable
{
    /// <summary>
    /// OData-style query visitor that converts LINQ expressions to OData query parameters
    /// </summary>
    public class ExpressionToOdataQueryVisitor : ExpressionVisitor, IQueryVisitor
    {
        private readonly StringBuilder _filterBuilder = new();
        private readonly StringBuilder _orderBuilder = new();
        private int _skip = 0;
        private int _take = int.MaxValue;
        private bool _hasWhere = false;

        public string ToQueryString()
        {
            var queryParams = new List<string>();

            if (_filterBuilder.Length > 0)
            {
                queryParams.Add($"$filter={HttpUtility.UrlEncode(_filterBuilder.ToString())}");
            }

            if (_orderBuilder.Length > 0)
            {
                queryParams.Add($"$orderby={HttpUtility.UrlEncode(_orderBuilder.ToString())}");
            }

            if (_skip > 0)
            {
                queryParams.Add($"$skip={_skip}");
            }

            if (_take != int.MaxValue)
            {
                queryParams.Add($"$top={_take}");
            }

            return string.Join("&", queryParams);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Handle LINQ methods first
            switch (node.Method.Name)
            {
                case "Where":
                    if (node.Arguments.Count >= 2)
                    {
                        if (_hasWhere) _filterBuilder.Append(" and ");
                        _hasWhere = true;
                        
                        // Extract lambda from UnaryExpression
                        var whereArgument = node.Arguments[1];
                        LambdaExpression lambda;
                        
                        if (whereArgument is UnaryExpression unary && unary.Operand is LambdaExpression)
                        {
                            lambda = (LambdaExpression)unary.Operand;
                        }
                        else if (whereArgument is LambdaExpression directLambda)
                        {
                            lambda = directLambda;
                        }
                        else
                        {
                            Console.WriteLine($"❌ Unexpected Where argument type: {whereArgument.GetType()}");
                            return Visit(node.Arguments[0]);
                        }
                        
                        // Process the lambda body
                        ProcessWhereExpression(lambda.Body);
                    }
                    return Visit(node.Arguments[0]); // Source

                case "OrderBy":
                case "OrderByDescending":
                    if (node.Arguments.Count >= 2)
                    {
                        if (_orderBuilder.Length > 0) _orderBuilder.Append(", ");
                        
                        // Extract property name from lambda
                        var lambda = (LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand;
                        var propertyName = GetPropertyName(lambda.Body);
                        
                        _orderBuilder.Append(propertyName);
                        if (node.Method.Name == "OrderByDescending")
                        {
                            _orderBuilder.Append(" desc");
                        }
                    }
                    return Visit(node.Arguments[0]);

                case "ThenBy":
                case "ThenByDescending":
                    if (node.Arguments.Count >= 2)
                    {
                        if (_orderBuilder.Length > 0) _orderBuilder.Append(", ");
                        
                        var lambda = (LambdaExpression)((UnaryExpression)node.Arguments[1]).Operand;
                        var propertyName = GetPropertyName(lambda.Body);
                        
                        _orderBuilder.Append(propertyName);
                        if (node.Method.Name == "ThenByDescending")
                        {
                            _orderBuilder.Append(" desc");
                        }
                    }
                    return Visit(node.Arguments[0]);

                case "Skip":
                    if (node.Arguments.Count >= 2 && node.Arguments[1] is ConstantExpression skipConst)
                    {
                        _skip = (int)skipConst.Value!;
                    }
                    return Visit(node.Arguments[0]);

                case "Take":
                    if (node.Arguments.Count >= 2 && node.Arguments[1] is ConstantExpression takeConst)
                    {
                        _take = (int)takeConst.Value!;
                    }
                    return Visit(node.Arguments[0]);
            }

            // Handle string methods like Contains, StartsWith, EndsWith
            if (node.Method.DeclaringType == typeof(string))
            {
                var propertyName = GetPropertyName(node.Object);
                var value = GetConstantValue(node.Arguments[0]);

                switch (node.Method.Name)
                {
                    case "Contains":
                        _filterBuilder.Append($"contains({propertyName}, {value})");
                        return node;
                    case "StartsWith":
                        _filterBuilder.Append($"startswith({propertyName}, {value})");
                        return node;
                    case "EndsWith":
                        _filterBuilder.Append($"endswith({propertyName}, {value})");
                        return node;
                }
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso)
            {
                Visit(node.Left);
                _filterBuilder.Append(" and ");
                Visit(node.Right);
                return node;
            }
            else if (node.NodeType == ExpressionType.OrElse)
            {
                _filterBuilder.Append("(");
                Visit(node.Left);
                _filterBuilder.Append(" or ");
                Visit(node.Right);
                _filterBuilder.Append(")");
                return node;
            }

            // Handle comparison operators
            var left = GetPropertyName(node.Left);
            var right = GetConstantValue(node.Right);

            var operatorStr = node.NodeType switch
            {
                ExpressionType.Equal => "eq",
                ExpressionType.NotEqual => "ne",
                ExpressionType.GreaterThan => "gt",
                ExpressionType.GreaterThanOrEqual => "ge",
                ExpressionType.LessThan => "lt",
                ExpressionType.LessThanOrEqual => "le",
                _ => "eq"
            };

            _filterBuilder.Append($"{left} {operatorStr} {right}");
            return node;
        }

        private void ProcessWhereExpression(Expression expression)
        {
            switch (expression)
            {
                case BinaryExpression binary:
                    VisitBinary(binary);
                    break;
                    
                case MethodCallExpression method when method.Method.Name == "Contains":
                    // Handle string.Contains()
                    if (method.Object is MemberExpression member && method.Arguments.Count > 0)
                    {
                        var propertyName = GetPropertyName(member);
                        var value = GetConstantValue(method.Arguments[0]);
                        _filterBuilder.Append($"contains({propertyName}, {value})");
                    }
                    break;
                    
                default:
                    Console.WriteLine($"⚠️ Unsupported Where expression type: {expression.GetType()}");
                    break;
            }
        }

        private string GetPropertyName(Expression? expression)
        {
            return expression switch
            {
                MemberExpression member => member.Member.Name.ToLowerInvariant(),
                UnaryExpression unary when unary.Operand is MemberExpression memberExpr => 
                    memberExpr.Member.Name.ToLowerInvariant(),
                _ => "unknown"
            };
        }

        private string GetConstantValue(Expression? expression)
        {
            if (expression is ConstantExpression constant)
            {
                if (constant.Value is string str)
                    return $"'{str}'";
                if (constant.Value is DateTime dt)
                    return $"'{dt:yyyy-MM-ddTHH:mm:ss}'";
                return constant.Value?.ToString() ?? "null";
            }

            if (expression is MemberExpression member && member.Expression is ConstantExpression constExpr)
            {
                var container = constExpr.Value;
                var value = member.Member switch
                {
                    System.Reflection.FieldInfo field => field.GetValue(container),
                    System.Reflection.PropertyInfo prop => prop.GetValue(container),
                    _ => null
                };

                if (value is string str)
                    return $"'{str}'";
                if (value is DateTime dt)
                    return $"'{dt:yyyy-MM-ddTHH:mm:ss}'";
                return value?.ToString() ?? "null";
            }

            return "null";
        }
    }
}
