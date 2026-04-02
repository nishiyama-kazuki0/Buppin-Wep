namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// DEFINE_COMPONENT_PROGRAMテーブル情報
/// 西山
/// </summary>
public class ComponentProgramInfo
{
    /// <summary>
    /// 現在メソッド名
    /// </summary>
    public string CurrentMethodName { get; set; }
    /// <summary>
    /// クライアント側メソッド名:クライアント側で呼び出すメソッド名
    /// または、アノテーション名
    /// </summary>
    public string CallMethodName { get; set; }
    /// <summary>
    /// コンポーネント名
    /// 西山16
    /// </summary>
    public string ComponentName { get; set; }
    /// <summary>
    /// メソッドの実行順
    /// </summary>
    public byte ExecOrderRank { get; set; }
    /// <summary>
    /// プログラム名:サーバー側のプログラム呼び出し名
    /// </summary>
    public string ProcessProgramName { get; set; }
    /// <summary>
    /// 権限レベル下限値:本値以上の権限レベルでないと適用しないとする。
    /// </summary>
    public byte AuthorityLevelLower { get; set; }
    /// <summary>
    /// 呼出タイプ:未使用
    /// </summary>
    public byte PrgramCallType { get; set; }
    /// <summary>
    /// 戻り値有無:0_該当なし
    /// </summary>
    public byte IsProgramReturn { get; set; }
    /// <summary>
    /// 戻り値データタイプ:空白_該当なし
    /// </summary>
    public Type RetrunDataType { get; set; }
    /// <summary>
    /// タイムアウト時間:単位[ms]負数はタイムアウト無し
    /// </summary>
    public int TimeoutValue { get; set; }
    /// <summary>
    /// リトライ数:画面側のリトライ数
    /// </summary>
    public byte RetryCount { get; set; }
    /// <summary>
    /// 引数データセット名:空白は未使用。PROCESS_FUNCTIONでデータテーブル型の引数を使用する場合の引数名。
    /// </summary>
    public string ArgumentDataSetName { get; set; }
    /// <summary>
    /// 非同期実行有無　0_同期,1_非同期
    /// </summary>
    public bool IsAsync { get; set; }
    public ComponentProgramInfo()
    {
        CurrentMethodName = string.Empty;
        CallMethodName = string.Empty;
        ComponentName = string.Empty;
        ExecOrderRank = 0;
        ProcessProgramName = string.Empty;
        AuthorityLevelLower = 0;
        PrgramCallType = 0;
        IsProgramReturn = 0;
        RetrunDataType = typeof(string);
        TimeoutValue = 0;
        RetryCount = 0;
        ArgumentDataSetName = string.Empty;
        IsAsync = false;
    }
}
