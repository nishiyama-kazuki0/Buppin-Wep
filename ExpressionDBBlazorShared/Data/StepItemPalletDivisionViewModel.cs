namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// パレット詰合せViewModel
/// </summary>
public class StepItemPalletDivisionViewModel : BaseViewModel
{
    /// <summary>
    /// 初期化させない判断用フラグ
    /// </summary>
    public bool NotClear { get; set; } = false;

    /// <summary>元パレットNo.</summary>
    public string MPalletNo { get; set; } = string.Empty;
    /// <summary>入荷明細No</summary>
    public string ArrivalDetailNo { get; set; } = string.Empty;

    /// <summary>パレットNo.</summary>
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>混載</summary>
    public bool IsMixed { get; set; } = false;
    /// <summary>入荷No</summary>
    public string ArrivalNo { get; set; } = string.Empty;
    /// <summary>明細No</summary>
    public string DetailNo { get; set; } = string.Empty;
    /// <summary>入荷-明細No</summary>
    public string ArrivalDetailNoShow { get; set; } = string.Empty;
    /// <summary>品名</summary>
    public string ProductName { get; set; } = string.Empty;
    /// <summary>等級</summary>
    public string Grade { get; set; } = string.Empty;
    /// <summary>階級</summary>
    public string Class { get; set; } = string.Empty;
    /// <summary>等級-階級</summary>
    public string GradeClass { get; set; } = string.Empty;
    /// <summary>産地</summary>
    public string ProductArea { get; set; } = string.Empty;
    /// <summary>ケース数</summary>
    public string Case { get; set; } = string.Empty;
    /// <summary>バラ数</summary>
    public string Bara { get; set; } = string.Empty;
    /// <summary>総バラ数</summary>
    public string MTotalBara { get; set; } = string.Empty;
    /// <summary>総引当済数</summary>
    public string MTotalAllocateBara { get; set; } = string.Empty;
    /// <summary>出荷予定集約ID</summary>
    public string ShippingAggrId { get; set; } = string.Empty;
    //----
    /// <summary>先パレットNo.</summary>
    public string SPalletNo { get; set; } = string.Empty;
    /// <summary>総バラ数</summary>
    public string STotalBara { get; set; } = string.Empty;

}
