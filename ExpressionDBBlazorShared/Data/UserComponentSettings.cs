namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// MST_USER_COMPONENT_SETTINGSテーブル情報
/// </summary>
public class UserComponentSettingsInfo
{
    /// <summary>
    /// コンポーネント名
    /// </summary>
    public string ComponentName { get; set; }
    /// <summary>
    /// View名
    /// </summary>
    public string ViewName { get; set; }
    /// <summary>
    /// キー
    /// </summary>
    public string PropertyKey { get; set; }
    /// <summary>
    /// 値ID
    /// </summary>
    public int ValueKeyId { get; set; }
    /// <summary>
    /// 値
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 値データタイプ文字列
    /// </summary>
    public string ValueDataType { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserComponentSettingsInfo()
    {
        ComponentName = string.Empty;
        ViewName = string.Empty;
        PropertyKey = string.Empty;
        ValueKeyId = 0;
        ValueDataType = string.Empty;
    }
}
