using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class QueryContext
    {
        private readonly IConnection _connection;

        public QueryContext(IConnection connection, ISqlConverter sqlConverter, DbObject dbObject)
        {
            _connection = connection;
            Converter = sqlConverter;
            DbObject = dbObject;
            Alias = new Dictionary<Type, DbAlias> { { dbObject.Type, new DbAlias(dbObject.EscapeName, "t") } };
        }

        private ICollection<JoinMetedata> _joinMetedatas;

        private ICollection<Expression> _whereExpressions;

        private ICollection<Expression> _orderExpressions;

        public DbObject DbObject { get; }

        public IDictionary<Type, DbAlias> Alias { get; }

        public ISqlConverter Converter { get; }

        public IDbConnection Connection => _connection.Connection;

        public ICollection<JoinMetedata> JoinMetedatas => _joinMetedatas ?? (_joinMetedatas = new List<JoinMetedata>());

        public ICollection<Expression> WhereExpressions => _whereExpressions ?? (_whereExpressions = new List<Expression>());

        public ICollection<Expression> OrderExpressions => _orderExpressions ?? (_orderExpressions = new List<Expression>());

        public OrderByMetedata OrderByMetedata { get; internal set; }

        public OrderByMetedata ThenByMetedata { get; internal set; }

        public Expression SelectorExpression { get; internal set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
