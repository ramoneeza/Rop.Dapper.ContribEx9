using System.Data;
using System.Reflection;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Rop.Dapper.ContribEx9;

public static partial class ConnectionHelper
{
    public static int InsertNoKeyAtt<T>(this IDbConnection connection, T entityToInsert,  IDbTransaction? transaction = null, int? commandTimeout = null) where T : class
    {   var sql = DapperHelperExtend.InsertNoKeyAtt(typeof(T));
        var r = connection.Execute(sql, entityToInsert, transaction, commandTimeout);
        return r;
    }
    
    
    public static int InsertOrUpdate<T>(this IDbConnection conn, T item, IDbTransaction? tr = null,int? timeout=null) where T : class
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(T));
        var objkey = kd.GetKeyValue(item);
        if (kd.IsAutoKey)
        {
            var key = (int)objkey;
            if (key <= 0)
            {
                key = (int) conn.Insert(item, tr,timeout);
            }
            else
            {
                conn.Update(item, tr,timeout);
            }
            return key;
        }
        else
        {
            if (objkey is int i)
            {
                var res = conn.Update(item, tr, timeout);
                if (!res) conn.Insert(item, tr, timeout);
                return i;
            }
            else
            {
                i = 1;
                var res = conn.Update(item, tr, timeout);
                if (!res) conn.Insert(item, tr, timeout);
                return i; // i=1 == sucessful 
            }
        }
    }
    
    public static int InsertOrUpdateMerge<T>(this IDbConnection conn, T item, IDbTransaction? tr = null, int? timeout = null) where T : class
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(T));
        var objkey = kd.GetKeyValue(item);
        if (kd.IsAutoKey)
        {
            var key = (int)objkey;
            if (key <= 0)
            {
                key = (int) conn.Insert(item, tr,timeout);
            }
            else
            {
                conn.Update(item, tr,timeout);
            }
            return key;
        }
        var tableName = kd.TableName;
        var keyName = kd.KeyName;
        var properties = DapperHelperExtend.TypePropertiesCache(typeof(T)).Where(c=>c.Name!=keyName).ToList();
        var propnames = DapperHelperExtend.GetColumnNames(properties).ToList();
        var columns = string.Join(", ", propnames);
        var values = string.Join(", ", propnames.Select(p => $"@{p}"));
        var updates = string.Join(", ", propnames.Select(p => $"{p} = @{p}"));
        var sql = $"""
                   
                           MERGE INTO {tableName} AS target
                           USING (SELECT @{keyName} AS {keyName}, {values}) AS source
                           ON (target.{keyName} = source.{keyName})
                           WHEN MATCHED THEN 
                               UPDATE SET {updates}
                           WHEN NOT MATCHED THEN
                               INSERT ({keyName}, {columns})
                               VALUES (@{keyName}, {values});
                       
                   """;
        var result = conn.Execute(sql, item, tr, timeout);
        if (objkey is int i)
            return (result > 0) ? i : 0;
        else
            return result;
    }
    
    
    public static bool UpdateIdValue<TA, T>(this IDbConnection conn, (dynamic id, T value) value, string field, IDbTransaction? tr = null, int? timeout = null)
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(TA));
        var sql = $"UPDATE {kd.TableName} SET {field}=@value WHERE {kd.KeyName}=@id";
        var r=conn.Execute(sql, new { id = value.id, value = value.value }, tr,timeout);
        return r == 1;
    }
    public static bool UpdateIdValue<TA, T>(this IDbConnection conn, dynamic id, T value, string field, IDbTransaction? tr = null, int? timeout = null)
    {
        return UpdateIdValue<TA, T>(conn, (id, value), field, tr, timeout);
    }
        
    // Async

    public static async Task<int> InsertOrUpdateAsync<T>(this IDbConnection conn, T item, IDbTransaction? tr = null,int? timeout=null) where T : class
    {
        return await Task.Run(() => conn.InsertOrUpdate(item, tr, timeout));

    }
    public static async Task<bool> UpdateIdValueAsync<TA, T>(this IDbConnection conn, (dynamic id, T value) value, string field, IDbTransaction? tr = null,int? timeout=null)
    {
        return await Task.Run(() => UpdateIdValue<TA, T>(conn, value, field, tr, timeout));
    }
    public static async Task<bool> UpdateIdValueAsync<TA, T>(this IDbConnection conn, dynamic id, T value, string field, IDbTransaction? tr = null, int? timeout = null)
    {
        return await Task.Run(() => UpdateIdValue<TA, T>(conn,id, value, field, tr, timeout));
    }

    public static bool InsertOrUpdatePartial<T>(this IDbConnection conn, dynamic id, IReadOnlyList<T> items, IDbTransaction tr, int? timeout = null) where T: class
    {
        
        var kd = DapperHelperExtend.GetPartialKeyDescription(typeof(T));
        var tablename=kd.TableName;
        var key1 = kd.KeyName;
        conn.Execute("DELETE * FROM tablename WHERE {key}=@{id}", new { id = id }, tr, timeout);
        var cnt = 0;
        foreach (var item in items)
        {
            kd.KeyProp.SetValue(item,id);
            cnt+=InsertNoKeyAtt<T>(conn, item, tr, timeout);
        }
        return cnt == items.Count;
    }
    
}