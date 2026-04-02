namespace SharedModels;

public class ExecResult
{
    /// <summary>
    /// 実行プログラム名
    /// </summary>
    public string ProgramName { get; set; }
    /// <summary>
    /// 実行ファンクション名
    /// </summary>
    public string FunctionName { get; set; }
    /// <summary>
    /// 実行順
    /// </summary>
    public int ExecOrderRank { get; set; }
    /// <summary>
    /// 結果コード
    /// </summary>
    public int RetCode { get; set; }
    /// <summary>
    /// 結果メッセージ
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// 実行結果戻り値。(未使用)
    /// </summary>
    public object? ResultValue { get; set; }
    /// <summary>
    /// 実行結果戻り値型。(未使用)
    /// </summary>
    public string? ResultValueType { get; set; }

    //TODO 要求元のクライアント名、ユーザー名が必要か検討する

    //JSONの関係でメンバーはすべてpublicで宣言必要とのこと。
    //TODO publicの場合はコンストラクタを定義する意味が薄れるので、削除するか検討。
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="programName"></param>
    /// <param name="functionName"></param>
    /// <param name="retCode"></param>
    public ExecResult(
        string programName
        , string functionName
        , int execOrderRank
        , int retCode
        , string message
        , object? resultValue = null
        , string? resultValueType = null
        )
    {
        ProgramName = programName;
        FunctionName = functionName;
        ExecOrderRank = execOrderRank;
        RetCode = retCode;
        Message = message;
        ResultValue = resultValue;
        ResultValueType = resultValueType;
    }
}
