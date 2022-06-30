using System.Collections.Generic;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (map.TryGetValue(p, out var replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null)
                return expr2;

            var body = ParameterRebinder.ReplaceParameters(
                expr2.Parameters.Select((p, i) => new { p, f = expr1.Parameters[i] }).ToDictionary(p => p.p, p => p.f), expr2.Body);
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, body), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null)
                return expr2;

            var body = ParameterRebinder.ReplaceParameters(
                expr2.Parameters.Select((p, i) => new { p, f = expr1.Parameters[i] }).ToDictionary(p => p.p, p => p.f), expr2.Body);
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(expr1.Body, body), expr1.Parameters);
        }
    }
}
