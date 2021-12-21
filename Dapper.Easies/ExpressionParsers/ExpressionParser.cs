﻿using System;
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
        private int _deep = 0;

        internal ParserData Visit(Expression exp)
		{
            ParserData data;
            _deep++;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    data = VisitLambda((LambdaExpression)exp);
                    break;
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
                    data = VisitBinary((BinaryExpression)exp);
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.Not:
                    data = VisitUnary((UnaryExpression)exp);
                    break;
                case ExpressionType.MemberAccess:
                    data = VisitMemberAccess((MemberExpression)exp);
                    break;
                case ExpressionType.Constant:
                    data = VisitConstant((ConstantExpression)exp);
                    break;
                case ExpressionType.Call:
                    data = VisitMethodCall((MethodCallExpression)exp);
                    break;
                default:
                    throw new NotImplementedException($"{exp.NodeType}");
            }
            _deep--;
            return data;
        }

        internal virtual ParserData VisitLambda(LambdaExpression lambda)
		{
            Visit(lambda.Body);
            return ParserData.Empty;
		}

        internal virtual ParserData VisitBinary(BinaryExpression b)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitUnary(UnaryExpression u)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitMethodCall(MethodCallExpression m)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitMemberAccess(MemberExpression m)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitConstant(ConstantExpression c)
        {
            return ParserData.Empty;
        }

        protected ParserData CreateConstant(object value)
        {
            return CreateConstant(value, null);
        }

        protected ParserData CreateConstant(object value, Expression expression)
        {
            return CreateParserData(ParserDataType.Constant, value, expression);
        }

        protected ParserData CreateParserData(ParserDataType type, object value)
        {
            return CreateParserData(type, value, null);
        }

        protected ParserData CreateParserData(ParserDataType type, object value, Expression expression)
        {
            return new ParserData(type, value, expression) { Deep = _deep };
        }

        protected int GetDeep() => _deep;

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

            if (expression is BinaryExpression binaryExpression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Coalesce:
                        {
                            var value = GetValue(binaryExpression.Left);
                            if (value == null)
                                value = GetValue(binaryExpression.Right);
                            return value;
                        }
                    case ExpressionType.ArrayIndex:
                        {
                            var array = (Array)GetValue(binaryExpression.Left);
                            var index = (long)Convert.ChangeType(GetValue(binaryExpression.Right), typeof(long));
                            return array.GetValue(index);
                        }
                }

            }
            throw new NotImplementedException($"NodeType：{expression.NodeType}");
        }
    }

    public enum ParserDataType
    {
        Empty,
        Property,
        Constant
    }

    public class ParserData
    {
        public static ParserData Empty = new ParserData(ParserDataType.Empty, null);

        public ParserData(ParserDataType type, object value)
        {
            Type = type;
            Value = value;
        }

        public ParserData(ParserDataType type, object value, Expression expression) : this(type, value)
        {
            Expression = expression;
        }

        public int Deep { get; internal set; }

        public Expression Expression { get; }

        public ParserDataType Type { get; }

        public object Value { get; }
    }
}
