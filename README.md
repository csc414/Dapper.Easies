# Dapper.Easies - 是基于[Dapper](https://github.com/DapperLib/Dapper "Dapper")非常轻量的一个Orm组件。

安装使用
------------------------------------------------------------
`Install-Package Dapper.Easies.MySql`

或

`Install-Package Dapper.Easies.SqlServer`
```csharp
var services = new ServiceCollection();
services.AddEasiesProvider(builder =>
{
    //生命周期默认是 Singleton。
    //Singleton 为每个查询创建一个新的连接，支持异步查询，并Task.When等待。
    //Transient，Scoped 在 IEasiesProvider 生命周期中将共用一个 DbConnection ，可根据自身需求配置
    builder.Lifetime = ServiceLifetime.Singleton;

    //开启开发模式会向 Logger 输出生成的Sql。
    builder.DevelopmentMode();
    
    //目前只支持MySql, SqlServer
    builder.UseMySql("连接字符串");
    
    builder.UseSqlServer("多库配置名", "连接字符串");
});

var serviceProvider = services.BuildServiceProvider();

//注入 IEasiesProvider 使用
var easiesProvider = serviceProvider.GetRequiredService<IEasiesProvider>();
```

映射特性
------------------------------------------------------------
```csharp
//表或视图可使用该特性描述数据库中实际表名
public class DbObjectAttribute : Attribute
{
    public DbObjectAttribute() { }

    public DbObjectAttribute(string tableName)
    {
        TableName = tableName;
    }

    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// 连接字符串配置名 留空等同于 EasiesOptions.DefaultName = "Default";
    /// </summary>
    public string ConnectionStringName { get; set; }
}

//模型字段可使用该特性描述数据库中字段的一些属性
public class DbPropertyAttribute : Attribute
{
    public DbPropertyAttribute() { }

    public DbPropertyAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// 数据库字段名
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// 是否主键
    /// </summary>
    public bool PrimaryKey { get; set; }

    /// <summary>
    /// 是否自增长，在 PrimaryKey 等于 true 的情况下才生效
    /// </summary>
    public bool Identity { get; set; }

    /// <summary>
    /// 忽略该字段
    /// </summary>
    public bool Ignore { get; set; }
}
```

创建模型
------------------------------------------------------------
模型需继承 `IDbTable` 或 `IDbView` 接口。

继承 `IDbView` 的模型只能参与查询。
```csharp
//学生表
[DbObject("tb_student")]
public class Student : IDbTable
{
    [DbProperty(PrimaryKey = true, Identity = true)]
    public int Id { get; set; }

    public Guid ClassId { get; set; }

    [DbProperty("StudentName")]
    public string Name { get; set; }
    
    public int Age { get; set; }

    public DateTime CreateTime { get; set; }
}

//班级表
[DbObject("tb_class")]
public class Class : IDbTable
{
    [DbProperty(PrimaryKey = true)]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime CreateTime { get; set; }
}

//不同的库只需配置 ConnectionStringName 即可
[DbObject("tb_class", ConnectionStringName = "多库配置名")]
...
```

新增
------------------------------------------------------------
```csharp
var cls = new Class();
cls.Id = Guid.NewGuid();
cls.Name = "六年一班";
cls.CreateTime = DateTime.Now;
await easiesProvider.InsertAsync(cls);
  
var stu = new Student();
stu.ClassId = cls.Id;
stu.Name = "张三";
stu.Age = 10;
stu.CreateTime = DateTime.Now;
await easiesProvider.InsertAsync(stu);
//因为Student是自增Id，新增后 stu.Id 就有值了

//批量新增，注意批量新增无法回填自增Id
await easiesProvider.InsertAsync(new[] { stu });
```
更新
------------------------------------------------------------
```csharp
//根据主键查询学生
var stu = await easiesProvider.GetAsync<Student>(1);
stu.Age = 11;

//此更新操作会更新除主键外所有字段
await easiesProvider.UpdateAsync(stu);

//批量更新
await easiesProvider.UpdateAsync(new[] { stu });

//此更新操作会按条件更新部分字段
await easiesProvider.From<Student>().Where(o => o.Id == stu.Id).UpdateAsync(o => new Student { Age = o.Age + 1 });
```
删除
------------------------------------------------------------
```csharp
var stu = await easiesProvider.GetAsync<Student>(1);

//删除
await easiesProvider.DeleteAsync(stu);

//批量删除
await easiesProvider.DeleteAsync(new[] { stu });

//全表删除，慎用
await easiesProvider.From<Student>().DeleteAsync();

//按条件删除
await easiesProvider.From<Student>().Where(o => o.Age == 12).DeleteAsync();
```

多表关联查询
------------------------------------------------------------
```csharp
await easiesProvider.From<Student>()
    .Join<Class>((a, b) => a.ClassId == b.Id)
    .QueryAsync();   //多表关联未调用Select的情况下，默认返回主表 Student

//Join 子查询
var query = await easiesProvider.From<Class>()
    .Select(o => new { o.Id, o.Name })

await easiesProvider.From<Student>()
    .Join<Class>(query, (a, b) => a.ClassId == b.Id)
    .Select((a, b) => b)
    //.Select((a, b) => new { a.Name, ClassName = b.Name })
    .QueryAsync();  //Select b 匿名类，查询将返回匿名类
```
分组查询
------------------------------------------------------------
```csharp
await easiesProvider.From<Student>()
    .GroupBy(o => o.ClassId)
    .Having(o => DbFunc.Avg(o.Age) > 10)
    .Select(o => new { o.ClassId, AvgAge = DbFunc.Avg(o.Age), Count = DbFunc.Count() })
    .QueryAsync();
```
子查询
------------------------------------------------------------
```csharp
//DbFunc.In 或 NotIn 使用子查询
await easiesProvider.From<Student>()
    .Where(s => DbFunc.In(
        s.ClassId,
        EasiesProvider.From<Class>()
        .Select(c => c.Id)
        .SubQuery()
    ));

//Select 中使用子查询
await easiesProvider.From<Class>()
    .Select(c => new 
    {
        ClassName = c.Name,
        StudentCount = easiesProvider.From<Student>()
                        .Where(s => s.ClassId == c.Id)
                        .Select(s => DbFunc.Count())
                        .SubQueryScalar()
    })

//需要注意的是，为了性能的考虑，子查询中只支持Where中解析父表达式的参数，只能用于跟父表的关联。
```
分页扩展方法
------------------------------------------------------------
```csharp
(IEnumerable<T> data, long total, int max_page) result = await easiesProvider.From<Student>().GetPagerAsync(int page, int size);

(IEnumerable<T> data, long total) result = await easiesProvider.From<Student>().GetLimitAsync(int skip, int take)
```
DbFunc
------------------------------------------------------------
```csharp
//使用Like
var name = "张%";
var query = easiesProvider.From<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => DbFunc.Like(a.Name, name));

//使用In或NotIn
var names = new [] { "张三", "李四" };
var query = easiesProvider.From<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => DbFunc.In(a.Name, names) || DbFunc.NotIn(a.Name, names));
              
//使用Expr实现Like和In，Expr非常强大，可在无法用Lambda实现的情况下使用自定义表达式，并且可以和Lambda表达式混用
var query = easiesProvider.From<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => a.Age > 10 && DbFunc.Expr<bool>($"{a.Name} LIKE {name} OR {a.Name} IN {names}"));

//也可以直接在Join 或 Where 中使用Expr
var query = easiesProvider.From<Student>()
              .Join<Class>((a, b) => $"{a.ClassId} = {b.Id}")
              .Where((a, b) => $"{a.Name} LIKE {name} OR {a.Name} IN {names}");
              
//Expr还可以使用在Selector
var query = easiesProvider.From<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name, IsYoung = DbFunc.Expr<bool>($"IF({a.Age} < {10}, 1, 0)") });
              
//更新使用 Expr
await easiesProvider.From<Student>().UpdateAsync(o => new Student { Age = DbFunc.Expr<int>($"IF({o.Name} in {names}, 18, {a.Age})") });

//DbFunc 还提供基本的聚合函数 Count Avg Sum Max Min
```
强类型实体
------------------------------------------------------------
```csharp
var studentEntity = easiesProvider.Entity<Student>();

studentEntity.InsertAsync(...);
studentEntity.UpdateAsync(...);
studentEntity.DeleteAsync(...);
studentEntity.Where(...);
...
//使用强类型实体可省略每次调用传递泛型，更多的方法不在此展示。
```
动态映射表名
------------------------------------------------------------
```csharp
//可用于分表操作

//Class = tb_class
using (new DynamicDbMappingScope(map => map.SetTableName<Class>("tb_class1")))
{
    //Class = tb_class1
    await easiesProvider.InsertAsync(...);

    using (new DynamicDbMappingScope(map => map.SetTableName<Class>("tb_class2")))
    {
        //Class = tb_class2
        await easiesProvider.InsertAsync(...);
    }

    //Class = tb_class1
    await easiesProvider.InsertAsync(...);
}
//Class = tb_class
```
原生Sql执行
------------------------------------------------------------
```csharp
//生命周期默认是 Singleton。
//Singleton 为每个查询创建一个新的连接。
//Transient，Scoped 在 IEasiesProvider 生命周期中将共用一个 DbConnection。
await easiesProvider.Connection.ExecuteAsync(...);

//多库
await easiesProvider.GetConnection("多库配置名").ExecuteAsync(...);
```

关于事务
------------------------------------------------------------
基于 [TransactionScope](https://docs.microsoft.com/zh-cn/dotnet/api/system.transactions.transactionscope?view=net-6.0 "TransactionScope") 做了简单的封装。
```csharp
//在回调函数中的代码将会在同一个事务中执行，事务的回滚是基于异常，如需基于逻辑判断触发回滚可主动抛出异常。
await easiesProvider.TransactionScopeAsync(async () =>
{
    var cls = new Class();
    cls.Id = Guid.NewGuid();
    cls.Name = "六年一班";
    cls.CreateTime = DateTime.Now;
    await easiesProvider.InsertAsync(cls);

    var stu = new Student();
    stu.ClassId = cls.Id;
    stu.Name = "张三";
    stu.Age = 10;
    stu.CreateTime = DateTime.Now;
    await easiesProvider.InsertAsync(stu);
});
```
关于批量操作
------------------------------------------------------------
批量新增，更新，删除是基于 `Dapper` 的 [Execute a Command multiple times](https://github.com/DapperLib/Dapper#execute-a-command-multiple-times "Execute a Command multiple times")

关于Sql转换
------------------------------------------------------------
所有函数或本地对象取值将在客户端计算后得到的值并且参数化，完全杜绝了Sql注入，可在开发过程中开启 `DevelopmentMode` 查看生成的Sql。
