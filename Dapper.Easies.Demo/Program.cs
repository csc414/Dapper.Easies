using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
                builder.DevelopmentMode();
                builder.UseMySql("Host=localhost;UserName=root;Password=123456;Database=School;Port=3306;CharSet=utf8mb4;Connection Timeout=1200;Allow User Variables=true;");

                //builder.UseSqlServer("Data Source=localhost;User Id=sa;Password=123456@Cxc;Initial Catalog=School;Encrypt=False;");
            });
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            var serviceProvider = services.BuildServiceProvider();

            var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();

            await easiesProvider.From<Class>()
                .Select<TestClass>()
                .QueryAsync();

            var subQuery = easiesProvider.From<Student>().Where(c => c.ClassId == Guid.NewGuid()).Select(o => new { o.ClassId, o.Name });

            using (AsyncExecutionScope.Create())
            {
                var a = easiesProvider.From<Class>()
                    .Join(subQuery, (a, b) => a.Id == b.ClassId)
                    .Where((a, b) => b.Name == "测试")
                    .WhereIf(true, (a, b) => DbFunc.IsNotNull(a.Id))
                    .OrderBy((a, b) => b.ClassId)
                    .Select((a, b) => a)
                    .QueryAsync();

                var b = easiesProvider.From<Class>()
                    .Where(o => DbFunc.In(o.Id, easiesProvider.From<Class>().Where(x => x.Id == o.Id).Select(o => o.Id).SubQuery()))
                    .OrderBy(o => o.CreateTime)
                    .GetPagerAsync(2, 2);

                var c = easiesProvider.From<Class>()
                    .Select((o) => new
                    {
                        o.Name,
                        Count = easiesProvider.From<Student>()
                                .Where(c => c.ClassId == o.Id)
                                .Select(c => DbFunc.Count())
                                .SubQueryScalar()
                    })
                    .QueryAsync();

                var d = easiesProvider.From<Student>()
                   .Join<Class>((a, b) => a.ClassId == b.Id)
                   .Where((o, _) => o.IsAdult && o.Name.Contains("阿萨"))
                   .GroupBy((o, _) => new { o.IsAdult, o.ClassId })
                   .Having((o, _) => DbFunc.Count() > 0)
                   .Select((o, _) => new { o.IsAdult, o.ClassId })
                   .OrderBy(o => o.IsAdult)
                   .ThenByDescending(o => o.ClassId)
                   .QueryAsync();

                await Task.WhenAll(a, b, c, d);
            }

            var pager = await easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .OrderBy((a, b) => a.CreateTime)
                .Select((a, b) => new { a.Age, a.Id })
                .GetPagerAsync<(int?, int)>(2, 2);

            await easiesProvider.From<Student>()
                .UpdateAsync(o => new Student { Age = o.Age + 1 });

            await easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .OrderBy((a, b) => a.CreateTime)
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

            //using (DynamicDbMappingScope.Create(map => map.SetTableName<Class>("bnt_class1")))
            //{
            //    var c = await easiesProvider.InsertAsync(new[] { cls, cls1 });

            //    using (DynamicDbMappingScope.Create(map => map.SetTableName<Class>("bnt_class2")))
            //    {
            //        cls.Name = "六年三班";
            //        cls1.Name = "六年四班";
            //        var d = await easiesProvider.UpdateAsync(new[] { cls, cls1 });
            //    }

            //    var i = await easiesProvider.DeleteAsync(new[] { cls, cls1 });
            //}

            await easiesProvider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .GroupBy((a, b) => b.Name)
                .Having((a, b) => DbFunc.Avg(a.Age) > 12)
                .Select((a, b) => new { ClassName = b.Name, AvgAge = DbFunc.Avg<decimal>(a.Age) })
                .QueryAsync();


            var stu = new Student();
            stu.ClassId = cls.Id;
            stu.Name = "李坤1";
            stu.Age = 18;
            stu.CreateTime = DateTime.Now;
            await easiesProvider.InsertAsync(stu);
            var ary = new[] { 1, 2, 3 };
            var ls = new List<string> { "123", "456" };
            var dict = new Dictionary<string, string> { { "aa", "bb" } };
            var student = await easiesProvider.From<Student>().FirstOrDefaultAsync();
            var count = await easiesProvider.From<Student>()
                .Join<Class>((student, cls) => $"{student.ClassId} != {Guid.Empty}")
                .Where((a, b) => $"{a.Name} != {dict["aa"]}")
                .Where((a, b) => !a.IsAdult && !(a.Age != 18) && (a.Age == (a.Age + 2) * 3) && DbFunc.In(a.Name, ls) && DbFunc.Expr<bool>($"{a.Name} LIKE {$"%{cls.Name}%"}"))
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .MinAsync((a, b) => a.Age);

            var temps = await easiesProvider.From<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .Select((a, b) => new StudentResponse { Name = DbFunc.Expr<string>($"{a.Name}"), ClassName = b.Name })
                .QueryAsync();

            foreach (var item in temps)
            {
                Console.WriteLine("{0} {1}", item.Name, item.ClassName);
            }

            var query = easiesProvider.From<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .Where((stu, cls) => stu.Age == 18);

            if (student != null)
            {
                await easiesProvider.UpdateAsync(student);

                await easiesProvider.From<Student>()
               .Where(o => o.Id == 2 && o.Name == DbFunc.Expr<string>($"{o.Name}"))
               .UpdateAsync(o => new Student { Age = DbFunc.Expr<int?>($"{student.Age}") });
            }

            await easiesProvider.From<Student>()
                .Where(o => o.Id == 2)
                .UpdateAsync(() => new Student { Age = 18 });

            await easiesProvider.From<Student>().Where(o => o.Age == 99).DeleteAsync();
        }

        public class StudentResponse
        {
            public string Name { get; set; }

            public string ClassName { get; set; }
        }
    }
}
