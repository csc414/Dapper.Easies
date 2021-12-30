# Dapper.Easies - 是一个使用Lambda表达式就能轻易读写数据库的轻量级Orm，它基于Dapper。

安装使用
------------------------------------------------------------
```csharp
var services = new ServiceCollection();
services.AddEasiesProvider(builder =>
{
    //生命周期默认是 Scoped
    builder.Lifetime = ServiceLifetime.Scoped;

    //开启开发模式会向 Logger 输出生成的Sql。
    builder.DevelopmentMode();
    
    //目前只支持MySql
    builder.UseMySql("连接字符串");
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

    public string TableName { get; set; }
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
```
查询以及更新
------------------------------------------------------------
```csharp
//根据主键查询学生
var stu = await easiesProvider.GetAsync<Student>(1);
stu.Age = 11;

//此更新操作会更新除主键外所有字段
await easiesProvider.UpdateAsync(stu);

//此更新操作会按条件更新部分字段
await easiesProvider.UpdateAsync<Student>(o => new Student { Age = 12 }, o => o.Id == stu.Id);
```
删除
------------------------------------------------------------
```csharp
//全表删除，慎用
await easiesProvider.DeleteAsync<Student>();

//按条件删除
await easiesProvider.DeleteAsync<Student>(o => o.Age == 12);
```
高级查询
------------------------------------------------------------
```csharp
//查询年龄大于10岁，按年龄倒序后按名称排序的第一个学生
await easiesProvider.Query<Student>()
  .Where(o => o.Age > 10)
  .OrderByDescending(o => o.Age)
  .ThenBy(o => o.Name)
  .FirstOrDefaultAsync();

//查询年龄大于10岁并且是六年一班, 这里使用了匿名类并取了10条数据（也可以建DTO）
await easiesProvider.Query<Student>()
    .Join<Class>((a, b) => a.ClassId == b.Id)
    .Where((a, b) => a.Age > 10 && b.Name == "六年一班")
    .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name })
    .Take(10)
    .QueryAsync();
    
//分页例子
var page = 1;
var size = 10;
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => a.Age > 10 && b.Name == "六年一班");
var count = await query.CountAsync();
var maxPage = Convert.ToInt32(Math.Ceiling(count * 1f / size));
if (page > maxPage)
    page = maxPage;
await query.Skip((page - 1) * size).Take(size).QueryAsync();
```

高级删除
------------------------------------------------------------
```csharp
//查询年龄大于10岁并且是六年一班
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => a.Age > 10 && b.Name == "六年一班");

//把查询条件作为删除条件，根据条件删除主操作表 Student
await easiesProvider.DeleteAsync(query);

//把查询条件作为删除条件，根据条件删除主操作表和关联的表 Student 和 Class 表
await easiesProvider.DeleteCorrelationAsync(query);
```

DbFunction
------------------------------------------------------------
```csharp
//使用Like
var name = "张%";
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => DbFunction.Like(a.Name, name));

//使用In或NotIn
var names = new [] { "张三", "李四" };
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => DbFunction.In(a.Name, names) || DbFunction.NotIn(a.Name, names));
              
//使用Expression实现Like和In，Expression非常强大，可在无法用Lambda实现的情况下使用自定义表达式，并且可以和Lambda表达式混用
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Where((a, b) => a.Age > 10 && DbFunction.Expression<bool>($"{a.Name} LIKE {name} OR {a.Name} IN {names}"));
              
//Expression还可以使用在Selector
var query = easiesProvider.Query<Student>()
              .Join<Class>((a, b) => a.ClassId == b.Id)
              .Select((a, b) => new { StudentName = a.Name, ClassName = b.Name, IsYoung = DbFunction.Expression<bool>($"IF({a.Age} < {10}, 1, 0)") })
```
关于Sql转换
------------------------------------------------------------
所有函数或本地对象取值将在客户端计算后得到的值并且参数化，杜绝了Sql注入，请开启开发模式查看生成的Sql。
