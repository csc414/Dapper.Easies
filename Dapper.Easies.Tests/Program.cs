using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Easies.Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddEasiesProvider();
            services.AddEasiesMysql("Host=121.41.203.49;UserName=root;Password=cpsyb_mysql;Database=dev_openeasy_nonlocal_saas;Port=3306;CharSet=utf8mb4;Connection Timeout=1200;Allow User Variables=true;");
            var serviceProvider = services.BuildServiceProvider();
            var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();

            var student = await easiesProvider.Table<Student>().Where(o => o.Name == "name").FirstOrDefaultAsync();

            var query = easiesProvider.Table<Student>().Join<Teacher>((a, b) => a.TeacherId == b.Id, JoinType.Left).Where((student, teacher) => student.Name == "李钊");

            await easiesProvider.TransactionScopeAsync(async () =>
            {
                await easiesProvider.Table<Teacher>().Join<Student>((a, b) => a.Id == b.TeacherId).FirstOrDefaultAsync();
            });
        }
    }
}
