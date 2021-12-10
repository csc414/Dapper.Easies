using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    internal class QueryContext
    {
        private readonly IConnection _connection;

        public QueryContext(Type dbTable, IConnection connection)
        {
            DbTable = dbTable;
            _connection = connection;
        }

        private ICollection<JoinMetedata> _joinMetedatas;

        private ICollection<Expression> _whereExpressions;

        public Type DbTable { get; }

        public IDbConnection Connection => _connection.Connection;

        public ICollection<JoinMetedata> JoinMetedatas => _joinMetedatas ?? (_joinMetedatas = new List<JoinMetedata>());

        public ICollection<Expression> WhereExpressions => _whereExpressions ?? (_whereExpressions = new List<Expression>());
    }
}
