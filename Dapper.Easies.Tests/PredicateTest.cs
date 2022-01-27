using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;
using static Dapper.SqlMapper;

namespace Dapper.Easies.Tests
{
    
    public class PredicateTest
    {
        private readonly ISqlSyntax _sqlSyntax;

        public PredicateTest()
        {
            _sqlSyntax = new DefaultSqlSyntax();
            DbObject.Initialize(_sqlSyntax);
        }

        [Fact]
        public void ArgumentTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            Expression<Predicate<Student>> exp = o => o.Id == ((o.Id * 3) + (o.Id / 5)) * 2 && (o.Age > 18 || o.StudentName == "张三") && o.IsHandsome;
            string sql = parser.ToSql(exp, context);
            Assert.Equal("t.Id = ((t.Id * @p0) + (t.Id / @p1)) * @p2 AND (t.Age > @p3 OR t.Name = @p4) AND t.IsHandsome", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal(3, (int)lookup["p0"]);
            Assert.Equal(5, (int)lookup["p1"]);
            Assert.Equal(2, (int)lookup["p2"]);
            Assert.Equal(18, (int)lookup["p3"]);
            Assert.Equal("张三", (string)lookup["p4"]);
        }

        [Fact]
        public void NotTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            Expression<Predicate<Student>> exp = o =>  !(o.Id == 1) && !o.IsHandsome;
            string sql = parser.ToSql(exp, context);
            Assert.Equal("NOT (t.Id = @p0) AND NOT (t.IsHandsome)", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal(1, (int)lookup["p0"]);
        }

        [Fact]
        public void JoinTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            Expression<Func<Student, Class, bool>> on = (a, b) => a.ClassId == b.Id;
            context.AddJoin(typeof(Class), on, JoinType.Left);
            string onSql = parser.ToSql(on, context);
            Assert.Equal("t.ClassId = t1.Id", onSql);

            Expression<Func<Student, Class, bool>> exp = (a, b) => a.StudentName == "张三" && b.Name == "一年级";
            string sql = parser.ToSql(exp, context);
            Assert.Equal("t.Name = @p0 AND t1.Name = @p1", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal("张三", (string)lookup["p0"]);
            Assert.Equal("一年级", (string)lookup["p1"]);
        }

        [Fact]
        public void InAndNotInTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            var ary = new[] { "张三", "李四" };
            Expression<Predicate<Student>> exp = (a) => !DbFunc.In(a.StudentName, ary) && DbFunc.NotIn(a.StudentName, ary);
            string sql = parser.ToSql(exp, context);
            Assert.Equal("NOT (t.Name IN @p0) AND t.Name NOT IN @p1", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal(ary, (string[])lookup["p0"]);
            Assert.Equal(ary, (string[])lookup["p1"]);
        }

        [Fact]
        public void LikeTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            var name = "张三";
            Expression<Predicate<Student>> exp = (a) => DbFunc.Like(a.StudentName, $"%{name}%");
            string sql = parser.ToSql(exp, context);
            Assert.Equal("t.Name LIKE @p0", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal($"%{name}%", (string)lookup["p0"]);
        }

        [Fact]
        public void ExpressionTest()
        {
            var builder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, builder);
            var context = new QueryContext(null, null, DbObject.Get(typeof(Student)));
            var name = "%张三%";
            Expression<Predicate<Student>> exp = (a) => DbFunc.Expr<bool>($"{a.StudentName} LIKE {name}");
            string sql = parser.ToSql(exp, context);
            Assert.Equal("t.Name LIKE @p0", sql);
            IParameterLookup lookup = builder.GetDynamicParameters();
            Assert.Equal(name, (string)lookup["p0"]);
        }
    }
}
