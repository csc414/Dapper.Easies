using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Dapper.Easies;
using static Dapper.SqlMapper;
using System.Linq.Expressions;

namespace Dapper.Easies.Tests
{
    public abstract class DbQueryTest : BaseTest
    {
        protected DbQueryTest(IDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        [Fact]
        public void FirstOrDefault()
        {
            var result = EasiesProvider.From<Student>()
                .GetSql(1);
            FirstOrDefaultTest(result.sql);
        }

        public abstract void FirstOrDefaultTest(string sql);

        [Fact]
        public void JoinTable()
        {
            (string sql, DynamicParameters parameters) result;
            result = EasiesProvider.From<Student>()
                .Join<Class>()
                .GetSql();
            JoinTableTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .GetSql();
            JoinTableOnTest(result.sql);

            var stuId = 1;
            result = EasiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id && a.Id == stuId)
                .GetSql();
            JoinTableOnWithParameterTest(result.sql, result.parameters);
        }

        public abstract void JoinTableTest(string sql);

        public abstract void JoinTableOnTest(string sql);

        public abstract void JoinTableOnWithParameterTest(string sql, IParameterLookup parameters);

        [Fact]
        public void JoinQuery()
        {
            var query = EasiesProvider.From<Class>()
                .Select(o => new { o.Id, o.Name });

            (string sql, DynamicParameters parameters) result;
            result = EasiesProvider.From<Student>()
                .Join(query)
                .GetSql();
            JoinQueryTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join(query, (a, b) => a.ClassId == b.Id)
                .GetSql();
            JoinQueryOnTest(result.sql);

            var stuId = 1;
            result = EasiesProvider.From<Student>()
                .Join(query, (a, b) => a.ClassId == b.Id && a.Id == stuId).GetSql();
            JoinQueryOnWithParameterTest(result.sql, result.parameters);
        }

        public abstract void JoinQueryTest(string sql);

        public abstract void JoinQueryOnTest(string sql);

        public abstract void JoinQueryOnWithParameterTest(string sql, IParameterLookup parameters);

        [Fact]
        public void Select()
        {
            (string sql, DynamicParameters parameters) result;

            var query = EasiesProvider.From<Class>()
                .Select(o => new { o.Id, o.Name });

            result = query.GetSql();
            SelectTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join<Class>()
                .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name })
                .GetSql();
            SelectJoinTableTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join(query)
                .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name })
                .GetSql();
            SelectJoinQueryTest(result.sql);
        }

        public abstract void SelectTest(string sql);

        public abstract void SelectJoinTableTest(string sql);

        public abstract void SelectJoinQueryTest(string sql);

        [Fact]
        public void Distinct()
        {
            (string sql, DynamicParameters parameters) result;

            result = EasiesProvider.From<Student>()
                .Distinct()
                .GetSql();
            DistinctTest(result.sql);
        }

        public abstract void DistinctTest(string sql);

        [Fact]
        public void SkipTake()
        {
            (string sql, DynamicParameters parameters) result;

            result = EasiesProvider.From<Student>()
                .Skip(5)
                .Take(10)
                .GetSql();
            SkipTakeTest(result.sql);
        }

        public abstract void SkipTakeTest(string sql);

        [Fact]
        public void OrderBy()
        {
            (string sql, DynamicParameters parameters) result;

            var query = EasiesProvider.From<Class>()
                .Select(o => new { o.Id, o.Name });

            result = EasiesProvider.From<Student>()
                .OrderBy(o => o.Id, o => o.Name)
                .ThenByDescending(o => o.Id, o => o.Name)
                .GetSql();
            OrderByTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join<Class>()
                .OrderBy((a, b) => a.Id, (a, b) => b.Name)
                .ThenByDescending((a, b) => a.Id, (a, b) => b.Name)
                .GetSql();
            OrderByJoinTableTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join(query)
                .OrderBy((a, b) => a.Id, (a, b) => b.Name)
                .ThenByDescending((a, b) => a.Id, (a, b) => b.Name)
                .GetSql();
            OrderByJoinQueryTest(result.sql);
        }

        public abstract void OrderByTest(string sql);

        public abstract void OrderByJoinTableTest(string sql);

        public abstract void OrderByJoinQueryTest(string sql);

        [Fact]
        public void GroupBy()
        {
            (string sql, DynamicParameters parameters) result;

            var query = EasiesProvider.From<Class>()
                .Select(o => new { o.Id, o.Name });

            result = EasiesProvider.From<Student>()
                .GroupBy(o => o.Id)
                .Select(o => o.Id)
                .GetSql();
            GroupByTest(result.sql);

            result = EasiesProvider.From<Student>()
                .GroupBy(o => new { o.Id, o.Name })
                .Select(o => new { o.Id, o.Name })
                .GetSql();
            GroupByMultipleTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join<Class>()
                .GroupBy((a, b) => new { a.Id, b.Name })
                .Select((a, b) => new { a.Id, b.Name })
                .GetSql();
            GroupByJoinTableTest(result.sql);

            result = EasiesProvider.From<Student>()
                .Join(query)
                .GroupBy((a, b) => new { a.Id, b.Name })
                .Select((a, b) => new { a.Id, b.Name })
                .GetSql();
            GroupByJoinQueryTest(result.sql);

            result = EasiesProvider.From<Student>()
                .GroupBy(o => o.Id)
                .Having(o => DbFunc.Count() > 0)
                .Select(o => new { o.Id, Count = DbFunc.Count() })
                .GetSql();
            GroupByHavingTest(result.sql, result.parameters);
        }

        public abstract void GroupByTest(string sql);

        public abstract void GroupByMultipleTest(string sql);

        public abstract void GroupByJoinTableTest(string sql);

        public abstract void GroupByJoinQueryTest(string sql);

        public abstract void GroupByHavingTest(string sql, IParameterLookup parameters);

        [Fact]
        public void Aggregate()
        {
            (string sql, DynamicParameters parameters) result;

            Expression<Func<Student, object>> field = o => o.Id;

            var query = EasiesProvider.From<Student>();

            //query.CountAsync();
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Count, null));
            AggregateCountTest(result.sql);

            //query.CountAsync(o => o.Id);
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Count, field));
            AggregateCountFieldTest(result.sql);

            //query.AvgAsync(o => o.Id);
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Avg, field));
            AggregateAvgTest(result.sql);

            //query.SumAsync(o => o.Id);
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Sum, field));
            AggregateSumTest(result.sql);

            //query.MaxAsync(o => o.Id);
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Max, field));
            AggregateMaxTest(result.sql);

            //query.MinAsync(o => o.Id);
            result = query.GetSql(aggregateInfo: new AggregateInfo(AggregateType.Min, field));
            AggregateMinTest(result.sql);

            var entity = EasiesProvider.Entity<Student>();

            result = entity
                .Select(o => DbFunc.Count())
                .GetSql();
            AggregateCountTest(result.sql);

            result = entity
                .Select(o => DbFunc.Count(o.Id))
                .GetSql();
            AggregateCountFieldTest(result.sql);

            result = entity
                .Select(o => DbFunc.Avg(o.Id))
                .GetSql();
            AggregateAvgTest(result.sql);

            result = entity
                .Select(o => DbFunc.Sum(o.Id))
                .GetSql();
            AggregateSumTest(result.sql);

            result = entity
                .Select(o => DbFunc.Max(o.Id))
                .GetSql();
            AggregateMaxTest(result.sql);

            result = entity
                .Select(o => DbFunc.Min(o.Id))
                .GetSql();
            AggregateMinTest(result.sql);
        }

        public abstract void AggregateCountTest(string sql);

        public abstract void AggregateCountFieldTest(string sql);

        public abstract void AggregateAvgTest(string sql);

        public abstract void AggregateSumTest(string sql);

        public abstract void AggregateMaxTest(string sql);

        public abstract void AggregateMinTest(string sql);

        [Fact]
        public void Where()
        {
            (string sql, DynamicParameters parameters) result;

            var name = "张三";
            var ids = new int[] { 1, 2, 3 };

            var student = EasiesProvider.Entity<Student>();

            result = student
                .Where(o => DbFunc.Like(o.Name, $"%{name}%"))
                .GetSql();
            WhereLikeTest(result.sql, result.parameters);

            result = student
                .Where(o => DbFunc.In(o.Id, ids))
                .GetSql();
            WhereInTest(result.sql, result.parameters);

            result = student
                .Where(o => DbFunc.NotIn(o.Id, ids))
                .GetSql();
            WhereNotInTest(result.sql, result.parameters);

            result = student
                .Where(o => o.Age == o.Age * 2 * 0.25 + o.Age / 2 && o.Name == name)
                .GetSql();
            WhereComplicatedTest(result.sql, result.parameters);
        }

        public abstract void WhereLikeTest(string sql, IParameterLookup parameters);

        public abstract void WhereInTest(string sql, IParameterLookup parameters);

        public abstract void WhereNotInTest(string sql, IParameterLookup parameters);

        public abstract void WhereComplicatedTest(string sql, IParameterLookup parameters);

        [Fact]
        public void Expr()
        {
            (string sql, DynamicParameters parameters) result;

            var name = "张三";
            var age = 18;

            var student = EasiesProvider.Entity<Student>();
            
            result = student
                .Join<Class>((a, b) => $"{a.ClassId} = {b.Id}")
                .GetSql();
            ExprJoinTest(result.sql);

            result = student
                .Where(o => $"{o.Name} LIKE {$"%{name}%"} AND {o.Age} = {age}")
                .GetSql();
            ExprWhereTest(result.sql, result.parameters);

            result = student
                .Where(o => DbFunc.Like(o.Name, $"%{name}%") && DbFunc.Expr<bool>($"{o.Age} = {age}"))
                .GetSql();
            ExprWhereTest(result.sql, result.parameters);

            result = student
                .Select(o => new { Name = DbFunc.Expr<string>($"({o.Name} + {name})") })
                .GetSql();
            ExprSelectTest(result.sql, result.parameters);

            //DbFunc.Expr 可嵌套在任何Lambda表达式内执行，相当于嵌入原生Sql
        }

        public abstract void ExprJoinTest(string sql);

        public abstract void ExprWhereTest(string sql, IParameterLookup parameters);

        public abstract void ExprSelectTest(string sql, IParameterLookup parameters);

        [Fact]
        public void SubQuery()
        {
            (string sql, DynamicParameters parameters) result;

            result = EasiesProvider.From<Student>()
                  .Where(s => DbFunc.In(
                      s.ClassId,
                      EasiesProvider.From<Class>()
                      .Select(c => c.Id)
                      .SubQuery()
                  ))
                  .GetSql();
            SubQueryTest(result.sql);

            result = EasiesProvider.From<Class>()
                .Select(c => new 
                {
                    ClassName = c.Name,
                    StudentCount = EasiesProvider.From<Student>()
                                    .Where(s => s.ClassId == c.Id)
                                    .Select(s => DbFunc.Count())
                                    .SubQueryScalar()
                })
                .GetSql();
            SubQueryScalarTest(result.sql);
        }

        public abstract void SubQueryTest(string sql);

        public abstract void SubQueryScalarTest(string sql);
    }
}
