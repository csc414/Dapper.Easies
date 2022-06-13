using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
	public class ExprExpression : Expression
	{
		public ExprExpression(string sql)
		{
			Sql = sql;
		}

		public override ExpressionType NodeType { get; } = (ExpressionType)998;

		public string Sql { get; }

		public override string ToString()
		{
			return Sql;
		}
	}
}
