using System.Reflection;

namespace Rop.Dapper.ContribEx9;

/// <summary>
/// Abstract class with Key Description for another Class
/// </summary>
public abstract class AbsKeyDescription
{
    public string TableName { get; }
    public string KeyName { get; }
    public PropertyInfo KeyProp { get; }
    public bool KeyTypeIsString { get; }
    public bool IsForeignTable { get; }
    public string ForeignDatabaseName { get; }
    public string GetUse()
    {
        return (IsForeignTable) ? $"USE {ForeignDatabaseName}; " : "";
    }
    public object? GetKeyValue(object item)
    {
        return KeyProp.GetValue(item);
    }
    protected AbsKeyDescription(string tableName, string keyName, PropertyInfo keyProp)
    {
        TableName = tableName;
        KeyName = keyName;
        KeyProp = keyProp;
        KeyTypeIsString = Type.GetTypeCode(KeyProp.PropertyType) == TypeCode.String;
        IsForeignTable = IsAForeignTable(tableName,out var fdbn);
        ForeignDatabaseName = fdbn;
    }
    protected AbsKeyDescription(string foreignDatabase, string tableName, string keyName, PropertyInfo keyProp)
    {
        TableName = tableName;
        KeyName = keyName;
        KeyProp = keyProp;
        KeyTypeIsString = Type.GetTypeCode(KeyProp.PropertyType) == TypeCode.String;
        IsForeignTable = true;
        ForeignDatabaseName = foreignDatabase;
    }
    public static bool IsAForeignTable(string tablename, out string foreigndatabase)
    {
        foreigndatabase = "";
        var res = tablename.Count(c => c == '.') >= 2;
        foreigndatabase = (res) ? (tablename.Split('.').FirstOrDefault()??"") : "";
        return res;
    }
}