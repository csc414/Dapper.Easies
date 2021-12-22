using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Easies.Tests
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
            });
            services.AddLogging(builder => {
                builder.AddConsole();
            });
            var serviceProvider = services.BuildServiceProvider();

            var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();

            //var cls = new Class();
            //cls.Id = Guid.NewGuid();
            //cls.Name = "六年二班";
            //cls.CreateTime = DateTime.Now;
            //await easiesProvider.InsertAsync(cls);


            //var stu = new Student();
            //stu.ClassId = cls.Id;
            //stu.StudentName = "李坤1";
            //stu.Age = 18;
            //stu.CreateTime = DateTime.Now;
            //await easiesProvider.InsertAsync(stu);
            var ary = new[] { 1, 2, 3 };
            var ls = new List<string> { "123", "456" };
            var student = await easiesProvider.GetAsync<Student>(2);
            student.Age = null;
            var count = await easiesProvider.Query<Student>()
                .Join<Class>((student, cls) => student.ClassId == Guid.Empty)
                .Where((a, b) => !a.IsOk && !(a.Age != 18) && (a.Age == (a.Age + 2) * 3) && DbFunction.In(a.StudentName, ls))
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .MinAsync((a,b) => a.Age);

            var temps = await easiesProvider.Query<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .Select((a, b) => new { Name = a.StudentName, ClassName = b.Name })
                .QueryAsync();

            foreach (var item in temps)
            {
                Console.WriteLine("{0} {1}", item.Name, item.ClassName);
            }

            var query = easiesProvider.Query<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .Where((stu, cls) => stu.Age == 18);

            await easiesProvider.UpdateAsync(student);
            await easiesProvider.UpdateAsync(() => new Student { Age = student.Age }, o => o.Id == 2);
           
            //await easiesProvider.DeleteAsync<Student>();
            //await easiesProvider.DeleteAsync<Student>(o => o.Age == 18);
            //await easiesProvider.DeleteAsync(query);
            //await easiesProvider.DeleteCorrelationAsync(query);
        }

        public class StudentResponse
        {
            public string Name { get; set; }

            public string ClassName { get; set; }
        }
    }
}
