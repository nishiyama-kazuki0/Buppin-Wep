using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 解凍指示数量情報内容VIEWモデル
/// </summary>
public class VW_ORDERTHAWQTY_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_ORDERTHAWQTY_INFO";

    /// <summary>
    /// 解凍パターン
    /// </summary>
    [Column("解凍パターン")]
    public short ThawPattern { get; set; } = 0;
    /// <summary>
    /// 解凍予定数
    /// </summary>
    [Column("解凍予定数")]
    public short ThawPlanQTY { get; set; } = 0;
    /// <summary>
    /// 解凍確定数
    /// </summary>
    [Column("解凍確定数")]
    public short ThawFixedQTY { get; set; } = 0;
}
