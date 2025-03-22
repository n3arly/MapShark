using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapShark.Configuration
{
    public class MapperConfigurator<TSource, TDestination>
    {
        private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>();

        public MapperConfigurator<TSource, TDestination> Map(Expression<Func<TSource, object>> sourceSelector, Expression<Func<TDestination, object>> destinationSelector)
        {
            string sourcePropertyName = GetPropertyName(sourceSelector);
            string destinationPropertyName = GetPropertyName(destinationSelector);
            _mappings[sourcePropertyName] = destinationPropertyName;
            return this;
        }

        private string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            MemberExpression memberExpression = null;

            if (expr.Body.NodeType == ExpressionType.Convert)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expr.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else if (expr.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expr.Body as MemberExpression;
            }

            if (memberExpression == null)
                throw new ArgumentException("Expression is not a member access", nameof(expr));

            return memberExpression.Member.Name;
        }

        public Dictionary<string, string> GetMappings() => _mappings;
    }
}
