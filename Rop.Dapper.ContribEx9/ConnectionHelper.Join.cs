using System.Collections;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Rop.Dapper.ContribEx9;

public static partial class ConnectionHelper
{
    public static List<T> QueryJoin<T, M>(this IDbConnection conn, string query, object? param, Action<T, M> join, IDbTransaction? tr = null)
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(T));
        if (kd.KeyTypeIsString)
            return _queryJoin<T, M, string>(conn, query, param, join, tr);
        else
            return _queryJoin<T, M, int>(conn, query, param, join, tr);
    }
    private static List<T> _queryJoin<T, M, K>(IDbConnection conn, string query, object? param, Action<T, M> join, IDbTransaction? tr = null) where K: notnull
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(T));
        var kd2 = DapperHelperExtend.GetKeyDescription(typeof(M));
        var dic = new Dictionary<K, T>();
        var res1 = conn.Query<T, M, T>(query, map: (t, m) =>
        {
            var key = (K)DapperHelperExtend.GetKeyValue(t);
            if (!dic.TryGetValue(key, out var v))
            {
                v = t;
                dic[key] = t;
            }
            @join(v, m);
            return v;
        }, param: param, splitOn: kd2.KeyName, transaction: tr).ToList();
        return dic.Values.ToList();
    }
    
    public static IEnumerable<T> GetPartial<T>(this IDbConnection connection, dynamic id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class
    {
        var type = typeof(T);
        var sql = DapperHelperExtend.SelectGetPartialCache(type);
        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);
        var obj = connection.Query<T>(sql, dynParams, transaction, commandTimeout: commandTimeout);
        return obj;
    }
    public static IEnumerable<T> GetAllNoKey<T>(this IDbConnection connection, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class
    {
        var tname = DapperHelperExtend.GetTableName(typeof(T));
        var sql = "select * from " +tname;
        var all= connection.Query<T>(sql,null,transaction,true,commandTimeout);
        return all;
    }
    public static T GetRawLeftJoin<T,M1>(this IDbConnection connection,T item, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class
    {
        var id = DapperHelperExtend.GetKeyValue(item);
        var m1= DapperHelperExtend.GetLeftJoinDescription<T, M1>()??throw new ArgumentException($"No leftjoin for {typeof(M1)}");
        var maniobra = GetPartial<M1>(connection, id, transaction, commandTimeout);
        m1.PropertyInfo.SetValue(item, maniobra?.ToArray()??[]);
        return item;
    }
    public static T GetRawLeftJoin<T,M1,M2>(this IDbConnection connection,T item, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class
    {
        var id = DapperHelperExtend.GetKeyValue(item);
        var m1= DapperHelperExtend.GetLeftJoinDescription<T, M1>()??throw new ArgumentException($"No leftjoin for {typeof(M1)}");
        var m2= DapperHelperExtend.GetLeftJoinDescription<T, M2>()??throw new ArgumentException($"No leftjoin for {typeof(M2)}");
        var maniobra1 = GetPartial<M1>(connection, id, transaction, commandTimeout);
        var maniobra2 = GetPartial<M2>(connection, id, transaction, commandTimeout);
        m1.PropertyInfo.SetValue(item, maniobra1?.ToArray()??[]);
        m2.PropertyInfo.SetValue(item, maniobra2?.ToArray()??[]);
        return item;
    }
    public static T GetRawLeftJoin<T,M1,M2,M3>(this IDbConnection connection,T item, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class where M3:class
    {
        var id = DapperHelperExtend.GetKeyValue(item);
        var m1= DapperHelperExtend.GetLeftJoinDescription<T, M1>()??throw new ArgumentException($"No leftjoin for {typeof(M1)}");
        var m2= DapperHelperExtend.GetLeftJoinDescription<T, M2>()??throw new ArgumentException($"No leftjoin for {typeof(M2)}");
        var m3= DapperHelperExtend.GetLeftJoinDescription<T, M3>()??throw new ArgumentException($"No leftjoin for {typeof(M3)}");
        var maniobra1 = GetPartial<M1>(connection, id, transaction, commandTimeout);
        var maniobra2 = GetPartial<M2>(connection, id, transaction, commandTimeout);
        var maniobra3 = GetPartial<M3>(connection, id, transaction, commandTimeout);
        m1.PropertyInfo.SetValue(item, maniobra1?.ToArray()??[]);
        m2.PropertyInfo.SetValue(item, maniobra2?.ToArray()??[]);
        m3.PropertyInfo.SetValue(item, maniobra3?.ToArray() ?? []);
        return item;
    }
    public static T? GetLeftJoin<T,M1>(this IDbConnection connection,object id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class
    {
        var item = connection.Get<T>(id,transaction, commandTimeout);
        if (item == null) return null;
        return GetRawLeftJoin<T,M1>(connection,item,transaction, commandTimeout);
    }
    public static T? GetLeftJoin<T,M1,M2>(this IDbConnection connection,object id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2 : class
    {
        var item = connection.Get<T>(id,transaction, commandTimeout);
        if (item == null) return null;
        return GetRawLeftJoin<T,M1,M2>(connection,item,transaction, commandTimeout);
    }
    public static T? GetLeftJoin<T,M1,M2,M3>(this IDbConnection connection,object id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2 : class where M3 : class
    {
        var item = connection.Get<T>(id, transaction, commandTimeout);
        if (item == null) return null;
        return GetRawLeftJoin<T,M1,M2,M3>(connection,item,transaction, commandTimeout);
    }
    public static List<T> GetSomeLeftJoin<T,M1>(this IDbConnection connection,IEnumerable id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class
    {
        var items = connection.GetSome<T>(id,transaction,commandTimeout);
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
    public static List<T> GetSomeLeftJoin<T,M1,M2>(this IDbConnection connection,IEnumerable id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class
    {
        var items = connection.GetSome<T>(id,transaction,commandTimeout);
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1,M2>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
    public static List<T> GetSomeLeftJoin<T,M1,M2,M3>(this IDbConnection connection,IEnumerable id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class where M3:class
    {
        var items = connection.GetSome<T>(id,transaction,commandTimeout);
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1,M2,M3>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
    public static List<T> GetAllLeftJoin<T,M1>(this IDbConnection connection, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class
    {
        var items = connection.GetAll<T>(transaction,commandTimeout).ToList();
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
    public static List<T> GetAllLeftJoin<T,M1,M2>(this IDbConnection connection, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class
    {
        var items = connection.GetAll<T>(transaction,commandTimeout).ToList();
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1,M2>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
    public static List<T> GetAllLeftJoin<T,M1,M2,M3>(this IDbConnection connection, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class where M3:class
    {
        var items = connection.GetAll<T>(transaction,commandTimeout).ToList();
        foreach (var item in items)
        {
            GetRawLeftJoin<T,M1,M2,M3>(connection, item, transaction, commandTimeout);
        }
        return items;
    }
}