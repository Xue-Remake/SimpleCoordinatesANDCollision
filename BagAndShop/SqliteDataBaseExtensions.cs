using SimpleSQLiteORM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop
{
    public static class SqliteDataBaseExtensions
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> _getByKeyCache = new();
        public static async Task<object?> GetByKeyAsync(this SqliteDataBase db, Type entityType,params object?[] keys)
        {
            // 找到泛型方法 GetByKeyAsync<T> 并构建
            var method = _getByKeyCache.GetOrAdd(entityType, type =>
            {
                var m = typeof(SqliteDataBase).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => x.Name == "GetByKeyAsync"
                     && x.IsGenericMethod
                     && x.GetParameters().Length == 1);
                return m.MakeGenericMethod(type);
            });
            // 调用参数为params object?[] keys
            var task = method.Invoke(db, new object[] { keys });

            await ((Task)task!).ConfigureAwait(false);
            return task!.GetType().GetProperty("Result")!.GetValue(task);
        }
    }
}
