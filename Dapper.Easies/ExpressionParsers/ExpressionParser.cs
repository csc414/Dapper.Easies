using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
	internal abstract class ExpressionParser
	{
        internal Expression Visit(Expression exp)
		{
			switch (exp.NodeType)
			{
				case ExpressionType.Lambda:
					return VisitLambda((LambdaExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Power:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);
                default:
					throw new NotImplementedException($"{exp.NodeType}");
			}
		}

        internal virtual Expression VisitLambda(LambdaExpression lambda)
		{
            Expression body = Visit(lambda.Body);
            if (body != lambda.Body)
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);

            return lambda;
		}

        internal virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = Visit(b.Left);
            Expression right = Visit(b.Right);
            Expression conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }

            return b;
        }

        internal virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = Visit(m.Object);
            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        internal virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
                return new ReadOnlyCollection<Expression>(list);
            return original;
        }

        internal virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = Visit(m.Expression);
            if (exp != m.Expression)
                return Expression.MakeMemberAccess(exp, m.Member);

            return m;
        }

        internal virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        internal static object GetValue(Expression expression)
        {
            if (expression == null)
                return null;

            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value;

            if(expression is MemberExpression memberExpression)
            {
                var obj = GetValue(memberExpression.Expression);
                if (memberExpression.Member is PropertyInfo propertyInfo)
                    return propertyInfo.GetValue(obj);

                if (memberExpression.Member is FieldInfo fieldInfo)
                    return fieldInfo.GetValue(obj);
            }

            if(expression is MethodCallExpression methodCallExpression)
            {
                var args = methodCallExpression.Arguments.Select(o => GetValue(o)).ToArray();
                object obj = null;
                if (methodCallExpression.Object != null)
                    obj = GetValue(methodCallExpression.Object);
                return methodCallExpression.Method.Invoke(obj, args);
            }

            if(expression is NewArrayExpression newArrayExpression)
            {
                var args = newArrayExpression.Expressions.Select(o => GetValue(o)).ToArray();
                var ary = (object[])Activator.CreateInstance(newArrayExpression.Type, args.Length);
                for (int i = 0; i < ary.Length; ++i)
                    ary[i] = args[i];
                return ary;
            }
            
            throw new NotImplementedException($"NodeType：{expression.NodeType}");
        }
    }
}
