using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 実行中バッチ情報VIEWモデル
/// </summary>
public class VW_BATCHACTIVECHECK_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_BATCHACTIVECHECK_INFO";

    /// <summary>
    /// バッチNO
    /// </summary>
    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;
    /// <summary>
    /// バッチ開始年月日(YYYYMMDD)
    /// </summary>
    [Column("バッチ開始年月日")]
    public string StartBatch { get; set; } = string.Empty;
    /// <summary>
    /// 浸漬タンクNO（9211、9212、9311、9312、9411、9511）
    /// </summary>
    [Column("浸漬タンクNO")]
    public string ImmersionTankNoChar { get; set; } = string.Empty;
}
