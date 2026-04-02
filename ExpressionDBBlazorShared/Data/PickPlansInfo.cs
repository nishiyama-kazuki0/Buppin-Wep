namespace ExpressionDBBlazorShared.Data;

public class PickPlansInfo
{
    /// <summary>
    /// 倉庫配送先
    /// </summary>
    public string DeliveryCd { get; set; } = string.Empty;
    /// <summary>
    /// 倉庫コード
    /// </summary>
    public string AreaCd { get; set; } = string.Empty;
    /// <summary>
    /// ゾーンコード
    /// </summary>
    public string ZoneCd { get; set; } = string.Empty;
}
