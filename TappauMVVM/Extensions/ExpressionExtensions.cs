using System;
using System.Linq.Expressions;
using TappauMVVM.Properties;

namespace TappauMVVM.Extensions
{
    public static class ExpressionExtensions
    {
        public static string NameForProperty<TDelegate>(this Expression<TDelegate> propertyExpression)
        {
            var body = propertyExpression.Body is UnaryExpression expression
                ? expression.Operand
                : propertyExpression.Body;

            if (!(body is MemberExpression member))
            {
                throw new ArgumentException(Resources.ExpressionExtensions_PropertyMustBeMemberExpression);
            }

            return member.Member.Name;
        }
    }
}