using Anotar.Serilog;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;
using ZennohBlazorShared.Data;

namespace ZennohWebAPI.Common
{
    public class DataSource
    {
        public static readonly string ParamPrefixStr = "@";//TODO コンストラクタでDB種類によって変更する

        /// <summary>
        /// DB接続文字列の取得
        /// </summary>
        /// <returns></returns>
        public static string GetConnectString()
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
        public static SqlConnection GetNewConnection()
        {
            string connString = GetConnectString();
            return new SqlConnection(connString);
        }

        /// <summary>
        /// connectionを参照引数で受けて、OpenしてなかったらOpenする
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void OpenConnectionEnsure(SqlConnection connection)
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
        public static SqlTransaction BeginTransactionAndOpenEnsure(SqlConnection connection)
        {
            OpenConnectionEnsure(connection);
            return connection.BeginTransaction();
        }

        /// <summary>
        /// トランザクションをコミットして、connectionを閉じる。閉じる前にコミットすることを保証する。
        /// </summary>
        /// <param name="transaction"></param>
        public static void CommitTransactionAndCloseEnsure(SqlTransaction transaction)
        {
            transaction.Commit();
            transaction.Connection?.Close();
        }
        /// <summary>
        /// トランザクションをロールバックして、connectionを閉じる。閉じる前にロールバックすることを保証する。
        /// </summary>
        /// <param name="transaction"></param>
        public static void RollbackTransactionAndCloseEnsure(SqlTransaction transaction)
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
        public static List<ResponseValue> ExecuteQuery(string strSQL, List<SqlParameter>? parameters = null)
        {
            List<ResponseValue> res = new();
            try
            {
                // SQLServerに接続する
                string connString = GetConnectString();
                using SqlConnection connection = new(connString);
                using SqlCommand command = new(strSQL, connection);
                command.CommandTimeout = 90;//デフォルトの30秒では短いので90にしておく対応
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
                        Columns = new List<string>(),
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
        public static List<ResponseValue> ExecuteQuery(string strSQL, IDictionary<string, WhereParam>? param, bool IsTableFunc = false)
        {
            List<ResponseValue> res = new();
            try
            {
                List<SqlParameter> lstParam = new();
                // Parametersの設定
                if (null != param)
                {
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
                                        parameter = new($"{ParamPrefixStr}param{i}_{j}", GetDbTypeVal((SqlDbType)pam.Value.type, pam.Value.linkingVals[j]))
                                        {
                                            Size = (pam.Value.isSizeSpecNeeded && pam.Value.size is not null) ? (int)pam.Value.size : 0
                                        };
                                        lstParam.Add(parameter);
                                    }
                                }
                                else
                                {
                                    parameter = new($"{ParamPrefixStr}param{i}", GetDbTypeVal((SqlDbType)pam.Value.type, pam.Value.val))
                                    {
                                        Size = (pam.Value.isSizeSpecNeeded && pam.Value.size is not null) ? (int)pam.Value.size : 0
                                    };
                                    lstParam.Add(parameter);
                                }
                            }
                            else
                            {
                                parameter = new($"{ParamPrefixStr}{pam.Key}", GetDbTypeVal((SqlDbType)pam.Value.type, pam.Value.val))
                                {
                                    Size = (pam.Value.isSizeSpecNeeded && pam.Value.size is not null) ? (int)pam.Value.size : 0
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
                                    parameter = new($"{ParamPrefixStr}param{i}_{j}", GetDbTypeVal((SqlDbType)pam.Value.type, pam.Value.linkingVals[j]))
                                    {
                                        Size = (pam.Value.isSizeSpecNeeded && pam.Value.size is not null) ? (int)pam.Value.size : 0
                                    };
                                    lstParam.Add(parameter);
                                }
                            }
                            else
                            {
                                parameter = new($"{ParamPrefixStr}param{i}", GetDbTypeVal((SqlDbType)pam.Value.type, pam.Value.val))
                                {
                                    Size = (pam.Value.isSizeSpecNeeded && pam.Value.size is not null) ? (int)pam.Value.size : 0
                                };
                                lstParam.Add(parameter);
                            }
                        }
                        i++;
                    }
                }
                res = ExecuteQuery(strSQL, lstParam);
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
        public static int ExecuteNonQuery(string strSQL, IDictionary<string, WhereParam>? param)
        {
            int res = -1;
            try
            {
                // SQLServerに接続する
                string connString = GetConnectString();
                using SqlConnection connection = new(connString);
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
        /// <param name="strSQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string? GetScalarValueNonQuery(string strSQL, IDictionary<string, object?> param)
        {
            using SqlConnection connection = GetNewConnection();
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
        public static string? GetScalarValueNonQuery(SqlConnection connection, string strSQL, IDictionary<string, object?> param, SqlTransaction? tran = null)
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
        public static IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, IDictionary<string, object?> param) where TDto : class, new()
        {
            using SqlConnection connection = GetNewConnection();
            return GetEntityCollection<TDto>(connection, strSQL, param);
        }
        /// <summary>
        /// SQL文とparamを指定してEntityを取得する
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="strSQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IEnumerable<TDto> GetEntityCollection<TDto>(SqlConnection connection, string strSQL, IDictionary<string, object?> param, SqlTransaction? tran = null) where TDto : class, new()
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
        /// ストアドファンクション/プロシージャを実行する
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteStoredFunction(string functionName, IDictionary<string, object?> param, int? timeout = null)
        {
            using SqlConnection connection = GetNewConnection();
            return ExecuteStoredFunction(connection, functionName, param, timeout: timeout);
        }
        /// <summary>
        /// ストアドファンクション/プロシージャを実行する
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteStoredFunction(
            SqlConnection connection
            , string functionName
            , IDictionary<string, object?> param
            , SqlTransaction? tran = null
            , int? timeout = null
            )
        {
            OpenConnectionEnsure(connection);
            //TODO 戻りなど、確認する必要あり
            //repoDBを使用してfunctionNameのストアドを実行する
            IEnumerable<dynamic> result
                = connection.ExecuteQuery(
                    $"[dbo].[{functionName}]"
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
        public static (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(string functionName, IDictionary<string, object?> param, int? timeout = null)
        {
            using SqlConnection connection = GetNewConnection();
            return ExecuteStoredFunctionReturnOretAndOmsg(connection, functionName, param, timeout: timeout);
        }
        /// <summary>
        /// ストアドファンクション/プロシージャを実行する
        /// param内にoutputパラメータとしてretCode,retMsgを追加する
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(
            SqlConnection connection
            , string functionName
            , IDictionary<string, object?> param
            , SqlTransaction? tran = null
            , int? timeout = null
            )
        {
            //戻り値のパラメータを追加する。StoredFunctionの定義にはあらかじめ以下2点のOUTPUTパラメータをを定義して置く必要あり
            param.Add("O_RET", new SqlParameter($"{ParamPrefixStr}O_RET", SqlDbType.Int) { Direction = ParameterDirection.Output });
            param.Add("O_MSG", new SqlParameter($"{ParamPrefixStr}O_MSG", SqlDbType.VarChar, 1024) { Direction = ParameterDirection.Output });
            OpenConnectionEnsure(connection);
            LogTo.Information($"ExecuteQuery_Start:[dbo].[{functionName}]");
            //repoDBを使用してfunctionNameのストアドを実行する
            _ = connection.ExecuteQuery(
                $"[dbo].[{functionName}]"
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
        public static object ExecuteStoredFunction(string functionName, ref IDictionary<string, object?> param, int? timeout = null)
        {
            using SqlConnection connection = GetNewConnection();
            return ExecuteStoredFunction(connection, functionName, ref param, timeout: timeout);
        }
        /// <summary>
        /// ストアドファンクション/プロシージャを実行する
        /// outputパラメータを取り出せるようにparamはrefで受ける。
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteStoredFunction(
            SqlConnection connection
            , string functionName
            , ref IDictionary<string, object?> param
            , SqlTransaction? tran = null
            , int? timeout = null
            )
        {
            OpenConnectionEnsure(connection);
            //TODO 戻りなど、確認する必要あり
            //repoDBを使用してfunctionNameのストアドを実行する
            IEnumerable<dynamic> result
                = connection.ExecuteQuery(
                    $"[dbo].[{functionName}]"
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
        public static SqlDbType GetDbType(string typeString)
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
                "structured" => SqlDbType.Structured,
                "text" => SqlDbType.Text,
                "time" => SqlDbType.Time,
                "timestamp" => SqlDbType.Timestamp,
                "tinyint" => SqlDbType.TinyInt,
                "uniqueidentifier" => SqlDbType.UniqueIdentifier,
                "varbinary" => SqlDbType.VarBinary,
                "varchar" => SqlDbType.VarChar,
                "xml" => SqlDbType.Xml,
                _ => SqlDbType.BigInt
            };
            return dbType;
        }

        /// <summary>
        /// SQLServerより取得したデータ型の文字列からSqlDbType列挙型に変換する
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static Type GetType(string typeString)
        {
            Type dbType = typeString.ToLower() switch
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
            return dbType;
        }

        /// <summary>
        /// SqlDbType列挙型から文字列型からデータ型に変換する
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static object GetDbTypeVal(SqlDbType type, string val)
        {
            object ret;
            switch (type)
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
    }
}
