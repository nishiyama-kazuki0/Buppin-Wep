namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// DropDown、RadioButton、CheckBoxなどのデータを格納するためのクラス
/// </summary>
public class ValueTextInfo
{
    /// <summary>
    /// 値
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 表示
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// Value1～Value10はドロップダウンデータグリッド用
    /// </summary>
    public string Value1 { get; set; } = null!;
    public string Value2 { get; set; } = null!;
    public string Value3 { get; set; } = null!;
    public string Value4 { get; set; } = null!;
    public string Value5 { get; set; } = null!;
    public string Value6 { get; set; } = null!;
    public string Value7 { get; set; } = null!;
    public string Value8 { get; set; } = null!;
    public string Value9 { get; set; } = null!;
    public string Value10 { get; set; } = null!;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ValueTextInfo()
    {
        Value = string.Empty;
        Text = string.Empty;
        Value1 = string.Empty;
        Value2 = string.Empty;
        Value3 = string.Empty;
        Value4 = string.Empty;
        Value5 = string.Empty;
        Value6 = string.Empty;
        Value7 = string.Empty;
        Value8 = string.Empty;
        Value9 = string.Empty;
        Value10 = string.Empty;
    }

    /// <summary>
    /// テキストの取得
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Text;
    }
}
