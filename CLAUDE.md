# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

Dapper.Easies 是一个基于 Dapper 的轻量级 LINQ 风格 ORM（核心库 `Dapper.Easies`，`netstandard2.1`，C# 9）。它把 `Expression` 树编译成参数化 SQL，再交给 Dapper 执行。仓库还包含三个数据库方言扩展包（MySql / SqlServer / Sqlite）、一个 Demo 控制台程序，以及测试工程。

## 常用命令

```bash
# 构建整个解决方案
dotnet build Dapper.Easies.sln

# 运行测试（实际可运行的测试在 Dapper.Easies.MySql.Tests 中，见下文）
dotnet test Dapper.Easies.MySql.Tests/Dapper.Easies.MySql.Tests.csproj

# 运行单个测试
dotnet test Dapper.Easies.MySql.Tests/Dapper.Easies.MySql.Tests.csproj --filter "FullyQualifiedName~MySqlBasicTest.Insert"

# 运行 Demo（需要在本机配置对应数据库，见 Dapper.Easies.Demo/Program.cs）
dotnet run --project Dapper.Easies.Demo
```

核心库与各扩展包都是 `netstandard2.1`；测试工程为 `netcoreapp3.1`，使用 xUnit。

## 架构要点

### 元数据注册：`DbObject`
`DbObject`（`Dapper.Easies/DbObject.cs`）是一个**进程级静态注册表**。`DbObject.Initialize` 在 `DefaultSqlConverter` 构造时被调用一次：扫描所有运行时程序集（排除 `System.*`/`Microsoft.*`/mscorlib 等），为每个 `IDbObject` 实现类反射出表名、字段、主键、自增键，并绑定该表对应的 `IDbConnectionFactory` 与 `ISqlSyntax`。结果缓存在 `DbObject._objs` 中。`DbObject.Get<T>()` 是后续所有 SQL 生成的元数据来源。

实体用 `[DbObject(TableName, ConnectionStringName)]` 与 `[DbProperty(PropertyName, PrimaryKey, Identity, Ignore)]` 标注；不标注则按类名/属性名映射。`IDbObject` 是空标记接口，`IDbTable` / `IDbView` 继承自它。

### 三层 SQL 生成流水线
1. **`QueryContext`**（`QueryContext.cs`）：可变的查询构建状态，持有 where / join / having / orderBy / thenBy / select / groupBy / skip / take / distinct 等表达式，以及表别名列表（`t`, `t0`, `t1` …）。实现了 `ICloneable`。
2. **`ISqlConverter`**（`DefaultSqlConverter`）：高层入口，把 `QueryContext` 翻译成具体 SQL 字符串（`ToQuerySql`/`ToInsertSql`/`ToUpdateSql`/`ToDeleteSql`/`ToGetSql` 等），并管理 `ParameterBuilder` 与开发模式下的 SQL 日志。它把真正拼 SQL 的活委托给 `context.DbObject.SqlSyntax`。
3. **`ISqlSyntax`**（`Abstractions/ISqlSyntax.cs`）：**按数据库方言**生成 SQL（`SelectFormat`/`InsertFormat`/`UpdateFormat`/`DeleteFormat` + 标识符转义 `EscapeTableName`/`EscapePropertyName`/`AliasTableName`）。实现：`MySqlSqlSyntax`、`SqlServerSqlSyntax`、`SqliteSqlSyntax`，各自还配有 `XxxDbConnectionFactory` 和 `XxxExpressionParser`（表达式树访问器）。每个方言包通过 `EasiesOptionsBuilder.UseMySql/UseSqlServer/UseSqlite` 注册自己的 factory 与 syntax 实例。

### 查询 API：`DbQuery<T1..T7>`
`DbQuery<T>`（`DbQuery.cs`）是 fluent 查询对象，构造时接收一个 `QueryContext`。关键设计：**所有 fluent 方法都 mutate 同一个共享的 `QueryContext`，然后返回一个新的、按 arity 重新包装的 `DbQuery`**。例如 `Select<TResult>()` 返回 `DbQuery<TResult>`，但内部仍是同一个 context；`Join<TJoin>` 把类型参数从 `T1` 扩展到 `T1,T2`（最多到 7 个表）。因此一个链式调用从头到尾操作同一份状态。

`DbQuery<T1..T7>` 按泛型 arity 逐层继承，每个 arity 都重复声明了 `Where`/`OrderBy`/`ThenBy`/`Join`/`GroupBy`/`Having`/`Select`/聚合 等方法的各种泛型重载（含 `Expression<Func<T1,T2,...,bool>>` 与原始 SQL 字符串 `Expression<Func<...string>>` 两种形式）。**新增一个 arity 或重载时，需要在 `DbQuery.cs` 与 `Abstractions/IDbQuery*.cs`、`IGroupingDbQuery*.cs` 等对应接口中同步声明。** 执行方法（`QueryAsync`/`FirstAsync`/`CountAsync`/`GetPagerAsync`/`GetLimitAsync`/`DeleteAsync`/`UpdateAsync`/`Max`/`Min`/`Sum`/`Avg`）通过 `InternalExecuteAsync` 打开连接并调用 Dapper。

查询辅助扩展在 `IDbQueryExtensions.cs`：`WhereIf`、`Skip`/`Take`、`Distinct`、`NewQuery`（基于 `Clone` 复用条件重开查询）、`NoAppender`、`SubQuery`/`SubQueryScalar`（把子查询嵌入 `In`/标量场景）、`GetSql`（只生成 SQL 不执行）。

### `DbFunc` —— 表达式中的 SQL 函数标记
`DbFunc`（`DbFunc.cs`）中的静态方法（`Like`/`In`/`NotIn`/`Min`/`Max`/`Sum`/`Avg`/`Count`/`IsNull`/`IsNotNull`/`N`/`Expr`）**运行时都会抛 `InvalidOperationException`**——它们只在 LINQ 表达式树里作为标记节点存在，由 `SqlExpressionParser` 识别并翻译成对应 SQL。`DbFunc.Expr("raw sql")` 与 `Where(Expression<Func<T,string>>)` 重载允许在 lambda 中嵌入原始 SQL 片段。

### Appender —— 全局查询过滤
实现 `IAppender<T>` 的类通过 `EasiesOptionsBuilder.UseAppender<T>()` 注册。`DbObject.Initialize` 会为每个匹配的实体编译一个 `Appender` 委托，`QueryContext.WhereExpressions` 在首次访问时把这些默认 where 表达式并入（多租户过滤等场景）。`.NoAppender()` 可在单条查询上关闭该行为。

### 多数据库与连接
`EasiesOptions` 以名称为键保存多组 `IDbConnectionFactory` 与 `ISqlSyntax`（`Default` = `"Default"`）。每个 `[DbObject(ConnectionStringName=...)]` 选择一组配置。**`QueryContext.AddJoin` 会拒绝跨连接配置的 Join**（抛 `ArgumentException`）。事务通过 `EasiesProviderExtensions.TransactionScopeAsync` 包装 `TransactionScope`（启用 async flow），`TransactionRollback` 提供受控回滚并可携带返回值。

### 依赖注入与单例
`ServiceCollectionExtensions.AddEasiesProvider`（位于 `Microsoft.Extensions.DependencyInjection` 命名空间，需要 `using` 该命名空间才能看到扩展方法）注册三个单例：`IEasiesProvider`→`DefaultEasiesProvider`、`ISqlConverter`→`DefaultSqlConverter`、`EasiesOptions`。注意 `EasiesOptionsBuilder` 自身也是单例（`Instance`），`EasiesOptions` 在其上懒构造。典型用法见 `Dapper.Easies.Demo/Program.cs`。

## 测试结构

- `Dapper.Easies.Tests`（`netcoreapp3.1`）包含**抽象**测试基类 `BasicTest` / `DbQueryTest`、抽象 `DapperEasiesFixture`、共享实体（`Entities/`）和 `IDapperEasiesFixture` 接口。这些测试**不连接真实数据库**——它们调用 `ISqlConverter.ToXxxSql(...)` 生成 SQL 字符串，再用 `Assert.Equal` 断言生成的 SQL 文本，因此可以在无 DB 环境下运行。
- `Dapper.Easies.MySql.Tests` 通过 `MySqlDapperEasiesFixture`（调用 `builder.UseMySql(null)`，连接字符串为 null 也不影响，因为只断言 SQL 文本）具体化上述抽象测试，并用 `[Collection("MySql")]` + `ICollectionFixture<MySqlDapperEasiesFixture>` 共享 fixture。
- **新增方言的测试**：新建 `Dapper.Easies.Xxx.Tests` 工程，引用 `Dapper.Easies.Tests`，继承 `DapperEasiesFixture` 并 override `Initialization` 调用 `UseXxx`，再继承 `BasicTest`/`DbQueryTest` 实现 `XxxTest` 抽象方法断言该方言的 SQL。
- 核心库通过 `[assembly: InternalsVisibleTo("Dapper.Easies.Tests")]` 暴露 internal 成员给测试。
