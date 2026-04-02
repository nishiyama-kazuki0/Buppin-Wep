using SharedModels;
using System.Data;
namespace ExpressionDBWebAPI.Common;

/// <summary>
/// データベース接続インターフェース
/// </summary>
internal interface IDataSource
{
    public static readonly string ParamPrefixStr = "@";//TODO コンストラクタでDB種類によって変更する

    /// <summary>
    /// 引用符を付加する
    /// </summary>
    /// <returns></returns>
    public string AddQuotationMarks(string str);

    public string GetConnectString();

    /// <summary>
    /// 新しいDBConnectionを取得する。
    /// 主に本クラス外部でもコネクションを使用する場合に使用する。外側の変数は原則、usingで受けること。
    /// 例.トランザクションを外側で張りたいときなど
    /// </summary>
    /// <returns></returns>
    public IDbConnection GetNewConnection();

    /// <summary>
    /// connectionを参照引数で受けて、OpenしてなかったらOpenする
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public void OpenConnectionEnsure(IDbConnection connection);

    /// <summary>
    /// 引数で受けたconnectionのトランザクションを開始する。開始後にトランザクションオブジェクトを返す。
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public IDbTransaction BeginTransactionAndOpenEnsure(IDbConnection connection);

    /// <summary>
    /// トランザクションをコミットして、connectionを閉じる。閉じる前にコミットすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public void CommitTransactionAndCloseEnsure(IDbTransaction transaction);
    /// <summary>
    /// トランザクションをロールバックして、connectionを閉じる。閉じる前にロールバックすることを保証する。
    /// </summary>
    /// <param name="transaction"></param>
    public void RollbackTransactionAndCloseEnsure(IDbTransaction transaction);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public List<ResponseValue> ExecuteQuery(string strSQL, List<IDbDataParameter>? parameters = null, int commandTimeout = 30);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="IsTableFunc">true:テーブルを返すFUNCTION</param>
    /// <returns></returns>
    public List<ResponseValue> ExecuteQuery(string strSQL, IDictionary<string, WhereParam>? param, bool IsTableFunc = false, int commandTimeout = 30);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public int ExecuteNonQuery(string strSQL, IDictionary<string, WhereParam>? param);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <param name="IsTableFunc"></param>
    /// <returns></returns>
    public List<IDbDataParameter>? CreateDbDataParamList(IDictionary<string, WhereParam>? param, bool IsTableFunc = false);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public string? GetScalarValueNonQuery(string strSQL, IDictionary<string, object?> param);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <param name="tran"></param>
    /// <returns></returns>
    public string? GetScalarValueNonQuery(IDbConnection connection, string strSQL, IDictionary<string, object?> param, IDbTransaction? tran = null);
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, IDictionary<string, object?> param) where TDto : class, new();
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, IDictionary<string, object?> param, IDbTransaction? tran = null) where TDto : class, new();
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDataParameter> param) where TDto : class, new();
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDataParameter> param, IDbTransaction? tran = null) where TDto : class, new();
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(string strSQL, List<IDbDataParameter> param) where TDto : class, new();
    /// <summary>
    /// SQL文とparamを指定してEntityを取得する
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="strSQL"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public IEnumerable<TDto> GetEntityCollection<TDto>(IDbConnection connection, string strSQL, List<IDbDataParameter> param, IDbTransaction? tran = null) where TDto : class, new();
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(string functionName, IDictionary<string, object?> param, int? timeout = null);
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
        );
    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// param内にoutputパラメータとしてretCode,retMsgを追加する
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public (int O_RET, string O_MSG) ExecuteStoredFunctionReturnOretAndOmsg(string functionName, IDictionary<string, object?> param, int? timeout = null);
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
        );

    /// <summary>
    /// ストアドファンクション/プロシージャを実行する
    /// outputパラメータを取り出せるようにparamはrefで受ける。
    /// </summary>
    /// <param name="functionName"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public object ExecuteStoredFunction(string functionName, ref IDictionary<string, object?> param, int? timeout = null);
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
        );
    ///// <summary>
    ///// データベースより取得したデータ型の文字列からSqlDbType列挙型に変換する//Todo一旦残しておく
    ///// </summary>
    ///// <param name="typeString"></param>
    ///// <returns></returns>
    //public SqlDbType GetDbType(string typeString);
    /// <summary>
    /// データベースより取得したデータ型の文字列からSqlDbType列挙型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public DbType GetDbType(string typeString);

    /// <summary>
    /// データベースより取得したデータ型の文字列からSqlDbType列挙型に変換する
    /// </summary>
    /// <param name="typeString"></param>
    /// <returns></returns>
    public Type GetType(string typeString);

    /// <summary>
    /// DbTypeも文字列をもとに文字列のデータを.net の値に変換する
    /// </summary>
    /// <param name="dbtypeString"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public object ConvertDbTypeVal(string dbtypeString, string val);

    /// <summary>
    /// パラメーター作成
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public IDataParameter CreateParameter(string parameterName, object parameterValue);
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type);
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size);
    public IDataParameter CreateParameter(string parameterName, object parameterValue, Type type, int size, ParameterDirection direction);

    /// <summary>
    /// TableFunction取得クエリ作成
    /// param名はプレフィックスをつけない名称を引数で指定すること
    /// </summary>
    /// <param name="procedureName"></param>
    /// <param name="paramName"></param>
    /// <param name="orderParamName"></param>
    /// <returns></returns>
    public string CreateTableFunctionQuery(string procedureName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null);
    public string CreateSimpleSelectQuery(string tableName, string[] paramName, (string colName, bool isDesc)[]? orderParamName = null);
    /// <summary>
    /// パラメータのListを作成して返す
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public List<IDataParameter> CreateParamList(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param);
    /// <summary>
    /// パラメータのDictionalyを作成して返す。
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public IDictionary<string, object?> CreateParamTable(Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> param);
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction);
    /// <summary>
    /// WHERE句でのTABLEFUNCTION指定
    /// </summary>
    /// <param name="ScalarFunctionName"></param>
    /// <param name="colName"></param>
    /// <param name="whereType"></param>
    /// <param name="i"></param>
    /// <param name="Conjuction"></param>
    /// <returns></returns>
    public string CreateWhereSQLString(string colName, string whereType, int i, string Conjuction, string ScalarFunctionName);
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
    public string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction);
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
    public string CreateWhereSQLString(string colName, string whereType, int i, int j, string Conjuction, string ScalarFunctionName);
    /// <summary>
    /// ヒント句文字列取得
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    public string GetStringQueryHints(ClassNameSelect selectInfo);
    /// <summary>
    /// サイズ指定が必要なDbTypeかどうかを判定する。主に文字列型の場合にtrueとなる。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsSizeSpecNeeded(string dbTypeString);
    /// <summary>
    /// FUNCの引数がテーブル型の引数かどうかを判定する。
    /// </summary>
    /// <param name="dbTypeString"></param>
    /// <returns></returns>
    public bool IsTableArgument(string dbTypeString);
}
