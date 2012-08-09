using System;
using System.Data.Objects;
using System.Linq.Expressions;

namespace RDC.Shared.Framework.Data
{
    public static class ObjectQueryExtensions
    {
        public static ObjectQuery<T> Include<T>(this ObjectQuery<T> query, Expression<Func<T, object>> includes)
            where T : class
        {
            return query.Include(GetExpressionName(includes.Body as MemberExpression));
        }

        public static ObjectQuery<T> Include<T>(this ObjectSet<T> query, Expression<Func<T, object>> includes)
            where T : class
        {
            return query.Include(GetExpressionName(includes.Body as MemberExpression));
        }

        static string GetExpressionName(MemberExpression memberExpression)
        {
            if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                return memberExpression.Member.Name;

            if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                return string.Format("{0}.{1}", GetExpressionName(memberExpression.Expression as MemberExpression), memberExpression.Member.Name);

            var methodCallExpression = (MethodCallExpression)memberExpression.Expression;
            if (methodCallExpression.Arguments.Count != 1)
                throw new Exception("invalid method call in Include expression");
            return string.Format("{0}.{1}", GetExpressionName(methodCallExpression.Arguments[0] as MemberExpression), memberExpression.Member.Name);
        }
    }
}
