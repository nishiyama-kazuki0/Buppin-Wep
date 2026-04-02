using Anotar.Serilog;
using Microsoft.Data.SqlClient;
using RepoDb;
using SharedModels;
using System.Collections.Frozen;
using System.Data;
using System.Text;

namespace ExpressionDBWebAPI.Common;

/// <summary>
/// SQLServer用機能実装クラス
/// </summary>
internal class SqlServerSource : IDataSource
{
    public readonly string ParamPrefixStr = "@";
    private const char quotationMarkStart = '[';
    private const char quotationMarkEnd = ']';
    private readonly FrozenDictionary<Type, SqlDbType> typeMap = new Dictionary<Type, SqlDbType>()
    {
        { typeof(byte), SqlDbType.TinyInt },
        { typeof(sbyte), SqlDbType.SmallInt },
        { typeof(short), SqlDbType.SmallInt },
        { typeof(ushort), SqlDbType.Int },
        { typeof(int), SqlDbType.Int },
        { typeof(uint), SqlDbType.BigInt },
        { typeof(long), SqlDbType.BigInt },
        { typeof(ulong), SqlDbType.Decimal },
        { typeof(float), SqlDbType.Real },
        { typeof(double), SqlDbType.Float },
        { typeof(decimal), SqlDbType.Decimal },
        { typeof(bool), SqlDbType.Bit },
        { typeof(string), SqlDbType.NVarChar },
        { typeof(char), SqlDbType.NChar },
        { typeof(Guid), SqlDbType.UniqueIdentifier },
        { typeof(DateTime), SqlDbType.DateTime },
        { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
        { typeof(byte[]), SqlDbType.VarBinary },
        { typeof(byte?), SqlDbType.TinyInt },
        { typeof(sbyte?), SqlDbType.SmallInt },
        { typeof(short?), SqlDbType.SmallInt },
        { typeof(ushort?), SqlDbType.Int },
        { typeof(int?), SqlDbType.Int },
        { typeof(uint?), SqlDbType.BigInt },
        { typeof(long?), SqlDbType.BigInt },
        { typeof(ulong?), SqlDbType.Decimal },
        { typeof(float?), SqlDbType.Real },
        { typeof(double?), SqlDbType.Float },
        { typeof(decimal?), SqlDbType.Decimal },
        { typeof(bool?), SqlDbType.Bit },
        { typeof(char?), SqlDbType.NChar },
        { typeof(Guid?), SqlDbType.UniqueIdentifier },
        { typeof(DateTime?), SqlDbType.DateTime },
        { typeof(DateTimeOffset?), SqlDbType.DateTimeOffset },
        { typeof(DataTable), SqlDbType.Structured }
    }.ToFrozenDictionary();
    public string AddQuotationMarks(string str)
    {
        return $"{quotationMarkStart}{str}{quotationMarkEnd}";
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SqlServerSource()
    {
        //初期化１回だけ
        _ = GlobalConfiguration
            .Setup()
            .UseSqlServer();
    }

    /// <summary>
    /// DB接続文字列の取得
    /// </summary>
    /// <returns></returns>
    public string GetConnectString()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();//TODO コンフィグはinjectしたサービスから取得する
        string? connString = config.GetConnectionString("DefaultConnection");

        return connString!;
    }

    /// <summary>
    /// 新しいDBConnectionを取得する。
    /// 主に本クラス外部でもコネクションを使用する場合に使用する。外側の変数は原則、usingで受けること。
    /// 例.トランザクションを外側で張りたいときなど
    /// </summary>
    /// <returns></returns>
    public IDbConnection GetNewConnection()
    {
        string connString = GetConnectString();
        return new SqlConnection(connString);
    }

    /// <summary>
    /// connectionを参照引数で受けて、OpenしてなかったらOpenする
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public void OpenConnectionEnsure(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    /// <summary>
    /// 引数で受けたconnectionのトランザクションを開始する。開始後にトランザクションオブジェクトを返す。
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public IDbTransaction BeginTransactionAndOpenEnsure(IDbConnection connection)
    {
        OpenConnectionEnsure(connection);
        return connection.BeginTransaction();
    }

    /// <summary>
    /// トランザクションをコミットして、connectionを閉じる。閉じる前にコミットすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public void CommitTransactionAndCloseEnsure(IDbTransaction transaction)
    {
        transaction.Commit();
        transaction.Connection?.Close();
    }
    /// <summary>
    /// トランザクションをロールバックして、connectionを閉じる。閉じる前にロールバックすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public void RollbackTransactionAndCloseEnsure(IDbTransaction transaction)
    {
        transaction.Rollback();
        transaction.Connection?.Close();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public List<ResponseValue> ExecuteQuery(string strSQL, List<IDbDataParameter>? parameters = null, int commandTimeout = 30)
    {
        List<ResponseValue> res = [];
        try
        {
            // SQLServerに接続する
            using SqlConnection connection = (SqlConnection)GetNewConnection();
            using SqlCommand command = new(strSQL, connection);
            command.CommandTimeout = commandTimeout;
            OpenConnectionEnsure(connection);

            // Parametersの設定
            if (null != parameters)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            // SQL実行
            using SqlDataReader reader = command.ExecuteReader();

            // 取得したデータを列名をキーにしてDictionaryの配列を作成する
            while (reader.Read())
            {
                ResponseValue todoItem = new()
                {
                    Columns = [],
                    Values = new Dictionary<string, object>()
                };
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    todoItem.Columns.Add(key);
                    if (reader.IsDBNull(i))
                    {
                        todoItem.Values.Add(key, string.Empty);
                    }
                    else
                    {
                        object val = reader.GetValue(i);
                        todoItem.Values.Add(key, val);
                    }
                }
                res.Add(todoItem);
            }
        }
        catch (Exception e)
        {

            LogTo.Fatal(e.Message);
        }
        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="IsTableFunc">true:テーブルを返すFUNCTION</param>
    /// <returns></returns>
    public List<ResponseValue> ExecuteQuery(string strSQL, IDictionary<string, WhereParam>? param, bool IsTableFunc = false, int commandTimeout = 30)
    {
        List<ResponseValue> res = [];
        try
        {
            List<IDbDataParameter>? lstParam = CreateDbDataParamList(param, IsTableFunc);
            res = ExecuteQuery(strSQL, lstParam, commandTimeout);
        }
        catch (Exception e)
        {
            LogTo.Fatal(e.Message);
        }
        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public int ExecuteNonQuery(string strSQL, IDictionary<string, WhereParam>? param)
    {
        int res = -1;
        try
        {
            // SQLServerに接続する
            using SqlConnection connection = (SqlConnection)GetNewConnection();
            using SqlCommand command = new(strSQL, connection);

            OpenConnectionEnsure(connection);

            //// Parametersの設定
            //if (null != param)
            //{
            //    foreach (KeyValuePair<string, WhereParam> pam in param)
            //    {
            //        string field = string.IsNullOrEmpty(pam.Value.field) ? pam.Key : pam.Value.field;
            //        SqlParameter parameter = command.Parameters.Add("@" + field, columns.DataType);
            //        parameter.Value = pam.Value.val;
            //    }
            //}

            // SQL実行
            res = command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            LogTo.Fatal(e.Message);
        }
        return res;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="IsTableFunc"></param>
    /// <returns></returns>
    public List<IDbDataParameter>? CreateDbDataParamList(IDictionary<string, WhereParam>? param, bool IsTableFunc = false)
    {
        // Parametersの設定
        if (param is null)
        {
            return null;
        }
        List<IDbDataParameter> lstParam = [];
        int i = 0;
        foreach (KeyValuePair<string, WhereParam> pam in param)
        {
            SqlParameter parameter;
            if (IsTableFunc)
            {
                if (pam.Value.tableFuncWithWhere)
                {
                    if (pam.Value.orLinking)
                    {
                        // OR連結
                        for (int j = 0; j < pam.Value.linkingVals.Count; j++)
                        {
                            parameter = new($"{ParamPrefixStr}param{i}_{j}", ConvertDbTypeVal(pam.Value.dbTypeString, pam.Value.linkingVals[j]))
                            {
                                Size = IsSizeSpecNeeded(pam.Value.dbTypeString) && pam.Value.size is not null ? (int)pam.Value.size : 0
                            };
                            lstParam.Add(parameter);
                        }
                    }
                    else
                    {
                        parameter = new($"{ParamPrefixStr}param{i}", ConvertDbTypeVal(pam.Value.dbTypeString, pam.Value.val))
                        {
                            Size = IsSizeSpecNeeded(pam.Value.dbTypeString) && pam.Value.size is not null ? (int)pam.Value.size : 0
                        };
                        lstParam.Add(parameter);
                    }
                }
                else
                {
                    parameter = new($"{ParamPrefixStr}{pam.Key}", ConvertDbTypeVal(pam.Value.dbTypeString, pam.Value.val))
                    {
                        Size = IsSizeSpecNeeded(pam.Value.dbTypeString) && pam.Value.size is not null ? (int)pam.Value.size : 0
                    };
                    lstParam.Add(parameter);
                }
            }
            else
            {
                if (pam.Value.orLinking)
                {
                    // OR連結
                    for (int j = 0; j < pam.Value.linkingVals.Count; j++)
                    {
                        parameter = new($"{ParamPrefixStr}param{i}_{j}", ConvertDbTypeVal(pam.Value.dbTypeString, pam.Value.linkingVals[j]))
                        {
                            Size = IsSizeSpecNeeded(pam.Value.dbTypeString) && pam.Value.size is not null ? (int)pam.Value.size : 0
                        };
                        lstParam.Add(parameter);
                    }
                }
                else
                {
                    parameter = new($"{ParamPrefixStr}param{i}", ConvertDbTypeVal(pam.Value.dbTypeString, pam.Value.val))
                    {
                        Size = IsSizeSpecNeeded(pam.Value.dbTypeString) && pam.Value.size is not null ? (int)pam.Value.size : 0
                    };
                    lstParam.Add(parameter);
                }
            }
            i++;
        }

        return lstParam;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public string? GetScalarValueNonQuery(string strSQL, IDictionary<string, object?> param)
    {
        using IDbConnection connection = GetNewConnection();
        return GetScalarValueNonQuery(connection, strSQL, param);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    public string? GetScalarValueNonQuery(IDbConnection connection, string strSQL, IDictionary<string, object?> param, IDbTransaction? tran = null)
    {
        OpenConnectionEnsure(connection);
        return connection.ExecuteQuery<string>(strSQL, param, transaction: tran).FirstOrDefault();
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, IDictionary<string, object?> param) where TDto : class, new()
    {
        using IDbConnection connection = GetNewConnection();
        return GetEntityCollection<TDto>(connection, strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, IDictionary<string, object?> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        //以下使い方例
        //var parameters = new Dictionary<string, string>
        //{
        //    { "key1", "value1" },
        //    { "key2", "value2" }
        //};
        //var results = connection.Query<T>("SELECT * FROM table WHERE column1 = @key1 AND column2 = @key2", parameters);

        OpenConnectionEnsure(connection);
        return connection.ExecuteQuery<TDto>(strSQL, param, transaction: tran);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDataParameter> param) where TDto : class, new()
    {
        using IDbConnection connection = GetNewConnection();
        return GetEntityCollection<TDto>(connection, strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDataParameter> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        //以下使い方例
        //var parameters = new Dictionary<string, string>
        //{
        //    { "key1", "value1" },
        //    { "key2", "value2" }
        //};
        //var results = connection.Query<T>("SELECT * FROM table WHERE column1 = @key1 AND column2 = @key2", parameters);

        OpenConnectionEnsure(connection);
        return connection.ExecuteQuery<TDto>(strSQL, param, transaction: tran);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDbDataParameter> param) where TDto : class, new()
    {
        using IDbConnection connection = GetNewConnection();
        return GetEntityCollection<TDto>(connection, strSQL, param);
    }
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDbDataParameter> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        //以下使い方例
        //var parameters = new Dictionary<string, string>
        //{
        //    { "key1", "value1" },
        //    { "key2", "value2" }
        //};
        //var results = connection.Query<T>("SELECT * FROM table WHERE column1 = @key1 AND column2 = @key2", parameters);

        OpenConnectionEnsure(connection);
        IDictionary<string, object?> paramDictionary
            = param.ToDictionary(param => param.ParameterName, param => param.Value);
        return connection.ExecuteQuery<TDto>(strSQL, paramDictionary, transaction: tran);
    }

    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(string functionName, IDictionary<string, object?> param, int? timeout = null)
    {
        using IDbConnection connection = GetNewConnection();
        return ExecuteStoredFunction(connection, functionName, param, timeout: timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        OpenConnectionEnsure(connection);
        //TODO 戻りなど、確認する必要あり
        //repoDBを使用してfunctionNameのストアドを実行する
        IEnumerable<dynamic> result
            = connection.ExecuteQuery(
                $"[dbo].{AddQuotationMarks(functionName)}"
                , param
                , CommandType.StoredProcedure
                , transaction: tran
                , commandTimeout: timeout
                );
        object? ret = result.FirstOrDefault();
        return ret;
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// param内にoutputパラメータとしてretCode,retMsgを追加する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(string functionName, IDictionary<string, object?> param, int? timeout = null)
    {
        using IDbConnection connection = GetNewConnection();
        return ExecuteStoredFunctionReturnOretAndOmsg(connection, functionName, param, timeout: timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// param内にoutputパラメータとしてretCode,retMsgを追加する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(
        IDbConnection connection
        , string functionName
        , IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        //戻り値のパラメータを追加する。StoredFunctionの定義にはあらかじめ以下2点のOUTPUTパラメータをを定義して置く必要あり
        param.Add("O_RET", new SqlParameter($"{ParamPrefixStr}O_RET", SqlDbType.Int) { Direction = ParameterDirection.Output });
        param.Add("O_MSG", new SqlParameter($"{ParamPrefixStr}O_MSG", SqlDbType.VarChar, 5120) { Direction = ParameterDirection.Output });
        OpenConnectionEnsure(connection);
        LogTo.Information($"ExecuteQuery_Start:[dbo].[{functionName}]");
        //repoDBを使用してfunctionNameのストアドを実行する
        _ = connection.ExecuteQuery(
            $"[dbo].{AddQuotationMarks(functionName)}"
            , param
            , CommandType.StoredProcedure
            , transaction: tran
            , commandTimeout: timeout
            );
        //TODO 実行戻りの扱い
        //_ = result.FirstOrDefault()?.Data;
        LogTo.Information($"ExecuteQuery_Completed:[dbo].[{functionName}]");
        SqlParameter objRetCode = (SqlParameter)(param["O_RET"] ?? throw new NullReferenceException());
        SqlParameter objRetMsg = (SqlParameter)(param["O_MSG"] ?? throw new NullReferenceException());
        LogTo.Information($"ExecuteQuery_returnObj Casted:[dbo].[{functionName}]");

        return (
            (int)objRetCode.Value
            , objRetMsg.Value == DBNull.Value ? string.Empty : (string)objRetMsg.Value
            );
    }

    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// outputパラメータを取り出せるようにparamはrefで受ける。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(string functionName, ref IDictionary<string, object?> param, int? timeout = null)
    {
        using IDbConnection connection = GetNewConnection();
        return ExecuteStoredFunction(connection, functionName, ref param, timeout: timeout);
    }
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// outputパラメータを取り出せるようにparamはrefで受ける。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , ref IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        OpenConnectionEnsure(connection);
        //TODO 戻りなど、確認する必要あり
        //repoDBを使用してfunctionNameのストアドを実行する
        IEnumerable<dynamic> result
            = connection.ExecuteQuery(
                $"[dbo].{AddQuotationMarks(functionName)}"
                , param, CommandType.StoredProcedure
                , transaction: tran
                , commandTimeout: timeout
                );
        object? ret = result.FirstOrDefault();
        return ret;
    }
    /// <summary>
    /// SQLServerより取得したデータ型の文字列からSqlDbType列挙型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    private SqlDbType GetSqlDbType(string typeString)
    {
        SqlDbType dbType = typeString.ToLower() switch
        {
            "bigint" => SqlDbType.BigInt,
            "binary" => SqlDbType.VarBinary,
            "bit" => SqlDbType.Bit,
            "char" => SqlDbType.Char,
            "date" => SqlDbType.Date,
            "datetime" => SqlDbType.DateTime,
            "datetime2" => SqlDbType.DateTime2,
            "datetimeoffset" => SqlDbType.DateTimeOffset,
            "decimal" => SqlDbType.Decimal,
            "float" => SqlDbType.Float,
            "image" => SqlDbType.Binary,
            "int" => SqlDbType.Int,
            "money" => SqlDbType.Money,
            "nchar" => SqlDbType.NChar,
            "ntext" => SqlDbType.NText,
            "numeric" => SqlDbType.Decimal,
            "nvarchar" => SqlDbType.NVarChar,
            "real" => SqlDbType.Real,
            "smalldatetime" => SqlDbType.SmallDateTime,
            "smallint" => SqlDbType.SmallInt,
            "smallmoney" => SqlDbType.SmallMoney,
            "structured" => SqlDbType.Structured, //structuredは特別扱いのため追記。
            "text" => SqlDbType.Text,
            "time" => SqlDbType.Time,
            "timestamp" => SqlDbType.Timestamp,
            "tinyint" => SqlDbType.TinyInt,
            "uniqueidentifier" => SqlDbType.UniqueIdentifier,
            "varbinary" => SqlDbType.VarBinary,
            "varchar" => SqlDbType.VarChar,
            "xml" => SqlDbType.Xml,
            _ => SqlDbType.NVarChar
        };
        return dbType;
    }

    //TODO 不要なら削除。
    public DbType GetDbType(string typeString)
    {
        DbType dbType = typeString.ToLower() switch
        {
            "number" => DbType.Decimal,
            "binary_float" => DbType.Single,
            "binary_double" => DbType.Double,
            "long" => DbType.Int64,
            "varchar2" => DbType.String,
            "nvarchar2" => DbType.String,
            "char" => DbType.String,
            "nchar" => DbType.String,
            "date" => DbType.Date,
            "timestamp" => DbType.DateTime,
            "blob" => DbType.Binary,
            "raw" => DbType.Binary,
            "clob" => DbType.String,
            "nclob" => DbType.String,
            _ => DbType.String
        };
        return dbType;
    }

    /// <summary>
    /// SQLServerより取得したデータ型の文字列から.net Typeを得る
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public Type GetType(string dbtypeString)
    {
        Type type = dbtypeString.ToLower() switch
        {
            "bigint" => typeof(long),
            "binary" => typeof(byte[]),
            "bit" => typeof(bool),
            "char" => typeof(string),
            "date" => typeof(DateTime),
            "datetime" => typeof(DateTime),
            "datetime2" => typeof(DateTime),
            "datetimeoffset" => typeof(DateTimeOffset),
            "decimal" => typeof(decimal),
            "float" => typeof(double),
            "image" => typeof(byte[]),
            "int" => typeof(int),
            "money" => typeof(decimal),
            "nchar" => typeof(string),
            "ntext" => typeof(string),
            "numeric" => typeof(decimal),
            "nvarchar" => typeof(string),
            "real" => typeof(float),
            "smalldatetime" => typeof(DateTime),
            "smallint" => typeof(int),
            "smallmoney" => typeof(decimal),
            "structured" => typeof(string),
            "text" => typeof(string),
            "time" => typeof(TimeSpan),
            "timestamp" => typeof(byte[]),
            "tinyint" => typeof(byte),
            "uniqueidentifier" => typeof(Guid),
            "varbinary" => typeof(byte[]),
            "varchar" => typeof(string),
            "xml" => typeof(string),        // マッピング表ではXml
            _ => typeof(long)
        };
        return type;
    }

    /// <summary>
    /// SqlDbType列挙型から文字列型からデータ型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public object ConvertDbTypeVal(string dbtypeString, string val)
    {
        object ret;
        SqlDbType s = GetSqlDbType(dbtypeString);
        switch (s)
        {
            case SqlDbType.BigInt:
            case SqlDbType.Bit:
            case SqlDbType.Decimal:
            case SqlDbType.Float:
            case SqlDbType.Int:
            case SqlDbType.Money:
            case SqlDbType.SmallInt:
            case SqlDbType.SmallMoney:
            case SqlDbType.TinyInt:
                _ = decimal.TryParse(val, out decimal dec);
                ret = dec;
                break;

            case SqlDbType.Date:
            case SqlDbType.Time:
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
                _ = DateTime.TryParse(val, out DateTime dt);
                ret = dt;
                break;

            default:
                ret = val;
                break;
        }
        return ret;
    }

    public IDataParameter CreateParameter(string parameterName, object parameterValue)
    {
        return new SqlParameter($"{ParamPrefixStr}{parameterName}", parameterValue);
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type)
    {
        return new SqlParameter($"{ParamPrefixStr}{parameterName}", typeMap[type])
        {
            Value = parameterValue,
        };
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size)
    {
        return new SqlParameter($"{ParamPrefixStr}{parameterName}", typeMap[type], size)
        {
            Value = parameterValue,
        };
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size, ParameterDirection direction)
    {
        return new SqlParameter($"{ParamPrefixStr}{parameterName}", typeMap[type], size)
        {
            Value = parameterValue,
            Direction = direction,
        };
    }
    /// <summary>
    /// TableFunctionに対してのSELECTを行うクエリの作成処理
    /// param名はプレフィックスをつけない名称を引数で指定すること
    /// </summary>
    /// <param name="procedureName">テーブルファンクション名</param>
    /// <param name="paramName">引数</param>
    /// <param name="orderParamName">並び変え順</param>
    /// <returns></returns>
    public string CreateTableFunctionQuery(string procedureName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null)
    {
        StringBuilder sb = new();
        _ = sb.Append($"SELECT * FROM {procedureName}(");
        for (int i = 0; i < paramName.Length; i++)
        {
            _ = sb.Append($"{(i == 0 ? " " : ", ")}{ParamPrefixStr}{paramName[i]}");
        }
        _ = sb.Append(") ");
        //TODO Where句の組み立てが必要か要検討。テーブル関数を使用する前提なので、Whereはテーブル関数内で使用するはずなので、とりあえずなし
        if (orderParamName is not null && orderParamName.Length > 0)
        {
            for (int i = 0; i < orderParamName.Length; i++)
            {
                _ = sb.Append($"{(i == 0 ? "ORDER BY " : ", ")}{AddQuotationMarks(orderParamName[i].colName)}{(orderParamName[i].isDesc ? " DESC" : "")}");
            }
        }
        return sb.ToString();
    }
    public string CreateSimpleSelectQuery(string tableName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null)
    {
        StringBuilder sb = new();
        _ = sb.Append($"SELECT * FROM {tableName}");
        for (int i = 0; i < paramName.Length; i++)
        {
            //TODO 一旦 = のみ考慮。他必要な場合は検討
            _ = sb.Append($"{(i == 0 ? " WHERE " : " AND ")}{AddQuotationMarks(paramName[i])} = {ParamPrefixStr}{paramName[i]}");
        }
        if (orderParamName is not null && orderParamName.Length > 0)
        {
            for (int i = 0; i < orderParamName.Length; i++)
            {
                _ = sb.Append($"{(i == 0 ? "ORDER BY " : ", ")}{AddQuotationMarks(orderParamName[i].colName)}{(orderParamName[i].isDesc ? " DESC" : "")}");
            }
        }
        return sb.ToString();
    }
    /// <summary>
    /// パラメータのListを作成して返す
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public List<IDataParameter> CreateParamList(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param)
    {
        List<IDataParameter> ret = [];
        foreach (KeyValuePair<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> p in param)
        {
            if (p.Value.type is not null && p.Value.size is not null && p.Value.parameterDirection is not null)
            {
                ret.Add(CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type, (int)p.Value.size, (ParameterDirection)p.Value.parameterDirection));
            }
            else if (p.Value.type is not null && p.Value.size is null)
            {
                ret.Add(CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type));
            }
            else if (p.Value.type is not null && p.Value.size is not null)
            {
                ret.Add(CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type, (int)p.Value.size));
            }
            else
            {
                ret.Add(CreateParameter(p.Key, p.Value.value ?? DBNull.Value));
            }
        }
        return ret;
    }
    /// <summary>
    /// パラメータのDictionalyを作成して返す。
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public IDictionary<string, object?> CreateParamTable(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param)
    {
        IDictionary<string, object?> ret = new Dictionary<string, object?>();
        foreach (KeyValuePair<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> p in param)
        {
            ret[p.Key] = p.Value.type is not null && p.Value.size is not null && p.Value.parameterDirection is not null
                ? CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type, (int)p.Value.size, (ParameterDirection)p.Value.parameterDirection)
                : p.Value.type is not null && p.Value.size is null
                    ? CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type)
                    : p.Value.type is not null && p.Value.size is not null
                                        ? CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type, (int)p.Value.size)
                                        : CreateParameter(p.Key, p.Value.value ?? DBNull.Value);
        }
        return ret;
    }
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction)
    {
        return
             $"  {(i == 0 ? "  " : Conjuction)} {AddQuotationMarks(colName)} {whereType} {ParamPrefixStr}param{i}";
    }
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction, string ScalarFunctionName)
    {
        return $"  {(i == 0 ? "  " : Conjuction)} dbo.{ScalarFunctionName}({AddQuotationMarks(colName)}) {whereType} {ParamPrefixStr}param{i}"
;
    }
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction)
    {
        return $"  {(j == 0 ? "  " : Conjuction)} {AddQuotationMarks(colName)} {whereType} {ParamPrefixStr}param{i}_{j}";
    }
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction, string ScalarFunctionName)
    {
        return $"  {(j == 0 ? "  " : Conjuction)} dbo.{ScalarFunctionName}({AddQuotationMarks(colName)}) {whereType} {ParamPrefixStr}param{i}_{j}"
           ;
    }
    /// <summary>
    /// Hint句取得
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    public string GetStringQueryHints(ClassNameSelect selectInfo)
    {
        string hints = "";
        if (selectInfo.tsqlHints != EnumTSQLhints.NONE && !selectInfo.tableFuncFlg)
        {
            hints = $" WITH ({selectInfo.GetHintStr}) "; //テーブルヒント句を設定
        }
        return hints;
    }
    /// <summary>
    /// サイズ指定が必要なDbTypeかどうかを判定する。主に文字列型の場合にtrueとなる。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsSizeSpecNeeded(string dbTypeString)
    {
        SqlDbType s = GetSqlDbType(dbTypeString);
        return s is SqlDbType.Char
        or SqlDbType.VarChar
        or SqlDbType.NVarChar
        or SqlDbType.NChar
        or SqlDbType.Binary
        or SqlDbType.VarBinary;
    }
    /// <summary>
    /// DB_FUNCの引数がテーブル型の引数かどうかを判定する。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsTableArgument(string dbTypeString)
    {
        return GetSqlDbType(dbTypeString) == SqlDbType.Structured;
    }

}
