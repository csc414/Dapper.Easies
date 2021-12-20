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

        private int _i = 0;

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
            Visit(exp);
            return _sql.ToString();
        }

        private void Clear()
        {
            _context = null;
            _sql = null;
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            var condition = b.NodeType != ExpressionType.AndAlso && b.NodeType != ExpressionType.OrElse;
            var operatorType = (OperatorType)b.NodeType;
            Visit(b.Left);
            if ((operatorType == OperatorType.Equal || operatorType == OperatorType.NotEqual) && b.Right is ConstantExpression constant && constant.Value == null)
            {
                var operatorStr = _sqlSyntax.Operator(operatorType == OperatorType.Equal ? OperatorType.EqualNull : OperatorType.NotEqualNull);
                if (operatorStr == null)
                    throw new NotImplementedException($"ExpressionType：{(ExpressionType)operatorType}");
                _sql.Append(operatorStr);
            }
            else
            {
                var operatorStr = _sqlSyntax.Operator(operatorType);
                if(operatorStr == null)
                    throw new NotImplementedException($"ExpressionType：{(ExpressionType)operatorType}");
                _sql.Append(operatorStr);
                Visit(b.Right);
            }
            
            void Visit(Expression exp)
            {
                var hasBracket = _i > 0 && exp is BinaryExpression;
                if (hasBracket)
                    _sql.Append("(");
                if (condition)
                    _i++;
                this.Visit(exp);
                if (condition)
                    _i--;
                if (hasBracket)
                    _sql.Append(")");
            }

            return b;
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression is ParameterExpression)
                _sql.AppendFormat("{0}.{1}", _context.Alias[m.Member.ReflectedType].Alias, DbObject.Get(m.Member.ReflectedType)[m.Member.Name].EscapeName);
            else
                AppendParameter(GetValue(m));

            return m;
        }

        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.IsStatic && typeof(DbFunction).IsAssignableFrom(m.Method.ReflectedType))
            {
                if (m.Arguments[0] is MemberExpression member && member.Expression is ParameterExpression)
                {
                    var field = string.Format("{0}.{1}", _context.Alias[member.Member.ReflectedType].Alias, DbObject.Get(member.Member.ReflectedType)[member.Member.Name].EscapeName);
                    var args = m.Arguments.Skip(1).Select(o => GetValue(o));
                    var result = _sqlSyntax.Method(m.Method, field, args.ToArray(), _parameters);
                    if (result == null)
                        throw new NotImplementedException($"MethodName：{m.Method.Name}");
                    _sql.Append(result);
                }
                else
                    throw new ArgumentException("自定义方法第一个字段必须是实体参数");
            }
            else
                AppendParameter(GetValue(m));

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
