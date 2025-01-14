using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rop.Dapper.ContribEx9;

public static class Helper
{
    public static K GetOrFactory<K>(this ConcurrentDictionary<RuntimeTypeHandle, K> dic,Type type, Func<Type, K> fn)
    {
        if (!dic.TryGetValue(type.TypeHandle, out var sql))
        {
            sql= fn(type);
            dic[type.TypeHandle] = sql;
        }
        return sql;
    }

    public static IEnumerable<(PropertyInfo property, A attr)> GetPropertyWithAttribute<A>(this IEnumerable<PropertyInfo> props,bool inherited)
    {
        foreach (var propertyInfo in props)
        {
            var lst = propertyInfo.GetCustomAttributes(inherited).OfType<A>().ToArray();
            if (lst.Length>0) yield return (propertyInfo, lst[0]);
        }    
    }
}