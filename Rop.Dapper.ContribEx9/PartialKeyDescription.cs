using System.Reflection;

namespace Rop.Dapper.ContribEx9;

/// <summary>
/// Immutable class with Partial Key Description for another Class
/// </summary>
public class PartialKeyDescription:KeyDescription
{
    public string Key2Name { get; }
    public PropertyInfo Key2Prop { get; }
    public bool Key2TypeIsString { get; }
    public override bool IsPartialKey => true;

    public PartialKeyDescription(string tableName, string keyName, PropertyInfo keyProp,string key2Name,PropertyInfo key2Prop,string foreignDatabase): base(tableName, keyName, keyProp,false,foreignDatabase)
    {
        Key2Name = key2Name;
        Key2Prop = key2Prop;
        Key2TypeIsString = Type.GetTypeCode(Key2Prop.PropertyType) == TypeCode.String;
    }
    public object GetKey2Value(object item)
    {
        return KeyProp.GetValue(item)??throw new Exception($"Key for {TableName} is null");
    }
    public string GetAllKeys(object item)
    {
        var key1 = GetKeyValue(item).ToString();
        var key2= GetKey2Value(item).ToString();
        return key1 + "|" + key2;
    }
    public (object,object) DeconstructKey(string key)
    {
        var keys = key.Split('|');
        object key1 = (KeyTypeIsString)?keys[0]:int.Parse(keys[0]);
        object key2 = (Key2TypeIsString) ? keys[1] : int.Parse(keys[1]);
        return (keys[0], keys[1]);
    }
}

public class LeftJoinDescription
{
    public PropertyInfo PropertyInfo { get; }
    public Type? JoinType { get; }
    public LeftJoinDescription(PropertyInfo propertyInfo,LeftJoinAttribute att)
    {
        PropertyInfo = propertyInfo;
        JoinType = att?.Type;
    }
    
}