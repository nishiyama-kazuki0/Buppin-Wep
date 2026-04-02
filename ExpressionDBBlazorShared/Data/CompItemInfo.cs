using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// コンポーネント情報
/// </summary>
public class CompItemInfo
{
    /// <summary>
    /// コンポーネント　Type
    /// </summary>
    public Type CompType;
    /// <summary>
    /// コンポーネント　パラメータ
    /// </summary>
    public Dictionary<string, object> CompParam;
    /// <summary>
    /// コンポーネント　オブジェクト
    /// </summary>
    public DynamicComponent? CompObj;
    /// <summary>
    /// タイトルラベル
    /// </summary>
    public string TitleLabel { get; set; }
    /// <summary>
    /// タイトルラベルの表示非表示
    /// </summary>
    public bool DispTitleLabel { get; set; }
    /// <summary>
    /// 必須表示非表示
    /// </summary>
    public bool DispRequiredSuffix { get; set; }
    /// <summary>
    /// コンポーネント名
    /// </summary>
    public string ComponentName { get; set; }
    /// <summary>
    /// View名
    /// </summary>
    public string ViewName { get; set; }
    /// <summary>
    /// 値データタイプ文字列
    /// </summary>
    public string ValueDataType { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CompItemInfo()
    {
        TitleLabel = string.Empty;
        CompType = typeof(RadzenText);
        CompParam = [];
        DispTitleLabel = true;
        DispRequiredSuffix = false;
        ComponentName = string.Empty;
        ViewName = string.Empty;
        ValueDataType = string.Empty;
    }
}
