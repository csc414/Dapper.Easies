using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Easies.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddEasiesProvider(builder =>
            {
                builder.Lifetime = ServiceLifetime.Singleton;
                builder.DevelopmentMode();
                //builder.UseSqlite("Data Source=D:\\school.db");
                builder.UseMySql("Host=localhost;UserName=root;Password=123456;Database=School;Port=3306;CharSet=utf8mb4;Connection Timeout=1200;Allow User Variables=true;");

                builder.UseSqlServer("MSSQL", "Data Source=localhost;User Id=sa;Password=123456@Cxc;Initial Catalog=School;");
            });
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            var serviceProvider = services.BuildServiceProvider();

            var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();

            var subQuery = easiesProvider.From<Student>().Where(c => c.ClassId == Guid.NewGuid()).Select(o => new { o.ClassId, o.StudentName });

            await easiesProvider.From<Class>()
                .Join(subQuery, (a, b) => a.Id == b.ClassId)
                .Where((a, b) => b.StudentName == "测试")
                .Select((a, b) => a)
                .QueryAsync();

            await easiesProvider.From<Class>()
                .Where(o => DbFunc.In(o.Id, easiesProvider.From<Class>().Select(o => o.Id).SubQuery()))
                .QueryAsync();

            var result = await easiesProvider.From<Class>()
                .Select((o) => new
                {
                    o.Name,
                    Count = easiesProvider.From<Student>()
                            .Where(c => c.ClassId == o.Id)
                            .Select(c => DbFunc.Count())
                            .SubQueryScalar()
                })
                .QueryAsync();

            var pager = await easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .Select((a, b) => a.Age)
                .GetPagerAsync(1, 10);

            await easiesProvider.From<Student>()
                .UpdateAsync(o => new Student { Age = o.Age + 1 });

            await easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .OrderBy((a, b) => b.CreateTime)
                .Select((a, b) => b)
                .QueryAsync();

            var cls = new Class();
            cls.Id = Guid.NewGuid();
            cls.Name = "六年二班1111";
            cls.CreateTime = DateTime.Now;

            var cls1 = new Class();
            cls1.Id = Guid.NewGuid();
            cls1.Name = "六年一班2222";
            cls1.CreateTime = DateTime.Now;

            //using (new DynamicDbMappingScope(map => map.SetTableName<Class>("bnt_class1")))
            //{
            //    var c = await easiesProvider.InsertAsync(new[] { cls, cls1 });

            //    using (new DynamicDbMappingScope(map => map.SetTableName<Class>("bnt_class2")))
            //    {
            //        cls.Name = "六年三班";
            //        cls1.Name = "六年四班";
            //        var d = await easiesProvider.UpdateAsync(new[] { cls, cls1 });
            //    }

            //    var i = await easiesProvider.DeleteAsync(new[] { cls, cls1 });
            //}

            //var cls2 = new MClass();
            //cls2.Id = Guid.NewGuid();
            //cls2.Name = "六年二班";
            //cls2.CreateTime = DateTime.Now;

            //var cls22 = new MClass();
            //cls22.Id = Guid.NewGuid();
            //cls22.Name = "六年一班";
            //cls22.CreateTime = DateTime.Now;
            //var cc = await easiesProvider.InsertAsync(new[] { cls2, cls22 });

            //cls2.Name = "六年三班";
            //cls22.Name = "六年四班";
            //var dd = await easiesProvider.UpdateAsync(new[] { cls2, cls22 });

            //var ii = await easiesProvider.DeleteAsync(new[] { cls2, cls22 });

            var a = easiesProvider.From<Student>()
                .Where(o => o.Age == 18 || !o.IsOk)
                .GroupBy(o => new { o.Id })
                .Having(o => DbFunc.Count(o.Id) > 0)
                .Select(o => new
                {
                    o.Id,
                    Count = easiesProvider.From<Class>()
                            .Where(a => a.Id == o.ClassId)
                            .GroupBy(a => a.Name)
                            .Select(o => DbFunc.Count(o.Name))
                            .SubQueryScalar()
                    
                })
                .OrderBy(o => o.Count)
                .ThenByDescending(o => o.Id)
                .GetPagerAsync(1, 10);

            var bb = easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .GroupBy((a, b) => b.Name)
                .Having((a, b) => DbFunc.Avg(a.Age) > 12)
                .Select((a, b) => new { ClassName = b.Name, AvgAge = DbFunc.Avg<decimal>(a.Age) })
                .QueryAsync();

            await Task.WhenAll(a, bb);

            var stu = new Student();
            stu.ClassId = cls.Id;
            stu.StudentName = "李坤1";
            stu.Age = 18;
            stu.CreateTime = DateTime.Now;
            await easiesProvider.InsertAsync(stu);
            var ary = new[] { 1, 2, 3 };
            var ls = new List<string> { "123", "456" };
            var dict = new Dictionary<string, string> { { "aa", "bb" } };
            var student = await easiesProvider.GetAsync<Student>(10);
            var count = await easiesProvider.From<Student>()
                .Join<Class>((student, cls) => $"{student.ClassId} != {Guid.Empty}")
                .Where((a, b) => $"{a.StudentName} != {dict["aa"]}")
                .Where((a, b) => !a.IsOk && !(a.Age != 18) && (a.Age == (a.Age + 2) * 3) && DbFunc.In(a.StudentName, ls) && DbFunc.Expr<bool>($"{a.StudentName} LIKE {$"%{cls.Name}%"}"))
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .MinAsync((a, b) => a.Age);

            var temps = await easiesProvider.From<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                //.Select((a, b) => a.StudentName)
                .Select((a, b) => new StudentResponse { Name = DbFunc.Expr<string>($"{a.StudentName}"), ClassName = b.Name })
                .QueryAsync();

            foreach (var item in temps)
            {
                Console.WriteLine("{0} {1}", item.Name, item.ClassName);
            }

            var query = easiesProvider.From<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .Where((stu, cls) => stu.Age == 18);

            await easiesProvider.UpdateAsync(student);

            await easiesProvider.From<Student>()
                .Where(o => o.Id == 2 && o.StudentName == DbFunc.Expr<string>($"{o.StudentName}"))
                .UpdateAsync(o => new Student { Age = DbFunc.Expr<int?>($"{student.Age}") });

            await easiesProvider.From<Student>()
                .Where(o => o.Id == 2)
                .UpdateAsync(() => new Student { Age = 18 });

            //await easiesProvider.Query<Student>().DeleteAsync();

            await easiesProvider.From<Student>().Where(o => o.Age == 20).DeleteAsync();
        }

        public class StudentResponse
        {
            public string Name { get; set; }

            public string ClassName { get; set; }
        }
    }
}
