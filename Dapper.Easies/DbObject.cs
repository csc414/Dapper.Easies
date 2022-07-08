using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dapper.Easies
{
    public class DbObject
    {
        private static Dictionary<Type, DbObject> _objs = new Dictionary<Type, DbObject>();

        private Dictionary<string, DbProperty> _properties = new Dictionary<string, DbProperty>();

        public DbObject(string dbName, Type type)
        {
            DbName = dbName;
            Type = type;
        }

        public IDbConnectionFactory ConnectionFactory { get; internal set; }

        public ISqlSyntax SqlSyntax { get; internal set; }

        public string ConnectionStringName { get; internal set; }

        public Func<IEnumerable<Expression>> Appender { get; internal set; }

        public string DbName { get; internal set; }

        public string EscapeName { get; internal set; }

        public Type Type { get; }

        public IEnumerable<DbProperty> Properties => _properties.Values.Where(o => !o.Ignore);

        public DbProperty IdentityKey { get; set; }

        public DbProperty this[string name]
        {
            get
            {
                _properties.TryGetValue(name, out var property);
                return property;
            }
        }

        internal bool Add(string name, DbProperty property) => _properties.TryAdd(name, property);

        public static DbObject Get<T>() where T : IDbObject => Get(typeof(T));

        public static DbObject Get(Type type)
        {
            if (_objs.TryGetValue(type, out var obj))
                return obj;
            return default;
        }

        public static IEnumerable<DbObject> Objects => _objs.Values;

        internal static bool Add(Type type, DbObject obj) => _objs.TryAdd(type, obj);

        public class DbProperty
        {
            public DbProperty(string dbName, PropertyInfo propertyInfo)
            {
                DbName = dbName;
                PropertyInfo = propertyInfo;
            }

            public string DbName { get; }

            public PropertyInfo PropertyInfo { get; }

            public string EscapeName { get; internal set; }

            public string EscapeNameAsAlias { get; internal set; }

            public bool PrimaryKey { get; internal set; }

            public bool IdentityKey { get; internal set; }

            public bool Ignore { get; internal set; }
        }

        internal static void Initialize(IServiceProvider serviceProvider, EasiesOptions options)
        {
            lock (_objs)
            {
                if (_objs.Count > 0)
                    return;

                var appenders = new List<(Type t, MethodInfo method, object obj)>();
                foreach (var t in options.Appenders)
                {
                    var obj = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, t);
                    appenders.AddRange(t.GetInterfaces().Where(o => o.IsGenericType).Select(o => (o.GenericTypeArguments.First(), o.GetMethod("Append"), obj)));
                }

                SqlMapper.AddTypeHandler(new UnmanagedTypeHandler<int>());
                SqlMapper.AddTypeHandler(new UnmanagedTypeHandler<long>());
                SqlMapper.AddTypeHandler(new UnmanagedTypeHandler<decimal>());

                var assemblies = GetRuntimeAssemblies();
                var type = typeof(IDbObject);
                var objs = assemblies.SelectMany(o => o.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && type.IsAssignableFrom(t)));
                foreach (var t in objs)
                {
                    var objAttr = t.GetCustomAttribute<DbObjectAttribute>();
                    var obj = new DbObject(objAttr?.TableName ?? t.Name, t);
                    obj.ConnectionStringName = objAttr?.ConnectionStringName;
                    obj.ConnectionFactory = options.GetConnectionFactory(obj.ConnectionStringName ?? EasiesOptions.DefaultName);
                    obj.SqlSyntax = options.GetSqlSyntax(obj.ConnectionStringName);
                    obj.EscapeName = obj.SqlSyntax.EscapeTableName(obj.DbName);
                    obj.Appender = GetAppendFunction(appenders.Where(o => o.t.IsAssignableFrom(t)));
                    foreach (var p in t.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var attr = p.GetCustomAttribute<DbPropertyAttribute>();
                        var property = new DbProperty(attr?.PropertyName ?? p.Name, p);
                        property.PrimaryKey = attr?.PrimaryKey ?? false;
                        property.Ignore = attr?.Ignore ?? false;
                        property.EscapeName = obj.SqlSyntax.EscapePropertyName(property.DbName);
                        property.EscapeNameAsAlias = obj.SqlSyntax.AliasPropertyName(property.EscapeName, obj.SqlSyntax.EscapePropertyName(property.PropertyInfo.Name));
                        obj.Add(p.Name, property);
                        if (attr != null && attr.PrimaryKey && attr.Identity && obj.IdentityKey == null)
                        {
                            property.IdentityKey = true;
                            obj.IdentityKey = property;
                        }
                    }
                    Add(t, obj);
                }
            }
        }

        static Func<IEnumerable<Expression>> GetAppendFunction(IEnumerable<(Type t, MethodInfo method, object obj)> funcs)
        {
            if (funcs.Any())
            {
                var exps = new List<Expression>();
                var lsType = typeof(List<Expression>);
                var lsVar = Expression.Variable(lsType);
                exps.Add(Expression.Assign(lsVar, Expression.New(lsType)));
                foreach (var item in funcs)
                    exps.Add(Expression.Call(lsVar, "Add", null, Expression.Call(Expression.Constant(item.obj), item.method)));
                exps.Add(lsVar);
                return Expression.Lambda<Func<IEnumerable<Expression>>>(Expression.Block(new ParameterExpression[] { lsVar }, exps)).Compile();
            }

            return default;
        }

        static IEnumerable<Assembly> GetRuntimeAssemblies()
        {
            var reg = new Regex("Microsoft\\..*|System.*|mscorlib|netstandard|WindowsBase", RegexOptions.Singleline | RegexOptions.Compiled);
            var context = DependencyContext.Default;
            var assemblies = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Where(o => !reg.IsMatch(o.Name))
                .Select(Assembly.Load)
                .ToArray();

            return assemblies;
        }
    }
}
