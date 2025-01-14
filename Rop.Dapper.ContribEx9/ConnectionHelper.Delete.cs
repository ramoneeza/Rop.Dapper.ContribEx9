using System.Data;
using Dapper;

namespace Rop.Dapper.ContribEx9;

public static partial class ConnectionHelper
{
    public static bool DeleteByKey<T>(this IDbConnection conn, dynamic id, IDbTransaction? tr = null, int? commandTimeout = null)
    {
        var sql = DapperHelperExtend.DeleteByKeyCache(typeof(T));
        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);
        var n = conn.Execute(sql, dynParams, tr, commandTimeout);
        return n > 0;
    }
    public static int DeleteByPartialKey<T>(this IDbConnection conn, dynamic id, IDbTransaction? tr = null, int? commandTimeout = null)
    {
        var sql = DapperHelperExtend.DeleteByPartialKeyCache(typeof(T));
        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);
        var n = conn.Execute(sql, dynParams, tr, commandTimeout);
        return n;
    }
    // Async 
    public static async Task<bool> DeleteByKeyAsync<T>(this IDbConnection conn, dynamic id, IDbTransaction? tr = null, int? commandTimeout = null)
    {
        var sql = DapperHelperExtend.DeleteByKeyCache(typeof(T));
        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);
        var n = await conn.ExecuteAsync(sql, dynParams, tr, commandTimeout);
        return n > 0;
    }
    public static async Task<int> DeleteByPartialKeyAsync<T>(this IDbConnection conn, dynamic id, IDbTransaction? tr = null, int? commandTimeout = null)
    {
        var sql = DapperHelperExtend.DeleteByPartialKeyCache(typeof(T));
        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);
        var n = await conn.ExecuteAsync(sql, dynParams, tr, commandTimeout);
        return n;
    }
}