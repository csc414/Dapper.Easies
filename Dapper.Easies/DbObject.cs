using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public ISqlSyntax SqlSyntax { get; internal set; }

        public string ConnectionStringName { get; internal set; }

        public string DbName { get; }

        public string EscapeName { get; internal set; }

        public Type Type { get; }

        public IEnumerable<DbProperty> Properties => _properties.Values.Where(o => !o.Ignore);

        public DbProperty IdentityKey { get; set; }

        public DbProperty this[string name] => _properties[name];

        internal bool Add(string name, DbProperty property) => _properties.TryAdd(name, property);

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

            public string EscapeName{ get; internal set; }

            public string EscapeNameAsAlias { get; internal set; }

            public bool PrimaryKey { get; set; }

            public bool IdentityKey { get; set; }

            public bool Ignore { get; set; }

            public PropertyInfo PropertyInfo { get; }
        }

        internal static string GetTablePropertyAlias(QueryContext context, DbProperty property)
        {
            return string.Format("{0}.{1}", context.Alias[property.PropertyInfo.ReflectedType].Alias, property.EscapeName);
        }

        internal static void Initialize(EasiesOptions options)
        {
            var assemblies = GetRuntimeAssemblies();
            var type = typeof(IDbObject);
            var objs = assemblies.SelectMany(o => o.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && type.IsAssignableFrom(t)));
            foreach (var t in objs)
            {
                var objAttr = t.GetCustomAttribute<DbObjectAttribute>();
                var obj = new DbObject(objAttr?.TableName ?? t.Name, t);
                obj.ConnectionStringName = objAttr?.ConnectionStringName;
                obj.SqlSyntax = options.GetSqlSyntax(obj.ConnectionStringName);
                obj.EscapeName = obj.SqlSyntax.EscapeTableName(obj.DbName);
                foreach (var p in t.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var attr = p.GetCustomAttribute<DbPropertyAttribute>();
                    var property = new DbProperty(attr?.PropertyName ?? p.Name, p);
                    property.PrimaryKey = attr?.PrimaryKey ?? false;
                    property.Ignore = attr?.Ignore ?? false;
                    property.EscapeName = obj.SqlSyntax.EscapePropertyName(property.DbName);
                    property.EscapeNameAsAlias = obj.SqlSyntax.PropertyNameAlias(new DbAlias(property.DbName, property.PropertyInfo.Name));
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
