using System.Reflection;

namespace Rop.Dapper.ContribEx9;

/// <summary>
/// Immutable class with Partial Key Description for another Class
/// </summary>
public class PartialKeyDescription:AbsKeyDescription
{
    public PartialKeyDescription(string tableName, string keyName, PropertyInfo keyProp): base(tableName, keyName, keyProp)
    {
    }
    public PartialKeyDescription(string foreignDatabase, string tableName, string keyName, PropertyInfo keyProp) : base(foreignDatabase, tableName, keyName, keyProp)
    {
    }
}