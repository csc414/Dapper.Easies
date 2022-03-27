using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class QueryContext : ICloneable
    {
        private readonly IDbConnectionCache _connection;

        public QueryContext(IDbConnectionCache connection, ISqlConverter sqlConverter, DbObject dbObject) : this(connection, sqlConverter, dbObject, new[] { new KeyValuePair<Type, DbAlias>(dbObject.Type, new DbAlias(dbObject.EscapeName, "t")) })
        {
        }

        public QueryContext(IDbConnectionCache connection, ISqlConverter sqlConverter, DbObject dbObject, IEnumerable<KeyValuePair<Type, DbAlias>> alias)
        {
            _connection = connection;
            Converter = sqlConverter;
            DbObject = dbObject;
            Alias = new Dictionary<Type, DbAlias>(alias);
        }

        private ICollection<JoinMetedata> _joinMetedatas;

        private ICollection<Expression> _whereExpressions;

        private ICollection<Expression> _havingExpressions;

        public DbObject DbObject { get; }

        public IDictionary<Type, DbAlias> Alias { get; }

        public ISqlConverter Converter { get; }

        public IDbConnection Connection => _connection.GetConnection(DbObject.ConnectionFactory);

        public IEnumerable<JoinMetedata> JoinMetedatas => _joinMetedatas;

        public IEnumerable<Expression> WhereExpressions => _whereExpressions;

        public IEnumerable<Expression> HavingExpressions => _havingExpressions;

        public OrderByMetedata OrderByMetedata { get; internal set; }

        public OrderByMetedata ThenByMetedata { get; internal set; }

        public Expression SelectorExpression { get; internal set; }

        public Expression GroupByExpression { get; internal set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool Distinct { get; set; }

        public void AddJoin(Type joinType, Expression joinExpression, JoinType type)
        {
            var dbObject = DbObject.Get(joinType);
            if (!Alias.TryAdd(dbObject.Type, new DbAlias(dbObject.EscapeName, $"t{Alias.Count}")))
                throw new ArgumentException($"请勿重复连接表 {dbObject.Type.Name}.");

            if (DbObject.ConnectionStringName != dbObject.ConnectionStringName)
                throw new ArgumentException($"无法连接来自不同配置的表");

            if (_joinMetedatas == null)
                _joinMetedatas = new List<JoinMetedata>();

            _joinMetedatas.Add(new JoinMetedata { DbObject = dbObject, JoinExpression = joinExpression, Type = type });
        }

        public void AddWhere(Expression whereExpression)
        {
            if (_whereExpressions == null)
                _whereExpressions = new List<Expression>();

            _whereExpressions.Add(whereExpression);
        }

        public void AddHaving(Expression havingExpression)
        {
            if (_havingExpressions == null)
                _havingExpressions = new List<Expression>();

            _havingExpressions.Add(havingExpression);
        }

        public object Clone()
        {
            var context = new QueryContext(_connection, Converter, DbObject, Alias);
            if (_havingExpressions != null)
                context._havingExpressions = new List<Expression>(_havingExpressions);
            if (_joinMetedatas != null)
                context._joinMetedatas = new List<JoinMetedata>(_joinMetedatas);
            if (_whereExpressions != null)
                context._whereExpressions = new List<Expression>(_whereExpressions);
            context.OrderByMetedata = OrderByMetedata;
            context.ThenByMetedata = ThenByMetedata;
            context.SelectorExpression = SelectorExpression;
            context.GroupByExpression = GroupByExpression;
            context.Skip = Skip;
            context.Take = Take;
            return context;
        }
    }
}
