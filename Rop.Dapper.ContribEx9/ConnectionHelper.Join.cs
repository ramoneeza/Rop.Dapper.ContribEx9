using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Rop.Dapper.ContribEx9;

public static partial class ConnectionHelper
{
    public static List<T> QueryJoin<T, M>(this IDbConnection conn, string query, object param, Action<T, M> join, IDbTransaction? tr = null)
    {
        var kd = DapperHelperExtend.GetKeyDescription(typeof(T));
        if (kd.KeyTypeIsString)
            return _queryJoin<T, M, string>(conn, query, param, join, tr);
        else
            return _queryJoin<T, M, int>(conn, query, param, join, tr);
    }
    private static List<T> _queryJoin<T, M, K>(IDbConnection conn, string query, object param, Action<T, M> join, IDbTransaction? tr = null) where K: notnull
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
    public static (T,M1[]) GetLeftJoin<T,M1>(this IDbConnection connection,dynamic id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class
    {
        var item= SqlMapperExtensions.Get<T>(connection, id, transaction, commandTimeout);
        var maniobra = GetPartial<M1>(connection, id, transaction, commandTimeout);
        return (item, maniobra.ToArray());
    }
    public static (T,M1[],M2[]) GetLeftJoin<T,M1,M2>(this IDbConnection connection,dynamic id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class
    {
        var item= SqlMapperExtensions.Get<T>(connection, id, transaction, commandTimeout);
        var maniobra1 = GetPartial<M1>(connection, id, transaction, commandTimeout);
        var maniobra2 = GetPartial<M2>(connection, id, transaction, commandTimeout);
        return (item, maniobra1.ToArray(), maniobra2.ToArray());
    }
    public static (T,M1[],M2[],M3[]) GetLeftJoin<T,M1,M2,M3>(this IDbConnection connection,dynamic id, IDbTransaction? transaction = null, int? commandTimeout = null) where T : class where M1 : class where M2:class where M3:class
    {
        var item= SqlMapperExtensions.Get<T>(connection, id, transaction, commandTimeout);
        var maniobra1 = GetPartial<M1>(connection, id, transaction, commandTimeout);
        var maniobra2 = GetPartial<M2>(connection, id, transaction, commandTimeout);
        var maniobra3= GetPartial<M3>(connection, id, transaction, commandTimeout);
        return (item, maniobra1.ToArray(), maniobra2.ToArray(),maniobra3.ToArray());
    }

}