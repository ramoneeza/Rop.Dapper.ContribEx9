using System.Collections.Concurrent;
using System.Reflection;

namespace Rop.Dapper.ContribEx9;

public static partial class DapperHelperExtend
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, PartialKeyDescription> PartialKeyDescriptions = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<LeftJoinDescription>> _leftjoinCacheDic = new();
    private static List<PropertyInfo> _partialKeyProperties(Type type)
    {
        List<PropertyInfo> source2 = TypePropertiesCache(type);
        var list = source2.GetPropertyWithAttribute<PartialKeyAttribute>(true).OrderBy(t=>t.attr.Order).Select(t=>t.property).ToList();
        return list;
    }
    private static List<LeftJoinDescription> _leftJoinDescriptionsCache(Type t)
    {
        return _leftjoinCacheDic.GetOrFactory(t, _ =>
        {
            var props = t.GetProperties();
            var list=props.GetPropertyWithAttribute<LeftJoinAttribute>(true).Select(p => new LeftJoinDescription(p.property,p.attr)).ToList();
            return list;
        });
    }

    /// <summary>
    /// Get Key Description for class of type t
    /// </summary>
    /// <param name="t">Type of class</param>
    /// <returns>KeyDescription</returns>
    public static PartialKeyDescription GetPartialKeyDescription(Type t)
    {
        return PartialKeyDescriptions.GetOrFactory(t, _ =>
        {
            var lst= _partialKeyProperties(t);
            if (lst.Count<2) throw new ArgumentException($"{t} has not two partial keys");
            var propkey = lst[0];
            var propkey2= lst[1];
            var keyname = propkey.Name;
            var key2Name=propkey2.Name;
            var tname = GetTableName(t);
            var fdb = GetForeignDatabaseName(t);
            return new PartialKeyDescription(tname, keyname, propkey,key2Name, propkey2,fdb);
        });
    }
    public static PartialKeyDescription GetPartialKeyDescription<T>() where T:class => GetPartialKeyDescription(typeof(T));

    /// <summary>
    /// Get Key Value for item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">Item</param>
    /// <returns>Item's Key</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static object? GetPartialKeyValue<T>(T item) where T:class
    {
        var kd = GetPartialKeyDescription<T>();
        return kd.GetKeyValue(item);
    }
    /// <summary>
    /// Get Key Description and Key Value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>Key description and Key value</returns>
    public static (PartialKeyDescription keydescription, object key) GetPartialKeyDescriptionAndValue<T>(T item) where T:class
    {
        var kd = GetPartialKeyDescription<T>();
        var v = kd.GetKeyValue(item);
        return (kd, v);
    }
    public static PropertyInfo GetPartialKey(Type t)
    {
        var kd = GetPartialKeyDescription(t);
        return kd.KeyProp;
    }
    public static List<LeftJoinDescription> GetLeftJoinDescription<T>()=> _leftJoinDescriptionsCache(typeof(T));
    public static List<LeftJoinDescription> GetLeftJoinDescription(Type t)=> _leftJoinDescriptionsCache(t);
    public static LeftJoinDescription? GetLeftJoinDescription<T,M>()
    {
        var all = _leftJoinDescriptionsCache(typeof(T));
        return all.FirstOrDefault(p => p.JoinType == typeof(M));
    }
}