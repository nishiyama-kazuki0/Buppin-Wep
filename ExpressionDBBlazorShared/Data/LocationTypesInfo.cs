using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// ロケーション区分
/// </summary>
public class LocationTypesInfo
{
    /// <summary>
    /// ロケーション区分
    /// </summary>
    public int LocationType { get; set; }
    /// <summary>
    /// ロケーション区分名
    /// </summary>
    public string LocationTypeName { get; set; }
    /// <summary>
    /// グリッド表示：前景色
    /// </summary>
    public string CellForeColor { get; set; }
    /// <summary>
    /// グリッド表示：背景色
    /// </summary>
    public string CellBackColor { get; set; }
    /// <summary>
    /// グリッド表示：非選択
    /// </summary>
    public bool CellUnSelectable { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LocationTypesInfo()
    {
        LocationType = 0;
        LocationTypeName = string.Empty;
        CellForeColor = string.Empty;
        CellBackColor = string.Empty;
        CellUnSelectable = false;
    }
}
