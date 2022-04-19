using Dapper.Easies.Tests;
using System;
using Xunit;
using static Dapper.SqlMapper;

namespace Dapper.Easies.MySql.Tests
{
    [Collection("MySql")]
    public class MySqlBasicTest : BasicTest
    {
        public MySqlBasicTest(MySqlDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        public override void InsertTest(string sql, bool hasIdentityKey)
        {
            Assert.Equal("INSERT INTO `bnt_students`(`ClassId`, `StudentName`, `Age`, `CreateTime`) VALUES(@ClassId, @Name, @Age, @CreateTime); SELECT LAST_INSERT_ID()", sql);
            Assert.True(hasIdentityKey);
        }

        public override void UpdateTest(string sql)
        {
            Assert.Equal("UPDATE `bnt_students` SET `ClassId` = @ClassId, `StudentName` = @Name, `Age` = @Age, `CreateTime` = @CreateTime WHERE `Id` = @Id", sql);
        }

        public override void UpdateFieldsTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("UPDATE `bnt_students` t SET t.`StudentName` = @p0, t.`Age` = t.`Age` + @p1", sql);
            Assert.Equal("张三", parameters["p0"]);
            Assert.Equal(1, parameters["p1"]);
        }

        public override void UpdateWhereFieldsTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("UPDATE `bnt_students` t SET t.`StudentName` = @p0, t.`Age` = t.`Age` + @p1 WHERE t.`Id` = @p2", sql);
            Assert.Equal("张三", parameters["p0"]);
            Assert.Equal(1, parameters["p1"]);
            Assert.Equal(1, parameters["p2"]);
        }

        public override void DeleteTest(string sql)
        {
            Assert.Equal("DELETE FROM `bnt_students`  WHERE `Id` = @Id", sql);
        }

        public override void DeleteAllTest(string sql)
        {
            Assert.Equal("DELETE t FROM `bnt_students` t", sql);
        }

        public override void DeleteWhereTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("DELETE t FROM `bnt_students` t WHERE t.`StudentName` = @p0 AND t.`Age` = @p1", sql);
            Assert.Equal("张三", parameters["p0"]);
            Assert.Equal(18, parameters["p1"]);
        }

        public override void GetTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT `Id` AS `Id`, `ClassId` AS `ClassId`, `StudentName` AS `Name`, `Age` AS `Age`, `CreateTime` AS `CreateTime` FROM `bnt_students` WHERE `Id` = @Id LIMIT 0,1", sql);
            Assert.Equal(1, parameters["Id"]);
        }

        public override void GetMutipleIdTest(string sql, IParameterLookup parameters)
        {
            Assert.Equal("SELECT `Id` AS `Id`, `IdCard` AS `IdCard`, `ClassId` AS `ClassId`, `StudentName` AS `Name`, `Age` AS `Age`, `CreateTime` AS `CreateTime` FROM `bnt_mutiple_id_students` WHERE `Id` = @Id AND `IdCard` = @IdCard LIMIT 0,1", sql);
            Assert.Equal(1, parameters["Id"]);
            Assert.Equal(2, parameters["IdCard"]);
        }
    }
}
