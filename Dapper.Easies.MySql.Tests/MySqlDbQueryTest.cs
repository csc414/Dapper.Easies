using Dapper.Easies.Tests;
using System;
using System.Collections.Generic;
using Xunit;
using static Dapper.SqlMapper;

namespace Dapper.Easies.MySql.Tests
{
    [Collection("MySql")]
    public class MySqlDbQueryTest : DbQueryTest
    {
        public MySqlDbQueryTest(MySqlDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        public override void FirstOrDefaultTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t LIMIT 0,1", sql);
        }

        public override void JoinTableTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN `tb_classes` t1", sql);
        }

        public override void JoinTableOnTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN `tb_classes` t1 ON t.`ClassId` = t1.`Id`", sql);
        }

        public override void JoinTableOnWithParameterTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN `tb_classes` t1 ON t.`ClassId` = t1.`Id` AND t.`Id` = @p0", sql);
            Assert.Equal(1, parameters["p0"]);
        }

        public override void JoinQueryTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1", sql);
        }

        public override void JoinQueryOnTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1 ON t.`ClassId` = t1.`Id`", sql);
        }

        public override void JoinQueryOnWithParameterTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1 ON t.`ClassId` = t1.`Id` AND t.`Id` = @p0", sql);
            Assert.Equal(1, parameters["p0"]);
        }

        public override void SelectTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t", sql);
        }

        public override void SelectJoinTableTest(string sql)
        {
            Assert.Equal("SELECT t.`StudentName` AS `StudentName`, t1.`ClassName` AS `ClassName` FROM `tb_students` t JOIN `tb_classes` t1", sql);
        }

        public override void SelectJoinQueryTest(string sql)
        {
            Assert.Equal("SELECT t.`StudentName` AS `StudentName`, t1.`Name` AS `ClassName` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1", sql);
        }

        public override void DistinctTest(string sql)
        {
            Assert.Equal("SELECT DISTINCT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t", sql);
        }

        public override void SkipTakeTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t LIMIT 5,10", sql);
        }

        public override void OrderByTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t ORDER BY t.`Id`, t.`StudentName` Asc, t.`Id`, t.`StudentName` Desc", sql);
        }

        public override void OrderByJoinTableTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN `tb_classes` t1 ORDER BY t.`Id`, t1.`ClassName` Asc, t.`Id`, t1.`ClassName` Desc", sql);
        }

        public override void OrderByJoinQueryTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1 ORDER BY t.`Id`, t1.`Name` Asc, t.`Id`, t1.`Name` Desc", sql);
        }

        public override void GroupByTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` FROM `tb_students` t GROUP BY t.`Id`", sql);
        }

        public override void GroupByMultipleTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`StudentName` AS `Name` FROM `tb_students` t GROUP BY t.`Id`, t.`StudentName`", sql);
        }

        public override void GroupByJoinTableTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t1.`ClassName` AS `Name` FROM `tb_students` t JOIN `tb_classes` t1 GROUP BY t.`Id`, t1.`ClassName`", sql);
        }

        public override void GroupByJoinQueryTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t1.`Name` AS `Name` FROM `tb_students` t JOIN (SELECT t.`Id` AS `Id`, t.`ClassName` AS `Name` FROM `tb_classes` t) t1 GROUP BY t.`Id`, t1.`Name`", sql);
        }

        public override void GroupByHavingTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, COUNT(*) AS `Count` FROM `tb_students` t GROUP BY t.`Id` HAVING COUNT(*) > @p0", sql);
            Assert.Equal(0L, parameters["p0"]);
        }

        public override void AggregateCountTest(string sql)
        {
            Assert.Equal("SELECT COUNT(*) FROM `tb_students` t", sql);
        }

        public override void AggregateCountFieldTest(string sql)
        {
            Assert.Equal("SELECT COUNT(t.`Id`) FROM `tb_students` t", sql);
        }

        public override void AggregateAvgTest(string sql)
        {
            Assert.Equal("SELECT AVG(t.`Id`) FROM `tb_students` t", sql);
        }

        public override void AggregateSumTest(string sql)
        {
            Assert.Equal("SELECT SUM(t.`Id`) FROM `tb_students` t", sql);
        }

        public override void AggregateMaxTest(string sql)
        {
            Assert.Equal("SELECT MAX(t.`Id`) FROM `tb_students` t", sql);
        }

        public override void AggregateMinTest(string sql)
        {
            Assert.Equal("SELECT MIN(t.`Id`) FROM `tb_students` t", sql);
        }

        public override void WhereLikeTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`StudentName` LIKE @p0", sql);
            Assert.Equal("%张三%", parameters["p0"]);
        }

        public override void WhereInTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`Id` IN @p0", sql);
            Assert.IsAssignableFrom<IEnumerable<int>>(parameters["p0"]);
        }

        public override void WhereNotInTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`Id` NOT IN @p0", sql);
            Assert.IsAssignableFrom<IEnumerable<int>>(parameters["p0"]);
        }

        public override void WhereComplicatedTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`Age` = (t.`Age` * @p0 * @p1) + t.`Age` / @p2 AND t.`StudentName` = @p3", sql);
            Assert.Equal(2, parameters["p0"]);
            Assert.Equal(0.25, parameters["p1"]);
            Assert.Equal(2, parameters["p2"]);
            Assert.Equal("张三", parameters["p3"]);
        }

        public override void ExprJoinTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t JOIN `tb_classes` t1 ON t.`ClassId` = t1.`Id`", sql);
        }

        public override void ExprWhereTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`StudentName` LIKE @p0 AND t.`Age` = @p1", sql);
            Assert.Equal("%张三%", parameters["p0"]);
            Assert.Equal(18, parameters["p1"]);
        }

        public override void ExprSelectTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT (t.`StudentName` + @p0) AS `Name` FROM `tb_students` t", sql);
            Assert.Equal("张三", parameters["p0"]);
        }

        public override void SubQueryTest(string sql)
        {
            Assert.Equal("SELECT t.`Id` AS `Id`, t.`ClassId` AS `ClassId`, t.`StudentName` AS `Name`, t.`Age` AS `Age`, t.`CreateTime` AS `CreateTime` FROM `tb_students` t WHERE t.`ClassId` IN (SELECT tt.`Id` FROM `tb_classes` tt)", sql);
        }

        public override void SubQueryScalarTest(string sql)
        {
            Assert.Equal("SELECT t.`ClassName` AS `ClassName`, (SELECT COUNT(*) FROM `tb_students` tt WHERE tt.`ClassId` = t.`Id`) AS `StudentCount` FROM `tb_classes` t", sql);
        }
    }
}
