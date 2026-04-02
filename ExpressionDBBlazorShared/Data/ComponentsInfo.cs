namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// DEFINE_COMPONENTSテーブル情報
/// </summary>
public class ComponentsInfo
{
    /// <summary>
    /// DEFINE_COMPONENTSのVALUE_OBJECT_TYPEの定義
    /// </summary>
    public enum EnumValueObjectType : int
    {
        ValueIndicator = 0,
        VariableIndicator = 1,
        EnumStringIndicator = 2,
        ClassNameIndicator = 3,
    }
    /// <summary>
    /// コンポーネント名
    /// </summary>
    public string ComponentName { get; set; }
    /// <summary>
    /// Attribute名
    /// </summary>
    public string AttributesName { get; set; }
    /// <summary>
    /// 値
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 0：値、1：変数、2：Enum値 , 3:クラス名 (ダイアログの判断に使用している)
    /// </summary>
    public byte ValueObjectType { get; set; }
    /// <summary>
    /// データタイプ
    /// </summary>
    public Type Type { get; set; }
    /// <summary>
    /// 最小値
    /// </summary>
    public int ValueMin { get; set; }
    /// <summary>
    /// 最大値
    /// </summary>
    public int ValueMax { get; set; }
    public ComponentsInfo()
    {
        ComponentName = "";
        AttributesName = "";
        Value = "";
        Type = typeof(string);
        ValueMin = 0;
        ValueMax = 0;
    }
}
