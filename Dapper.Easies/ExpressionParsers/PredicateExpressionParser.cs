using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    internal class PredicateExpressionParser : ExpressionParser
    {
        private readonly ISqlSyntax _sqlSyntax;

        private QueryContext _context;

        private StringBuilder _sql;

        private ParameterBuilder _parameters;

        private HashSet<string> _lambdaParameters;

        internal PredicateExpressionParser(ISqlSyntax sqlSyntax, ParameterBuilder parameterBuilder)
        {
            _sqlSyntax = sqlSyntax;
            _parameters = parameterBuilder;
        }

        internal string ToSql(Expression exp, QueryContext context)
        {
            if (exp == null)
                return null;

            Clear();
            _context = context;
            _sql = new StringBuilder();
            _lambdaParameters = ((LambdaExpression)exp).Parameters.Select(o => o.Name).ToHashSet();
            Visit(exp);
            return _sql.ToString();
        }

        private void Clear()
        {
            _context = null;
            _sql = null;
            _lambdaParameters = null;
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);
            _sql.Append(_sqlSyntax.Operator((OperatorType)b.NodeType));
            Visit(b.Right);
            return b;
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression is ParameterExpression parameter && _lambdaParameters.Contains(parameter.Name))
                _sql.AppendFormat("{0}.{1}", _context.Alias[m.Member.ReflectedType].Alias, DbObject.Get(m.Member.ReflectedType)[m.Member.Name].EscapeName);
            else
                AppendParameter(GetPropertyValue(m));

            return m;
        }

        internal override Expression VisitConstant(ConstantExpression c)
        {
            AppendParameter(c.Value);
            return c;
        }

        void AppendParameter(object value)
        {
            _sql.Append(_parameters.AddParameter(value));
        }
    }
}
