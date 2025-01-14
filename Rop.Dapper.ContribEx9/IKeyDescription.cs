using System.Reflection;

namespace Rop.Dapper.ContribEx9;

public interface IKeyDescription
{
    string TableName { get; }
    string KeyName { get; }
    PropertyInfo KeyProp { get; }
    bool KeyTypeIsString { get; }
    bool IsForeignTable { get; }
    bool IsAutoKey { get; }
    string ForeignDatabaseName { get; }
    string GetUse();
    object GetKeyValue(object item);
}

