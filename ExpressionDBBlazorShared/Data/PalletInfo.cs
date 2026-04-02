namespace ExpressionDBBlazorShared.Data;

public class PalletInfo
{
    /// <summary>
    /// 混載
    /// </summary>
    public bool IsMixed { get; set; }
    /// <summary>
    /// 引当済在庫
    /// </summary>
    public bool IsReserved { get; set; }
    /// <summary>
    /// 引当済アラートタイトル
    /// </summary>
    public string ReservedAlertTitle { get; set; } = string.Empty;
    /// <summary>
    /// 引当済アラートテキスト
    /// </summary>
    public string ReservedAlertText { get; set; } = string.Empty;
    /// <summary>
    /// 混載アラートタイトル
    /// </summary>
    public string MixedAlertTitle { get; set; } = string.Empty;
    /// <summary>
    /// 混載アラートテキスト
    /// </summary>
    public string MixedAlertText { get; set; } = string.Empty;
}
