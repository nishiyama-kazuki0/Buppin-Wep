using Anotar.Serilog;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using SharedModels;
using System.Collections.Frozen;
using System.Data;
using System.Text;

namespace ExpressionDBWebAPI.Common;

/// <summary>
/// Oracle用機能実装クラス
/// </summary>
internal class OracleDataSource : IDataSource
{
    public readonly string ParamPrefixStr = ":";
    private const char quotationMarkStart = '"';
    private const char quotationMarkEnd = '"';
    public FrozenDictionary<Type, OracleDbType> typeMap = new Dictionary<Type, OracleDbType>()
    {
        { typeof(byte), OracleDbType.Byte },
        { typeof(sbyte), OracleDbType.Int16 },
        { typeof(short), OracleDbType.Int16 },
        { typeof(ushort), OracleDbType.Int32 },
        { typeof(int), OracleDbType.Int32 },
        { typeof(uint), OracleDbType.Int64 },
        { typeof(long), OracleDbType.Int64 },
        { typeof(ulong), OracleDbType.Decimal },
        { typeof(float), OracleDbType.Single },
        { typeof(double), OracleDbType.Double },
        { typeof(decimal), OracleDbType.Decimal },
        { typeof(bool), OracleDbType.Byte }, // Oracle doesn't have a boolean type, so Byte is often used
        { typeof(string), OracleDbType.Varchar2 },
        { typeof(char), OracleDbType.Char },
        { typeof(Guid), OracleDbType.Raw }, // Oracle doesn't have a GUID type, Raw is used with a specific length
        { typeof(DateTime), OracleDbType.Date },
        { typeof(DateTimeOffset), OracleDbType.TimeStampTZ },
        { typeof(byte[]), OracleDbType.Blob },
        { typeof(byte?), OracleDbType.Byte },
        { typeof(sbyte?), OracleDbType.Int16 },
        { typeof(short?), OracleDbType.Int16 },
        { typeof(ushort?), OracleDbType.Int32 },
        { typeof(int?), OracleDbType.Int32 },
        { typeof(uint?), OracleDbType.Int64 },
        { typeof(long?), OracleDbType.Int64 },
        { typeof(ulong?), OracleDbType.Decimal },
        { typeof(float?), OracleDbType.Single },
        { typeof(double?), OracleDbType.Double },
        { typeof(decimal?), OracleDbType.Decimal },
        { typeof(bool?), OracleDbType.Byte },
        { typeof(char?), OracleDbType.Char },
        { typeof(Guid?), OracleDbType.Raw },
        { typeof(DateTime?), OracleDbType.Date },
        { typeof(DateTimeOffset?), OracleDbType.TimeStampTZ },
        { typeof(DataTable), OracleDbType.Array } //DataTable型の場合はOracleUdt[]をVlaueにセットする処理が必要のため追加
    }.ToFrozenDictionary();
    public string AddQuotationMarks(string str)
    {
        return $"{quotationMarkStart}{str}{quotationMarkEnd}";
    }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OracleDataSource()
    {
    }
    /// <summary>
    /// DB接続文字列の取得
    /// </summary>
    /// <returns></returns>
    public string GetConnectString()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
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
        return new OracleConnection(connString);
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
            // Oracleに接続する
            using OracleConnection connection = (OracleConnection)GetNewConnection();
            using OracleCommand command = new(strSQL, connection);
            command.CommandTimeout = commandTimeout;
            OpenConnectionEnsure(connection);

            // Parametersの設定
            if (null != parameters)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            // SQL実行
            using OracleDataReader reader = command.ExecuteReader();

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
            using OracleConnection connection = (OracleConnection)GetNewConnection();
            using OracleCommand command = new(strSQL, connection);

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
    /// <exception cref="NotImplementedException"></exception>
    public List<IDbDataParameter>? CreateDbDataParamList(IDictionary<string, WhereParam>? param, bool IsTableFunc = false)
    {
        //TODO Oracle用に調整必要
        // Parametersの設定
        if (param is null)
        {
            return null;
        }
        List<IDbDataParameter> lstParam = [];
        int i = 0;
        foreach (KeyValuePair<string, WhereParam> pam in param)
        {
            OracleParameter parameter;
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
        //※メモリを大量に消費する場合はforeachを使う
        //DynamicParametersに変換する
        DynamicParameters parameterList = param.Values.OfType<IDbDataParameter>()
            .Aggregate(new DynamicParameters(), (dp, p) => { dp.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size); return dp; });
        return connection.Query<string>(strSQL, parameterList, transaction: tran).FirstOrDefault();
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

        //※メモリを大量に消費する場合はforeachを使う
        //DynamicParametersに変換する
        DynamicParameters parameterList = param.Values.OfType<IDbDataParameter>()
            .Aggregate(new DynamicParameters(), (dp, p) => { dp.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size); return dp; });

        return connection.Query<TDto>(strSQL, parameterList, transaction: tran);
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
        return connection.Query<TDto>(strSQL, param, transaction: tran);
    }
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="connection"></param>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDbDataParameter> param, IDbTransaction? tran = null) where TDto : class, new()
    {
        //DynamicParametersに変換する
        DynamicParameters parameterList = new();
        param.ForEach(p => parameterList.Add(p.ParameterName, p.Value));

        OpenConnectionEnsure(connection);
        return connection.Query<TDto>(strSQL, parameterList, transaction: tran);
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
    public object? ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        OpenConnectionEnsure(connection);
        //DynamicParametersに変換する
        DynamicParameters parameterList = param.Values.OfType<IDbDataParameter>()
            .Aggregate(new DynamicParameters(), (dp, p) => { dp.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size); return dp; });
        //repoDBを使用してfunctionNameのストアドを実行する
        _
            = connection.Execute(
                $"{functionName}"
                , parameterList
                , transaction: tran
                , commandTimeout: timeout
                , CommandType.StoredProcedure
                );
        //object? ret = result.FirstOrDefault();
        return null; //TODO
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
        param.Add("O_RET", new OracleParameter($"{ParamPrefixStr}O_RET", OracleDbType.Int32) { Direction = ParameterDirection.Output });
        param.Add("O_MSG", new OracleParameter($"{ParamPrefixStr}O_MSG", OracleDbType.Varchar2, 5120) { Direction = ParameterDirection.Output });
        OpenConnectionEnsure(connection);
        LogTo.Information($"ExecuteQuery_Start:[dbo].[{functionName}]");
        //DynamicParametersに変換する
        DynamicParameters parameterList = param.Values.OfType<IDbDataParameter>()
            .Aggregate(new DynamicParameters(), (dp, p) => { dp.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size); return dp; });
        //repoDBを使用してfunctionNameのストアドを実行する
        _ = connection.Execute(
            $"{functionName}"
            , parameterList
            , transaction: tran
            , commandTimeout: timeout
            , CommandType.StoredProcedure
            );
        //TODO 実行戻りの扱い
        //_ = result.FirstOrDefault()?.Data;
        LogTo.Information($"ExecuteQuery_Completed:[dbo].[{functionName}]");
        int objRetCode = parameterList.Get<int>("O_RET");//ストアド側で必ずセットする前提とする
        object? objRetMsg = parameterList.Get<object>("O_MSG");
        LogTo.Information($"ExecuteQuery_returnObj Casted:[dbo].[{functionName}]");

        return (
            objRetCode
            , objRetMsg == DBNull.Value || objRetMsg is null ? string.Empty : (string)objRetMsg
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
    public object? ExecuteStoredFunction(
        IDbConnection connection
        , string functionName
        , ref IDictionary<string, object?> param
        , IDbTransaction? tran = null
        , int? timeout = null
        )
    {
        OpenConnectionEnsure(connection);
        //TODO 戻りなど、確認する必要あり
        //DynamicParametersに変換する
        DynamicParameters parameterList = param.Values.OfType<IDbDataParameter>()
            .Aggregate(new DynamicParameters(), (dp, p) => { dp.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size); return dp; });
        _
            = connection.Execute(
                $"{functionName}"
                , parameterList
                , transaction: tran
                , commandTimeout: timeout
                , CommandType.StoredProcedure
                );
        //object? ret = result.FirstOrDefault();
        return null;//TODO
    }

    /// <summary>
    /// Oracleより取得したデータ型の文字列からOracleDbType列挙型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    private OracleDbType GetOracleDbType(string typeString)
    {
        OracleDbType dbType = typeString.ToLower() switch
        {
            "blob" => OracleDbType.Blob,
            "char" => OracleDbType.Char,
            "clob" => OracleDbType.Clob,
            "date" => OracleDbType.Date,
            "float" => OracleDbType.Double,
            "integer" => OracleDbType.Int32,
            "long" => OracleDbType.Long,
            "nclob" => OracleDbType.NClob,
            "number" => OracleDbType.Decimal,
            "nvarchar2" => OracleDbType.NVarchar2,
            "raw" => OracleDbType.Raw,
            "timestamp" => OracleDbType.TimeStamp,
            "timestamp with local time zone" => OracleDbType.TimeStampLTZ,
            "timestamp with time zone" => OracleDbType.TimeStampTZ,
            "varchar2" => OracleDbType.Varchar2,
            "xmltype" => OracleDbType.XmlType,
            "table" => OracleDbType.Array,
            "structured" => OracleDbType.Array,//structuredは特別扱いしたいので、Arrayとして扱う
            _ => OracleDbType.NVarchar2
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
    /// Oracleより取得したデータ型の文字列から.net Typeを得る
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public Type GetType(string dbtypeString)
    {
        Type type = dbtypeString.ToLower() switch
        {
            "blob" => typeof(byte[]),
            "char" => typeof(string),
            "clob" => typeof(string),
            "date" => typeof(DateTime),
            "float" => typeof(double),
            "integer" => typeof(int),
            "long" => typeof(string),
            "nclob" => typeof(string),
            "number" => typeof(decimal),
            "nvarchar2" => typeof(string),
            "raw" => typeof(byte[]),
            "timestamp" => typeof(DateTime),
            "timestamp with local time zone" => typeof(DateTime),
            "timestamp with time zone" => typeof(DateTimeOffset),
            "varchar2" => typeof(string),
            "xmltype" => typeof(string),
            _ => typeof(string)
        };
        return type;
    }

    /// <summary>
    /// OracleDbType列挙型から文字列型からデータ型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public object ConvertDbTypeVal(string dbtypeString, string val)
    {
        object ret;
        OracleDbType s = GetOracleDbType(dbtypeString);
        switch (s)
        {
            case OracleDbType.Int16:
            case OracleDbType.Int32:
            case OracleDbType.Int64:
            case OracleDbType.Decimal:
            case OracleDbType.Double:
            case OracleDbType.Single:
                if (decimal.TryParse(val, out decimal dec))
                {
                    ret = dec;
                }
                else
                {
                    ret = 0; // デフォルト値を設定
                }
                break;

            case OracleDbType.Date:
            case OracleDbType.TimeStamp:
            case OracleDbType.TimeStampLTZ:
            case OracleDbType.TimeStampTZ:
                if (DateTime.TryParse(val, out DateTime dt))
                {
                    ret = dt;
                }
                else
                {
                    ret = DateTime.MinValue; // デフォルト値を設定
                }
                break;

            case OracleDbType.Blob:
            case OracleDbType.Clob:
            case OracleDbType.NClob:
            case OracleDbType.BFile:
            case OracleDbType.Raw:
                ret = Convert.FromBase64String(val); // バイナリデータを扱う場合
                break;

            default:
                ret = val;
                break;
        }
        return ret;
    }

    /// <summary>
    /// パラメータ作成
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public IDataParameter CreateParameter(string parameterName, object parameterValue)
    {
        return new OracleParameter($"{ParamPrefixStr}{parameterName}", parameterValue);
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type)
    {
        //TODO typemapで.net DataTable型の場合はOracleUdt[]をVlaueにセットする処理が必要ではないか。
        //TODO DapperのDynamicparametersがうまく変換してくれるなら不要だが、要動作確認。
        //DataTableの場合はここを通るため、OracleUdtに変換してValueにセットする
        OracleDbType dbtype = typeMap[type];
        if (dbtype == OracleDbType.Array
            && type == typeof(DataTable)
            && parameterValue is DataTable dt)
        {
            List<OracleUdt> udtList = [];
            foreach (DataRow row in dt.Rows)
            {
                OracleUdt ou = new();
                //dtの内容をすべてOracleUdtに変換してリストに追加
                foreach (DataColumn column in dt.Columns)
                {
                    //TODO 途中
                    //ou["COLUMN1"] = row["Column1"].ToString();
                    //ou["COLUMN2"] = Convert.ToInt32(row["Column2"]);
                }

                udtList.Add(ou);
            }
            parameterValue = udtList.ToArray();
        }
        return new OracleParameter($"{ParamPrefixStr}{parameterName}", dbtype)
        {
            Value = parameterValue,
        };
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size)
    {
        return new OracleParameter($"{ParamPrefixStr}{parameterName}", typeMap[type], size)
        {
            Value = parameterValue,
        };
    }
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size, ParameterDirection direction)
    {
        return new OracleParameter($"{ParamPrefixStr}{parameterName}", typeMap[type], size)
        {
            Value = parameterValue,
            Direction = direction,
        };
    }
    /// <summary>
    /// TableFunctionに対してのSELECTを行うクエリの作成処理
    /// param名はプレフィックスをつけない名称を引数で指定すること
    /// </summary>
    /// <param name="procedureName"></param>
    /// <param name="paramName"></param>
    /// <param name="orderParamName"></param>
    /// <returns></returns>
    public string CreateTableFunctionQuery(string procedureName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null)
    {
        StringBuilder sb = new();
        _ = sb.Append($"SELECT * FROM TABLE({procedureName}(");
        for (int i = 0; i < paramName.Length; i++)
        {
            _ = sb.Append($"{(i == 0 ? " " : ", ")}{ParamPrefixStr}{paramName[i]}");
        }
        _ = sb.Append(")) ");
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
                : (object)(p.Value.type is not null && p.Value.size is null
                    ? CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type)
                    : p.Value.type is not null && p.Value.size is not null
                                        ? CreateParameter(p.Key, p.Value.value ?? DBNull.Value, p.Value.type, (int)p.Value.size)
                                        : CreateParameter(p.Key, p.Value.value ?? DBNull.Value));
        }
        return ret;
    }
    /// <summary>
    /// WHERE句文字列の作成
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction)
    {
        return $"  {(i == 0 ? "  " : Conjuction)} {AddQuotationMarks(colName)} {whereType} {ParamPrefixStr}param{i}";
    }
    /// <summary>
    /// WHERE句文字列の作成
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction, string ScalarFunctionName)
    {
        return $"  {(i == 0 ? "  " : Conjuction)} {ScalarFunctionName}({AddQuotationMarks(colName)}) {whereType} {ParamPrefixStr}param{i}"
         ;
    }
    /// <summary>
    /// WHERE句文字列の作成
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
    /// WHERE句文字列の作成
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
        return $"  {(j == 0 ? "  " : Conjuction)} {ScalarFunctionName}({AddQuotationMarks(colName)}) {whereType} {ParamPrefixStr}param{i}_{j}"
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
        return hints;
    }
    /// <summary>
    /// サイズ指定が必要なDbTypeかどうかを判定する。主に文字列型の場合にtrueとなる。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsSizeSpecNeeded(string dbTypeString)
    {
        OracleDbType s = GetOracleDbType(dbTypeString);
        return s is OracleDbType.Char
            or OracleDbType.Varchar2
            or OracleDbType.NVarchar2
            or OracleDbType.NChar
            or OracleDbType.Raw;
    }
    /// <summary>
    /// DB_FUNCの引数がテーブル型の引数かどうかを判定する。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsTableArgument(string dbTypeString)
    {
        return GetOracleDbType(dbTypeString) == OracleDbType.Array;
    }


}
