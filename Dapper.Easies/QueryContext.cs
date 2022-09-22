using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Dapper.Easies
{
    public class QueryContext : ICloneable
    {
        public QueryContext(ISqlConverter sqlConverter, DbObject dbObject) : this(sqlConverter, dbObject, new[] { new DbAlias(dbObject.EscapeName, "t") })
        {
        }

        public QueryContext(ISqlConverter sqlConverter, DbObject dbObject, IEnumerable<DbAlias> alias)
        {
            Converter = sqlConverter;
            DbObject = dbObject;
            Alias = new List<DbAlias>(alias);
        }

        private List<JoinMetedata> _joinMetedatas;

        private List<Expression> _whereExpressions;

        private List<Expression> _havingExpressions;

        private List<Expression> _combineWhereExpressions;

        private bool _initAppender = false;

        public DbObject DbObject { get; }

        public List<DbAlias> Alias { get; }

        public ISqlConverter Converter { get; }

        public IReadOnlyCollection<JoinMetedata> JoinMetedatas => _joinMetedatas;

        public IReadOnlyCollection<Expression> WhereExpressions
        {
            get
            {
                if (!_initAppender)
                {
                    if (!NoAppender && DbObject.Appender != null)
                    {
                        _combineWhereExpressions = new List<Expression>(DbObject.Appender());
                        if(_whereExpressions != null)
                            _combineWhereExpressions.AddRange(_whereExpressions);
                    }
                    else
                        _combineWhereExpressions = _whereExpressions;

                    _initAppender = true;
                }

                return _combineWhereExpressions;
            }
        }

        public IReadOnlyCollection<Expression> HavingExpressions => _havingExpressions;

        public OrderByMetedata OrderByMetedata { get; internal set; }

        public OrderByMetedata ThenByMetedata { get; internal set; }

        public Expression SelectorExpression { get; internal set; }

        public Expression GroupByExpression { get; internal set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool Distinct { get; set; }

        public bool NoAppender { get; set; }

        public void AddJoin(Type joinType, Expression joinExpression, JoinType type, IDbQuery query = null)
        {
            var dbObject = DbObject.Get(joinType);

            if (dbObject != null)
            {
                if (DbObject.ConnectionStringName != dbObject.ConnectionStringName)
                    throw new ArgumentException($"无法连接来自不同配置的表");
            }

            if (query != null)
            {
                if (DbObject.ConnectionStringName != query.Context.DbObject.ConnectionStringName)
                    throw new ArgumentException($"无法连接来自不同配置的表");
            }

            if (_joinMetedatas == null)
                _joinMetedatas = new List<JoinMetedata>();

            var alias = new DbAlias(dbObject?.EscapeName, $"t{Alias.Count}");
            Alias.Add(alias);
            _joinMetedatas.Add(new JoinMetedata(dbObject, type, joinExpression, query));
        }

        public void AddWhere(Expression whereExpression)
        {
            if (_whereExpressions == null)
                _whereExpressions = new List<Expression>();

            _whereExpressions.Add(whereExpression);
            _initAppender = false;
        }

        public void SetWhere(Index i, LambdaExpression whereExpression)
        {
            if (whereExpression.ReturnType == typeof(string))
                _whereExpressions[i] = DbQuery.CreateExpressionLambda(whereExpression);
            else
                _whereExpressions[i] = whereExpression;
            _initAppender = false;
        }

        public void AddHaving(Expression havingExpression)
        {
            if (_havingExpressions == null)
                _havingExpressions = new List<Expression>();

            _havingExpressions.Add(havingExpression);
        }

        public object Clone()
        {
            var context = new QueryContext(Converter, DbObject, Alias);
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
            context.Distinct = Distinct;
            context.NoAppender = NoAppender;
            return context;
        }
    }
}
