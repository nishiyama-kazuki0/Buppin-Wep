using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 端数移し替え情報VIEWモデル
/// </summary>
public class VW_TRANSFERFRACTION_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_TRANSFERFRACTION_INFO";

    /// <summary>
    /// バッチNo
    /// </summary>
    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;
    /// <summary>
    /// パレットNo
    /// </summary>
    [Column("パレットNO")]
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>
    /// 原料品目コード
    /// </summary>
    [Column("原料品目コード")]
    public string ItemCode { get; set; } = string.Empty;
    /// <summary>
    /// 投入フェーズ
    /// </summary>
    [Column("投入フェーズ")]
    public short EntryPhase { get; set; } = 0;
    /// <summary>
    /// バッチ開始年月日(YYYYMMDD)
    /// </summary>
    [Column("バッチ開始年月日")]
    public string StartBatch { get; set; } = string.Empty;
    /// <summary>
    /// 原料登録No(XXXX)
    /// </summary>
    [Column("原料登録NO")]
    public short MaterialNo { get; set; } = 0;

    /// <summary>
    /// バッチ開始年(YYYY)
    /// </summary>
    public short StartBatchYear => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) / 10000);
    /// <summary>
    /// バッチ開始月(MM)
    /// </summary>
    public short StartBatchMonth => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) / 100 % 100);
    /// <summary>
    /// バッチ開始日(DD)
    /// </summary>
    public short StartBatchDay => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) % 100);
}
