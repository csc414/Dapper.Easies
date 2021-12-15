using Microsoft.Extensions.DependencyInjection;
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
            services.AddEasiesProvider(options =>
            {
                options.ConnectionString = "Host=localhost;UserName=root;Password=123456;Database=School;Port=3306;CharSet=utf8mb4;Connection Timeout=1200;Allow User Variables=true;";
                options.AddMysql();
            });
            var serviceProvider = services.BuildServiceProvider();

            var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();

            var student = new Student();
            student.ClassId = Guid.NewGuid();
            student.StudentName = "李坤";
            student.Age = 18;
            student.CreateTime = DateTime.Now;

            await easiesProvider.InsertAsync(student);
            //var stop = Stopwatch.StartNew();
            //await easiesProvider.InsertAsync(student);
            //stop.Stop();
            //Console.WriteLine("耗时：{0}", stop.ElapsedMilliseconds);

            var temp = await easiesProvider.Query<Student>().Join<Class>((student, cls) => student.ClassId == cls.Id, JoinType.Left).FirstAsync();
            
        }
    }

    class Test
    {
        public Guid MyProperty { get; set; }
    }
}
