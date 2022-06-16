using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using static Dapper.SqlMapper;

namespace Dapper.Easies.Tests
{
    public abstract class BasicTest : BaseTest
    {
        public BasicTest(IDapperEasiesFixture dapperEasiesFixture) : base(dapperEasiesFixture)
        {
        }

        [Fact]
        public void Insert()
        {
            var sql = SqlConverter.ToInsertSql<Student>(out var hasIdentityKey);
            InsertTest(sql, hasIdentityKey);
        }

        public abstract void InsertTest(string sql, bool hasIdentityKey);

        [Fact]
        public void Update()
        {
            DynamicParameters parameters;
            string sql;

            sql = SqlConverter.ToUpdateSql<Student>();
            UpdateTest(sql);
            
            Expression<Func<Student, Student>> updateFields = o => new Student { Name = "张三", Age = o.Age + 1 };
            var query = EasiesProvider.From<Student>();

            sql = SqlConverter.ToUpdateFieldsSql(updateFields, query.Context, out parameters);
            UpdateFieldsTest(sql, parameters);

            query.Where(o => o.Id == 1);
            //query.UpdateAsync(updateFields);

            sql = SqlConverter.ToUpdateFieldsSql(updateFields, query.Context, out parameters);
            UpdateWhereFieldsTest(sql, parameters);
        }

        public abstract void UpdateTest(string sql);

        public abstract void UpdateFieldsTest(string sql, IParameterLookup parameters);

        public abstract void UpdateWhereFieldsTest(string sql, IParameterLookup parameters);

        [Fact]
        public void Delete()
        {
            string sql;

            sql = SqlConverter.ToDeleteSql<Student>();
            DeleteTest(sql);

            var query = EasiesProvider.From<Student>();

            sql = SqlConverter.ToDeleteSql(query.Context, out _);
            DeleteAllTest(sql);

            query.Where(o => o.Name == "张三" && o.Age == 18);

            sql = SqlConverter.ToDeleteSql(query.Context, out var parameters);
            DeleteWhereTest(sql, parameters);
        }

        public abstract void DeleteTest(string sql);

        public abstract void DeleteAllTest(string sql);

        public abstract void DeleteWhereTest(string sql, IParameterLookup parameters);

        [Fact]
        public void Get()
        {
            DynamicParameters parameters;
            string sql;

            sql = SqlConverter.ToGetSql<Student>(new object[] { 1 }, out parameters);
            GetTest(sql, parameters);

            sql = SqlConverter.ToGetSql<MutipleIdStudent>(new object[] { 1, 2 }, out parameters);
            GetMutipleIdTest(sql, parameters);
        }

        public abstract void GetTest(string sql, IParameterLookup parameters);
        
        public abstract void GetMutipleIdTest(string sql, IParameterLookup parameters);
    }
}
