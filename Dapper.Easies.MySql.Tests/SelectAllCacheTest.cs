using Dapper.Easies;
using Dapper.Easies.Tests;
using Xunit;

namespace Dapper.Easies.MySql.Tests
{
    [Collection("MySql")]
    public class SelectAllCacheTest : BaseTest
    {
        public SelectAllCacheTest(MySqlDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        // schema-only 全表查询的 SQL 仅依赖表结构，应与逐字段生成的结果完全一致。
        [Fact]
        public void SelectAll_GeneratesExpectedSql()
        {
            var (sql, _) = EasiesProvider.From<Student>().GetSql();
            Assert.Equal(
                "SELECT t.`Id`, t.`ClassId`, t.`StudentName` AS `Name`, t.`Age`, t.`CreateTime` FROM `tb_students` t",
                sql);
        }

        // 第二次生成应命中缓存，返回同一字符串实例（ReferenceEqual），证明跳过了重复生成。
        [Fact]
        public void SelectAll_SecondCallReturnsCachedInstance()
        {
            var (sql1, _) = EasiesProvider.From<Student>().GetSql();
            var (sql2, _) = EasiesProvider.From<Student>().GetSql();
            Assert.True(ReferenceEquals(sql1, sql2));
        }

        // 带 where 的查询不应命中全表缓存，且每次生成独立 SQL。
        [Fact]
        public void SelectAll_WithWhereDoesNotUseCache()
        {
            var id = 1;
            var (sql, _) = EasiesProvider.From<Student>().Where(o => o.Id == id).GetSql();
            Assert.Equal(
                "SELECT t.`Id`, t.`ClassId`, t.`StudentName` AS `Name`, t.`Age`, t.`CreateTime` FROM `tb_students` t WHERE t.`Id` = @p0",
                sql);
        }

        // 分页查询（take>0）不应命中全表缓存。
        [Fact]
        public void SelectAll_WithTakeDoesNotUseCache()
        {
            var (sql, _) = EasiesProvider.From<Student>().GetSql(0, 10);
            Assert.EndsWith(" LIMIT 0,10", sql);
        }
    }
}
