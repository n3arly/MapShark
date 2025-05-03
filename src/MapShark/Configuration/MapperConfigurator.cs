using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapShark.Configuration
{
    public class MapperConfigurator<TSource, TDestination>
    {
        private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>();

        /// <summary>
        /// Specifies a mapping from a source property to a destination property.
        /// </summary>
        /// <param name="destinationSelector">
        /// An expression selecting the destination property of <typeparamref name="TDestination"/> to map to.
        /// </param>
        /// <param name="sourceSelector">
        /// An expression selecting the source property of <typeparamref name="TSource"/> to map from.
        /// </param>
        /// <returns>
        /// The same <see cref="MapperConfigurator{TSource,TDestination}"/> instance to allow fluent chaining of mappings.
        /// </returns>
        public MapperConfigurator<TSource, TDestination> Map(Expression<Func<TDestination, object>> destinationSelector, Expression<Func<TSource, object>> sourceSelector)
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
