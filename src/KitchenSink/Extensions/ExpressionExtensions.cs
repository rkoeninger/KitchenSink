using System;
using System.Linq.Expressions;
using System.Reflection;

namespace KitchenSink.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets reflection info for method referred to in expression.
        /// </summary>
        public static MethodInfo GetMethod<A>(this Expression<A> expr) =>
            (expr.Body as MethodCallExpression)?.Method
                ?? throw new ArgumentException("Expression must be a method call");

        /// <summary>
        /// Gets reflection info for property referred to in expression.
        /// </summary>
        public static PropertyInfo GetProperty<A>(this Expression<A> expr) =>
            (expr.Body as MemberExpression)?.Member as PropertyInfo
                ?? throw new ArgumentException("Expression must be a property");
    }
}
