using System.Collections.Concurrent;
using System.Reflection;

namespace Rop.Dapper.ContribEx9;

public static partial class DapperHelperExtend
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, PartialKeyDescription> PartialKeyDescriptions = new();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo?> _partialKeyPropertiesCacheDic = new();
    private static PropertyInfo? _partialKeyPropertiesCache(Type type)
    {
        if (_partialKeyPropertiesCacheDic.TryGetValue(type.TypeHandle, out var prop)) return prop;
        List<PropertyInfo> source2 = TypePropertiesCache(type);
        List<PropertyInfo> list = source2.Where(p =>p.GetCustomAttributes(true).OfType<PartialKeyAttribute>().Any()).ToList();
        prop=(list.Count==1)? list[0] : null;
        _partialKeyPropertiesCacheDic[type.TypeHandle] = prop;
        return prop;
    }
    /// <summary>
    /// Get Key Description for class of type t
    /// </summary>
    /// <param name="t">Type of class</param>
    /// <returns>KeyDescription</returns>
    public static PartialKeyDescription? GetPartialKeyDescription(Type t)
    {
        if (PartialKeyDescriptions.TryGetValue(t.TypeHandle, out var kd)) return kd;
        var propkey = _partialKeyPropertiesCache(t);
        if (propkey == null) return null;
        var keyname = propkey.Name;
        var tname = GetTableName(t);
        var fdb = GetForeignDatabaseName(t);
        kd = string.IsNullOrEmpty(fdb) ? new PartialKeyDescription(tname, keyname, propkey) : new PartialKeyDescription(fdb,tname, keyname, propkey);
        PartialKeyDescriptions[t.TypeHandle]= kd;
        return kd;
    }
    public static PartialKeyDescription? GetPartialKeyDescription<T>() where T:class => GetPartialKeyDescription(typeof(T));

    /// <summary>
    /// Get Key Value for item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">Item</param>
    /// <returns>Item's Key</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static object? GetPartialKeyValue<T>(T item) where T:class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var kd = GetPartialKeyDescription(typeof(T))??throw new ArgumentException($"{typeof(T)} has not partial key");
        return kd.GetKeyValue(item);
    }
    /// <summary>
    /// Get Key Description and Key Value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>Key description and Key value</returns>
    public static (PartialKeyDescription keydescription, object? key) GetPartialKeyDescriptionAndValue<T>(T item) where T:class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var kd = GetPartialKeyDescription(typeof(T))??throw new ArgumentException($"{typeof(T)} has not partial key");
        var v = kd.GetKeyValue(item);
        return (kd, v);
    }
    public static PropertyInfo? GetPartialKey(Type t)
    {
        var kd = GetPartialKeyDescription(t);
        return kd?.KeyProp;
    }
}