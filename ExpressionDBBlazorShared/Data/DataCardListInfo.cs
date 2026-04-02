namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// カードリストデータクラス
/// </summary>
public class DataCardListInfo
{
    /// <summary>
    /// 表示フラグ
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DataCardListInfo()
    {
        Visible = true;
        Value = string.Empty;
    }
}
