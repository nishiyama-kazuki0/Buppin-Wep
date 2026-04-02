using Microsoft.Data.SqlClient;
using SharedModels;
using System.Collections.Frozen;
using System.Data;
using static SharedModels.SharedConst;


namespace ExpressionDBWebAPI.Common;

public class DataSource
{
    private static IDataSource _DatabaseAccess = null!;
    public static string ParamPrefixStr = "@";//TODO コンストラクタでDB種類によって変更する
    public FrozenDictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>() {
        { typeof(byte), DbType.Byte }
        , { typeof(sbyte), DbType.SByte }
        , { typeof(short), DbType.Int16 }
        , { typeof(ushort), DbType.UInt16 }
        , { typeof(int), DbType.Int32 }
        , { typeof(uint), DbType.UInt32 }
        , { typeof(long), DbType.Int64 }
        , { typeof(ulong), DbType.UInt64 }
        , { typeof(float), DbType.Single }
        , { typeof(double), DbType.Double }
        , { typeof(decimal), DbType.Decimal }
        , { typeof(bool), DbType.Boolean }
        , { typeof(string), DbType.String }
        , { typeof(char), DbType.StringFixedLength }
        , { typeof(Guid), DbType.Guid }
        , { typeof(DateTime), DbType.DateTime }
        , { typeof(DateTimeOffset), DbType.DateTimeOffset }
        , { typeof(byte[]), DbType.Binary }
        , { typeof(byte?), DbType.Byte }
        , { typeof(sbyte?), DbType.SByte }
        , { typeof(short?), DbType.Int16 }
        , { typeof(ushort?), DbType.UInt16 }
        , { typeof(int?), DbType.Int32 }
        , { typeof(uint?), DbType.UInt32 }
        , { typeof(long?), DbType.Int64 }
        , { typeof(ulong?), DbType.UInt64 }
        , { typeof(float?), DbType.Single }
        , { typeof(double?), DbType.Double }
        , { typeof(decimal?), DbType.Decimal }
        , { typeof(bool?), DbType.Boolean }
        , { typeof(char?), DbType.StringFixedLength }
        , { typeof(Guid?), DbType.Guid }
        , { typeof(DateTime?), DbType.DateTime }
        , { typeof(DateTimeOffset?), DbType.DateTimeOffset }
    }.ToFrozenDictionary();
    /// <summary>
    /// 引用符を付加する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string AddQuotationMarks(string str)
    {
        return _DatabaseAccess.AddQuotationMarks(str);
    }
    private readonly IConfiguration? _configuration;
    public static void setDataSource()//Todoやり方をもう少し何とかしたい
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        int dbtype = config.GetValue<int>("DBSourceType");
        _DatabaseAccess ??= DatabaseFactory.CreateDatabaseAccess((TYPE_DB_TYPE)dbtype);
    }

    /// <summary>
    /// DB接続文字列の取得
    /// </summary>
    /// <returns></returns>
    public static string GetConnectString()
    {
        return _DatabaseAccess.GetConnectString();
    }

    /// <summary>
    /// 新しいDBConnectionを取得する。
    /// 主に本クラス外部でもコネクションを使用する場合に使用する。外側の変数は原則、usingで受けること。//Todo コネクション作成CreateConnection()を使用するので削除予定
    /// 例.トランザクションを外側で張りたいときなど
    /// </summary>
    /// <returns></returns>
    public static IDbConnection GetNewConnection()
    {
        //TODO 戻りの型はSqlConnectionになっているが、Oracleでも問題ないか確認する必要あり。問題があるなら、IDbConnectionにするなどの対応が必要。他メソッドも同様。コントローラー側の使用箇所も同様。
        return _DatabaseAccess.GetNewConnection();
    }

    /// <summary>
    /// connectionを参照引数で受けて、OpenしてなかったらOpenする
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public static void OpenConnectionEnsure(IDbConnection connection)
    {
        _DatabaseAccess.OpenConnectionEnsure(connection);
    }

    /// <summary>
    /// 引数で受けたconnectionのトランザクションを開始する。開始後にトランザクションオブジェクトを返す。
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public static IDbTransaction BeginTransactionAndOpenEnsure(IDbConnection connection)
    {
        return _DatabaseAccess.BeginTransactionAndOpenEnsure(connection);
    }

    /// <summary>
    /// トランザクションをコミットして、connectionを閉じる。閉じる前にコミットすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public static void CommitTransactionAndCloseEnsure(IDbTransaction transaction)
    {
        _DatabaseAccess.CommitTransactionAndCloseEnsure(transaction);
    }

    /// <summary>
    /// トランザクションをロールバックして、connectionを閉じる。閉じる前にロールバックすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public static void RollbackTransactionAndCloseEnsure(IDbTransaction transaction)
    {
        _DatabaseAccess.RollbackTransactionAndCloseEnsure(transaction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static List<ResponseValue> ExecuteQuery(string strSQL, List<IDbDataParameter>? parameters = null, int commandTimeout = 30)
    {
        return _DatabaseAccess.ExecuteQuery(strSQL, parameters, commandTimeout);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="IsTableFunc">true:テーブルを返すFUNCTION</param>
    /// <returns></returns>
    public static List<ResponseValue> ExecuteQuery(string strSQL, IDictionary<string, WhereParam>? param, bool IsTableFunc = false, int commandTimeout = 30)
    {
        return _DatabaseAccess.ExecuteQuery(strSQL, param, IsTableFunc, commandTimeout);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static int ExecuteNonQuery(string strSQL, IDictionary<string, WhereParam>? param)
    {
        return _DatabaseAccess.ExecuteNonQuery(strSQL, param);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="IsTableFunc"></param>
    /// <returns></returns>
    public static List<IDbDataParameter>? CreateDbDataParamList(IDictionary<string, WhereParam>? param, bool IsTableFunc = false)
    {
        return _DatabaseAccess.CreateDbDataParamList(param, IsTableFunc);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static string? GetScalarValueNonQuery(string strSQL, IDictionary<string, object?> param)
    {
        return _DatabaseAccess.GetScalarValueNonQuery(strSQL, param);

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    public static string? GetScalarValueNonQuery(IDbConnection connection, string strSQL, IDictionary<string, object?> param, SqlTransaction? tran = null)
    {
        return _DatabaseAccess.GetScalarValueNonQuery(connection, strSQL, param, tran);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, IDictionary<string, object?> param) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, IDictionary<string, object?> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(connection, strSQL, param, tran);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDataParameter> param) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDataParameter> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(connection, strSQL, param, tran);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDbDataParameter> param) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDbDataParameter> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        return _DatabaseAccess.GetEntityCollection<TDto>(connection, strSQL, param, tran);
    }
    ///// <summary>
    ///// テーブル名とparamを指定してEntityを取得する
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="targetTableName"></param>
    ///// <param name="param"></param>
    ///// <returns></returns>
    //public static IEnumerable<TDto> GetEntityCollection<TDto>(string targetTableName, object param) where TDto : class
    //{

    //    //using IDbConnection connection = new(GetConnectString());
    //    using IDbConnection connection = _DatabaseAccess.GetNewConnection();
    //    // 条件付きでデータをクエリして取得
    //    IEnumerable<TDto> data = connection.Query<TDto>(targetTableName, param);//Todo 2024/05/31時点でRepoDBはOracle非対応のためインターフェースに作る想定。

    //    return data;
    //}
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static object ExecuteStoredFunction(string functionName, IDictionary<string, object?> param, int? timeout = null)
    {
        return _DatabaseAccess.ExecuteStoredFunction(functionName, param, timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static object ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        return _DatabaseAccess.ExecuteStoredFunction(connection, functionName, param, tran);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// param内にoutputパラメータとしてretCode,retMsgを追加する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(string functionName, IDictionary<string, object?> param, int? timeout = null)
    {
        return _DatabaseAccess.ExecuteStoredFunctionReturnOretAndOmsg(functionName, param, timeout: timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// param内にoutputパラメータとしてretCode,retMsgを追加する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(
        IDbConnection connection
        , string functionName
        , IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        return _DatabaseAccess.ExecuteStoredFunctionReturnOretAndOmsg(connection, functionName, param, tran);
    }

    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// outputパラメータを取り出せるようにparamはrefで受ける。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static object ExecuteStoredFunction(string functionName, ref IDictionary<string, object?> param, int? timeout = null)
    {
        return _DatabaseAccess.ExecuteStoredFunction(functionName, ref param, timeout: timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// outputパラメータを取り出せるようにparamはrefで受ける。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static object ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , ref IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        return _DatabaseAccess.ExecuteStoredFunction(connection, functionName, ref param, timeout: timeout);
    }

    /// <summary>
    /// Dbより取得したデータ型の文字列からDbType列挙型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public static DbType GetDbType(string typeString)
    {
        return _DatabaseAccess.GetDbType(typeString);
    }

    /// <summary>
    /// Dbより取得したデータ型の文字列から.net Typeに変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public static Type GetType(string typeString)
    {
        return _DatabaseAccess.GetType(typeString);
    }
    /// <summary>
    /// DbTypeも文字列をもとに文字列のデータを.net の値に変換する
    /// </summary>
    /// <param name="dbtypeString"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static object ConvertDbTypeVal(string dbtypeString, string val)
    {
        return _DatabaseAccess.ConvertDbTypeVal(dbtypeString, val);
    }

    /// <summary>
    /// パラメータ作成
    /// パラメータ名はPrefix文字を付加していない名称を引数として指定すること。
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public static IDataParameter CreateParameter(string parameterName, object parameterValue)
    {
        return _DatabaseAccess.CreateParameter(parameterName, parameterValue);
    }
    public static IDataParameter CreateParameter(string parameterName, object parameterValue, Type type)
    {
        return _DatabaseAccess.CreateParameter(parameterName, parameterValue, type);
    }
    public static IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size)
    {
        return _DatabaseAccess.CreateParameter(parameterName, parameterValue, type, size);
    }
    public static IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size, ParameterDirection direction)
    {
        return _DatabaseAccess.CreateParameter(parameterName, parameterValue, type, size, direction);
    }
    /// <summary>
    /// TableFunctionに対してのSELECTを行うクエリの作成処理
    /// param名はプレフィックスをつけない名称を引数で指定すること
    /// </summary>
    /// <param name="procedureName"></param>
    /// <param name="paramName"></param>
    /// <param name="orderParamName"></param>
    /// <returns></returns>
    public static string CreateTableFunctionQuery(string procedureName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null)
    {
        return _DatabaseAccess.CreateTableFunctionQuery(procedureName, paramName, orderParamName);
    }
    public static string CreateSimpleSelectQuery(string tableName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null)
    {
        return _DatabaseAccess.CreateSimpleSelectQuery(tableName, paramName, orderParamName);
    }
    /// <summary>
    /// パラメータのListを作成して返す
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static List<IDataParameter> CreateParamList(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param)
    {
        return _DatabaseAccess.CreateParamList(param);
    }
    /// <summary>
    /// パラメータのDictionalyを作成して返す。
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static IDictionary<string, object?> CreateParamTable(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param)
    {
        return _DatabaseAccess.CreateParamTable(param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction">OR/AND</param>
    /// <returns></returns>
    public static string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction)
    {
        return _DatabaseAccess.CreateWhereSQLString(colName, whereType, i, Conjuction);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction">OR/AND</param>
    /// <returns></returns>
    public static string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction, string ScalarFunctionName)
    {
        return _DatabaseAccess.CreateWhereSQLString(colName, whereType, i, Conjuction, ScalarFunctionName);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction">OR/AND</param>
    /// <returns></returns>
    public static string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction)
    {
        return _DatabaseAccess.CreateWhereSQLString(colName, whereType, i, j, Conjuction);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction">OR/AND</param>
    /// <returns></returns>
    public static string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction, string ScalarFunctionName)
    {
        return _DatabaseAccess.CreateWhereSQLString(colName, whereType, i, j, Conjuction, ScalarFunctionName);
    }

    /// <summary>
    /// hint句取得
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    public static string GetStringQueryHints(ClassNameSelect selectInfo)
    {
        return _DatabaseAccess.GetStringQueryHints(selectInfo);
    }
    /// <summary>
    /// サイズ指定が必要なDbTypeかどうかを判定する。主に文字列型の場合にtrueとなる。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public static bool IsSizeSpecNeeded(string dbTypeString)
    {
        return _DatabaseAccess.IsSizeSpecNeeded(dbTypeString);
    }
    /// <summary>
    /// DB_FUNCの引数がテーブル型の引数かどうかを判定する。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public static bool IsTableArgument(string dbTypeString)
    {
        return _DatabaseAccess.IsTableArgument(dbTypeString);
    }
}
