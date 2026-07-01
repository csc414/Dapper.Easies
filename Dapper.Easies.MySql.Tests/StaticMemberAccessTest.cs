using System;
using System.Linq.Expressions;
using Dapper.Easies;
using Dapper.Easies.Tests;
using Xunit;

namespace Dapper.Easies.MySql.Tests
{
    // 回归测试：表达式树中出现静态成员访问（如 DateTime.Now）时，
    // GetMemberValue 的编译 getter 必须正确处理静态成员（target 传 null），
    // 不能抛 "Static property requires null instance"。
    [Collection("MySql")]
    public class StaticMemberAccessTest : BaseTest
    {
        public StaticMemberAccessTest(MySqlDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        // update 中使用静态属性 DateTime.Now —— 修复前的崩溃场景。
        [Fact]
        public void Update_WithStaticProperty_DoesNotThrow()
        {
            var query = EasiesProvider.From<Student>();
            var sql = query.Context.Converter.ToUpdateFieldsSql(
                (Expression<Func<Student, Student>>)(o => new Student { CreateTime = DateTime.Now }),
                query.Context, out var parameters);

            Assert.Equal("UPDATE `tb_students` t SET t.`CreateTime` = @p0", sql);
            Assert.IsType<DateTime>(parameters.Get<DateTime>("p0"));
        }

        // where 中使用静态属性同样必须正常参数化。
        [Fact]
        public void Where_WithStaticProperty_DoesNotThrow()
        {
            var (sql, parameters) = EasiesProvider.From<Student>()
                .Where(o => o.CreateTime == DateTime.Now)
                .GetSql();

            Assert.Equal(
                "SELECT t.`Id`, t.`ClassId`, t.`StudentName` AS `Name`, t.`Age`, t.`CreateTime` FROM `tb_students` t WHERE t.`CreateTime` = @p0",
                sql);
            Assert.IsType<DateTime>(parameters.Get<DateTime>("p0"));
        }
    }
}
