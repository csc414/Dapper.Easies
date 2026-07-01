using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dapper.Easies;
using Dapper.Easies.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Easies.Benchmarks
{
    // 测量 SQL 生成层（不含 DB 往返）的开销：From<T>()...GetSql() 的完整路径。
    // 每次迭代都新建查询与表达式树，模拟真实每请求一查的场景。
    [MemoryDiagnoser]
    public class SqlGenerationBenchmarks
    {
        private IEasiesProvider _provider;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddEasiesProvider(builder => builder.UseMySql("Host=localhost;Database=_bench;"));
            _provider = services.BuildServiceProvider().GetRequiredService<IEasiesProvider>();
        }

        // 基线：无 where 全表查询
        [Benchmark(Baseline = true)]
        public (string sql, object _) FromAll() => _provider.From<Student>().GetSql();

        // 单字段 where（带捕获变量），结构稳定、值变化——模板缓存目标场景
        [Benchmark]
        public (string sql, object _) WhereById()
        {
            var id = 42;
            return _provider.From<Student>().Where(o => o.Id == id).GetSql();
        }

        // 复合 where
        [Benchmark]
        public (string sql, object _) WhereComplex()
        {
            var name = "abc";
            var age = 18;
            return _provider.From<Student>()
                .Where(o => o.Name == name && o.Age > age)
                .GetSql();
        }

        // Join + where + select
        [Benchmark]
        public (string sql, object _) JoinSelect()
        {
            var cid = System.Guid.NewGuid();
            return _provider.From<Student>()
                .Join<Class>((a, b) => a.ClassId == b.Id)
                .Where((a, _) => a.ClassId == cid)
                .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name })
                .GetSql();
        }

        // 分页
        [Benchmark]
        public (string sql, object _) Pager()
        {
            return _provider.From<Student>()
                .OrderBy(o => o.CreateTime)
                .GetSql(skip: 20, take: 10);
        }

        // 聚合
        [Benchmark]
        public (string sql, object _) Count()
        {
            var age = 18;
            return _provider.From<Student>().Where(o => o.Age > age).GetSql(aggregateInfo: new AggregateInfo(AggregateType.Count, null));
        }
    }

    public class Program
    {
        public static void Main(string[] args) =>
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
