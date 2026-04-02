using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// デパレ解凍情報モデル (取り急ぎ必要な列のみ定義)
/// </summary>
public class VW_DEPALLET_THAWING_STATUS_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    private const string _VIEW_NAME = "VW_DEPALLET_THAWING_STATUS";
    /// <summary>
    /// VIEW名
    /// </summary>
    public static new string VIEW_NAME => VW_DEPALLET_THAWING_STATUS_Model._VIEW_NAME;

    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;

    [Column("バッチ開始年月日")]
    public string StartBatch { get; set; } = string.Empty;

    [Column("原料品目コード")]
    public string ItemCode { get; set; } = string.Empty;

    [Column("原料登録NO")]
    public short MaterialNo { get; set; } = 0;

    [Column("デパレカウントNO")]
    public int DepalletizeCountNo { get; set; } = 0;
}
