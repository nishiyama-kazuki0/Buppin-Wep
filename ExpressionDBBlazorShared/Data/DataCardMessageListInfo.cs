namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// カードメッセージリストデータクラス
/// </summary>
public class DataCardMessageListInfo
{
    /// <summary>
    /// 発生時刻
    /// </summary>
    public string OccurredTime { get; set; }
    /// <summary>
    /// 結果区分
    /// </summary>
    public int ResultCategory { get; set; }
    /// <summary>
    /// メッセージ
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DataCardMessageListInfo()
    {
        OccurredTime = string.Empty;
        ResultCategory = 0;
        Message = string.Empty;
    }
}
