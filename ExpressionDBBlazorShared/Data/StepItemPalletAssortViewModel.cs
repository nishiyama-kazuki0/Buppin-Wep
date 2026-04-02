namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// パレット詰合せViewModel
/// </summary>
public class StepItemPalletAssortViewModel : BaseViewModel
{
    /// <summary>
    /// 初期化させない判断用フラグ
    /// </summary>
    public bool NotClear { get; set; } = false;

    /// <summary>共通パレットNo</summary>
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>元パレットNo</summary>
    public string PPalletNo { get; set; } = string.Empty;
    //----
    /// <summary>混載</summary>
    public bool IsMixed { get; set; } = false;
    public string Mixed { get; set; } = string.Empty;
    /// <summary>混載アラートタイトル</summary>
    public string AlertTitle { get; set; } = string.Empty;
    /// <summary>混載アラートテキスト</summary>
    public string AlertText { get; set; } = string.Empty;
    /// <summary>品名</summary>
    public string PProductName { get; set; } = string.Empty;
    /// <summary>等級-階級</summary>
    public string GradeClass { get; set; } = string.Empty;
    /// <summary>産地</summary>
    public string ProductArea { get; set; } = string.Empty;
    /// <summary>総バラ数</summary>
    public string TotalBara { get; set; } = string.Empty;
    //----
    /// <summary>子パレットNo</summary>
    public string CPalletNo { get; set; } = string.Empty;
    /// <summary>子品名</summary>
    public string CProductName { get; set; } = string.Empty;
    /// <summary>等級</summary>
    public string Grade { get; set; } = string.Empty;
    /// <summary>階級</summary>
    public string Class { get; set; } = string.Empty;
    /// <summary>ケース数</summary>
    public string Case { get; set; } = string.Empty;
    /// <summary>バラ数</summary>
    public string Bara { get; set; } = string.Empty;

    /// <summary>入力子パレットNo</summary>
    public string InCPalletNo { get; set; } = string.Empty;
}
