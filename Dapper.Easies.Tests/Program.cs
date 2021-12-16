using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
                builder.AddMysql("Host=localhost;UserName=root;Password=123456;Database=School;Port=3306;CharSet=utf8mb4;Connection Timeout=1200;Allow User Variables=true;");
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
            //stu.StudentName = "李坤";
            //stu.Age = 18;
            //stu.CreateTime = DateTime.Now;
            //await easiesProvider.InsertAsync(stu);

            var temp = await easiesProvider.Query<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .Where((stu, cls) => stu.Age == 18)
                .OrderBy((a, b) => a.Age)
                .ThenBy((a, b) => b.CreateTime)
                .Select((a, b) => new { a.StudentName, ClassName = b.Name })
                .FirstOrDefaultAsync();

            var query = easiesProvider.Query<Student>()
                .Join<Class>((student, cls) => student.ClassId == cls.Id)
                .Where((stu, cls) => stu.Age == 18);
            //await easiesProvider.DeleteAsync<Student>();
            //await easiesProvider.DeleteAsync<Student>(o => o.Age == 18);
            await easiesProvider.DeleteCorrelationAsync(query);

            Console.WriteLine("{0} {1}", temp?.StudentName, temp?.ClassName);
        }

        public class StudentResponse
        {
            public string Name { get; set; }

            public string ClassName { get; set; }
        }
    }
}
