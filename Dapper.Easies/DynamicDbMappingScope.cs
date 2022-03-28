using System;
using System.Collections.Generic;
using System.Threading;

namespace Dapper.Easies
{
    public sealed class DynamicDbMappingScope : IDbMapper, IDisposable
    {
        internal static readonly AsyncLocal<DbMappingHolder> _locals = new AsyncLocal<DbMappingHolder>();

        private readonly DbMappingHolder _oldHolder;

        private readonly Lazy<DbMappingHolder> _holder;

        public DynamicDbMappingScope(Action<IDbMapper> mapAction)
        {
            if(mapAction == null)
                throw new ArgumentNullException(nameof(mapAction));

            _oldHolder = _locals.Value;
            _holder = new Lazy<DbMappingHolder>(() =>
            {
                var holder = new DbMappingHolder();
                if (_oldHolder?.TableNameMap != null)
                    holder.TableNameMap = new Dictionary<DbObject, (string, string)>(_oldHolder.TableNameMap);
                else
                    holder.TableNameMap = new Dictionary<DbObject, (string, string)>();
                return holder;
            });
            
            mapAction.Invoke(this);
            if (_holder.IsValueCreated)
                _locals.Value = _holder.Value;
        }

        void IDbMapper.SetTableName<T>(string tableName)
        {
            var dbObject = DbObject.Get<T>();
            _holder.Value.TableNameMap[dbObject] = (tableName, dbObject.SqlSyntax.EscapeTableName(tableName));
        }

        public void Dispose()
        {
            if(_holder.IsValueCreated)
            {
                if (_oldHolder != null)
                    _locals.Value = _oldHolder;
                else
                {
                    _holder.Value.TableNameMap = null;
                    _locals.Value = null;
                }
            }
        }

        internal class DbMappingHolder
        {
            public Dictionary<DbObject, (string name, string escapeName)> TableNameMap;
        }
    }
}
