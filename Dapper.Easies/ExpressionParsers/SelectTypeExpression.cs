using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
	public class SelectTypeExpression : Expression
	{
		public SelectTypeExpression(Type type)
		{
			SelectType = type;
		}

		public override ExpressionType NodeType { get; } = (ExpressionType)998;

		public Type SelectType { get; }
	}
}
