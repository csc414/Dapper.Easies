﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    public abstract class ExpressionParser
    {
        private static Type s_booleanType = typeof(bool);

        private static Type s_stringType = typeof(string);

        private static Type s_dateTimeType = typeof(DateTime);

        private static Type s_dbFuncType = typeof(DbFunc);

        private static Type s_dbObjectExtensionType = typeof(DbObjectExtensions);

        protected Expression Visit(Expression node)
        {
            if (node == null)
                return node;

            switch (node.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)node);
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    return VisitCalculate((BinaryExpression)node);
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return VisitLogical((BinaryExpression)node);
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return VisitPredicate((BinaryExpression)node);
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    return VisitBinary((BinaryExpression)node);
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                    return VisitUnary((UnaryExpression)node);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)node);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)node);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)node);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)node);
                case ExpressionType.NewArrayInit:
                    return VisitNewArray((NewArrayExpression)node);
                default:
                    throw new NotImplementedException($"{node.NodeType}");
            }
        }

        private LambdaExpression _lambda = null;

        protected LambdaExpression Lambda => _lambda;

        protected void SetLambdaExpression(LambdaExpression lambda)
        {
            _lambda = lambda;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            _lambda = node;
            var body = Visit(node.Body);
            if (body is ConstantExpression constant)
                AppendSql(GetParameterName(constant.Value));
            else if (body is MemberExpression member)
            {
                if (node.Type == s_booleanType)
                    AppendPredicate(member, Expression.Constant(true), ExpressionType.Equal);
                else
                    AppendSql(GetPropertyName(member));
            }
            else if (body is SqlExpression sql)
                AppendSql(sql.Sql);
            _lambda = null;
            return node;
        }

        protected virtual Expression VisitCalculate(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            if (left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.Constant)
            {
                dynamic number1 = GetConstantValue(left);
                dynamic number2 = GetConstantValue(right);
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                        return Expression.Constant(number1 + number2);
                    case ExpressionType.Subtract:
                        return Expression.Constant(number1 - number2);
                    case ExpressionType.Multiply:
                        return Expression.Constant(number1 * number2);
                    case ExpressionType.Divide:
                        return Expression.Constant(number1 / number2);
                }

                throw new NotSupportedException($"{node.NodeType} not supported");
            }

            return AppendCalculate(left, right, node.NodeType);
        }

        protected virtual Expression VisitPredicate(BinaryExpression node)
        {
            var left = Visit(HandleNull(node.Left));
            var right = Visit(HandleNull(node.Right));

            if (left == null)
            {
                var temp = left;
                left = right;
                right = temp;
            }

            AppendPredicate(left, right, node.NodeType);

            Expression HandleNull(Expression exp)
            {
                if (exp is ConstantExpression constant && constant.Value == null)
                    return null;

                return exp;
            }
            return node;
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);
            if (left is ConstantExpression leftConstant && right is ConstantExpression rightConstant)
            {
                if (node.NodeType == ExpressionType.Coalesce)
                    return Expression.Constant(leftConstant.Value ?? rightConstant.Value);
                else if (node.NodeType == ExpressionType.ArrayIndex)
                {
                    dynamic array = leftConstant.Value;
                    dynamic index = rightConstant.Value;
                    return Expression.Constant(array[index]);
                }
            }

            throw new NotSupportedException($"{node.NodeType} not supported");
        }

        protected virtual Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        protected virtual Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            var exp = Visit(node.Expression);
            if (exp == null || exp.NodeType == ExpressionType.Constant)
            {
                object val = null;
                if (exp != null)
                    val = ((ConstantExpression)exp).Value;

                if (node.Member is PropertyInfo propertyInfo)
                    return Expression.Constant(propertyInfo.GetValue(val), propertyInfo.PropertyType);

                if (node.Member is FieldInfo fieldInfo)
                    return Expression.Constant(fieldInfo.GetValue(val), fieldInfo.FieldType);
            }
            else if (Nullable.GetUnderlyingType(node.Member.DeclaringType) != null)
                return exp;

            if (node.Member.DeclaringType == s_dateTimeType)
                return AppendDateTimeMember(exp, node.Member);

            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression obj = Visit(node.Object);
            var args = node.Arguments.Select(o => Visit(o));
            if (node.Object != null)
            {
                if (obj == null)
                    throw new NullReferenceException();

                if (obj is ConstantExpression constant)
                    return Expression.Constant(node.Method.Invoke(constant.Value, GetConstantValue(args)));
                else if (obj.NodeType == ExpressionType.MemberAccess)
                {
                    var result = AppendMethod(obj, node.Method, args.ToArray());
                    if (result != null)
                        return result;
                }
                else
                    throw new NotSupportedException($"{obj} {node.Method.Name}");
            }
            else if (s_dbObjectExtensionType == node.Method.ReflectedType || s_dbFuncType == node.Method.ReflectedType)
            {
                var result = AppendMethod(null, node.Method, args.ToArray());
                if (result != null)
                    return result;
            }
            else
            {
                if (node.Method.Name.Equals("Format", StringComparison.Ordinal) && args.Any(o => o.NodeType == ExpressionType.MemberAccess))
                {
                    var vals = args.Select((o, i) =>
                    {
                        if (i > 0)
                        {
                            if (o is MemberExpression parameter)
                                return GetPropertyName(parameter);
                            else if (o is ConstantExpression constant)
                                return GetParameterName(constant.Value);
                            else
                                throw new NotSupportedException($"{o}");
                        }

                        return GetConstantValue<string>(o);
                    });

                    return new ExprExpression(string.Format(vals.First(), vals.Skip(1).ToArray()));
                }

                return Expression.Constant(node.Method.Invoke(null, GetConstantValue(args)));
            }

            return node;
        }

        protected virtual Expression VisitLogical(BinaryExpression node)
        {
            var left = Visit(node.Left);
            AppendLogicalOperator(node.NodeType);
            var right = Visit(node.Right);

            Handle(left);
            Handle(right);

            void Handle(Expression exp)
            {
                if (exp.NodeType == ExpressionType.MemberAccess)
                    AppendPredicate(exp, Expression.Constant(true, node.Type), ExpressionType.Equal);
                else if (exp is SqlExpression sql)
                    AppendSql(sql.Sql);
                else if (exp.NodeType == ExpressionType.Constant)
                    throw new NotSupportedException($"{exp} constant should't be logical");
            }

            return node;
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
                return Expression.Constant(node.Operand);

            Expression operand = Visit(node.Operand);
            if (node.NodeType == ExpressionType.Convert)
                return operand;
            else if (node.NodeType == ExpressionType.Not)
            {
                if (operand is MemberExpression member)
                    AppendPredicate(member, Expression.Constant(false, node.Type), ExpressionType.Equal);
                else if (operand is SqlExpression sql)
                    return AppendNot(sql);
            }
            else if (node.NodeType == ExpressionType.ArrayLength && operand is ConstantExpression constant)
            {
                if (constant.Value == null)
                    throw new NullReferenceException();

                var array = (Array)constant.Value;
                return Expression.Constant(array.Length);
            }

            return node;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            var args = VisitConstantExpressions(node.Expressions);
            return Expression.Constant(args);
        }

        protected virtual object GetConstantValue(Expression exp)
        {
            if (exp is ConstantExpression constant)
                return constant.Value;

            throw new NotSupportedException($"{exp}");
        }

        protected virtual T GetConstantValue<T>(Expression exp)
        {
            return (T)GetConstantValue(exp);
        }

        protected virtual object[] GetConstantValue(IEnumerable<Expression> exps)
        {
            return exps.Select(o => GetConstantValue(o)).ToArray();
        }

        protected virtual object[] VisitConstantExpressions(IEnumerable<Expression> exps)
        {
            var args = exps.Select(o =>
            {
                var exp = Visit(o);
                if (exp is ConstantExpression constant)
                    return constant.Value;

                throw new NotSupportedException(o.ToString());
            }).ToArray();

            return args;
        }

        protected virtual string GetPropertyName(Expression exp)
        {
            return exp.ToString();
        }

        protected virtual string GetParameterName(object value)
        {
            return value.ToString();
        }

        protected virtual void AppendLogicalOperator(ExpressionType type)
        {
        }

        protected virtual void AppendPredicate(Expression left, Expression right, ExpressionType type)
        {
        }

        protected virtual Expression AppendCalculate(Expression left, Expression right, ExpressionType type)
        {
            return null;
        }

        protected virtual Expression AppendMethod(Expression instance, MethodInfo method, Expression[] args)
        {
            return null;
        }

        protected virtual Expression AppendDateTimeMember(Expression instance, MemberInfo member)
        {
            return null;
        }

        protected virtual void AppendSql(string sql)
        {
        }

        protected virtual Expression AppendNot(SqlExpression sql)
        {
            return sql;
        }

        protected SqlExpression CreateSql(string sql)
        {
            return new SqlExpression(sql);
        }
    }
}
