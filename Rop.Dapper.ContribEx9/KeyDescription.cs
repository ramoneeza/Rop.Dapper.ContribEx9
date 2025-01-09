using System.Reflection;

namespace Rop.Dapper.ContribEx9;

/// <summary>
/// Immutable class with Key Description for another Class
/// </summary>
public class KeyDescription:AbsKeyDescription
{
    public bool IsAutoKey { get; }
    public KeyDescription(string tableName, string keyName, bool isAutoKey, PropertyInfo keyProp): base(tableName, keyName, keyProp)
    {
        IsAutoKey = isAutoKey;
    }
    public KeyDescription(string foreignDatabase, string tableName, string keyName, bool isAutoKey, PropertyInfo keyProp) : base(foreignDatabase, tableName, keyName, keyProp)
    {
        IsAutoKey = isAutoKey;
    }
}