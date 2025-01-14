using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Dapper.Contrib.Extensions;

namespace Rop.Dapper.ContribEx9;

public static partial class DapperHelperExtend
{
    private static readonly MethodInfo ExplicitKeyPropertiesCacheInfo;

    private static readonly MethodInfo KeyPropertiesCacheInfo;

    private static readonly MethodInfo GetTableNameInfo;

    private static readonly MethodInfo TypePropertiesCacheInfo;

    private static readonly MethodInfo ComputedPropertiesCacheInfo;

    private static readonly MethodInfo GetFormatterInfo;

    private static readonly FieldInfo GetQueriesInfo;

    private static T Invoke<T>(MethodInfo method, params object[] args)
    {
        Debug.Assert(method != null, nameof(method) + " != null");
        var obj=method.Invoke(null, args);
        if (obj is not T t) throw new InvalidOperationException($"Invalid cast {obj?.GetType()} to {typeof(T)}");
        return t;
    }

    private static ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries => (ConcurrentDictionary<RuntimeTypeHandle,string>)GetQueriesInfo.GetValue(null)!;

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> ForeignDatabase = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> SelectSlimDic = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetSlimDic = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetPartialDic = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> DeleteByKeyDic = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> DeleteByPartialKeyDic = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> InsertNoKeyAttDic = new();
    static DapperHelperExtend()
    {
        ExplicitKeyPropertiesCacheInfo = GetInfo("ExplicitKeyPropertiesCache");
        KeyPropertiesCacheInfo = GetInfo("KeyPropertiesCache");
        GetTableNameInfo = GetInfo("GetTableName");
        TypePropertiesCacheInfo = GetInfo("TypePropertiesCache");
        ComputedPropertiesCacheInfo = GetInfo("ComputedPropertiesCache");
        GetFormatterInfo = GetInfo("GetFormatter");
        GetQueriesInfo= typeof(SqlMapperExtensions).GetField("GetQueries", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException($"Invalid field GetMethod in static constructor");
        MethodInfo GetInfo(string name) => typeof(SqlMapperExtensions).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException($"Invalid method {name} in static constructor");
    }
        
        

}