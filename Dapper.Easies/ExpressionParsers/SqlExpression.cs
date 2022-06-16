using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
	public class SqlExpression : Expression
	{
		public SqlExpression(string sql)
		{
			Sql = sql;
		}

		public override ExpressionType NodeType { get; } = (ExpressionType)999;

		public string Sql { get; }

		public override string ToString()
		{
			return Sql;
		}
	}
}
