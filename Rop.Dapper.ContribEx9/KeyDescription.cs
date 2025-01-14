using System.Reflection;

namespace Rop.Dapper.ContribEx9;

/// <summary>
/// Immutable class with Key Description for another Class
/// </summary>
public class KeyDescription : IKeyDescription
{
    public string TableName { get; }
    public string KeyName { get; }
    public PropertyInfo KeyProp { get; }
    public bool KeyTypeIsString { get; }
    public bool IsForeignTable { get; }
    public bool IsAutoKey { get; }
    public virtual bool IsPartialKey => false;
    public string ForeignDatabaseName { get; }
    public string GetUse()
    {
        return (IsForeignTable) ? $"USE {ForeignDatabaseName}; " : "";
    }
    public object GetKeyValue(object item)
    {
        return KeyProp.GetValue(item)??throw new Exception($"Key for {TableName} is null");
    }
    public KeyDescription(string tableName, string keyName, PropertyInfo keyProp, bool isAutoKey,string foreignDatabaseName)
    {
        TableName = tableName;
        KeyName = keyName;
        KeyProp = keyProp;
        KeyTypeIsString = Type.GetTypeCode(KeyProp.PropertyType) == TypeCode.String;
        IsAutoKey = isAutoKey;
        ForeignDatabaseName = foreignDatabaseName;
        IsForeignTable= ForeignDatabaseName != "";
        IsAutoKey = isAutoKey;
    }
    
}