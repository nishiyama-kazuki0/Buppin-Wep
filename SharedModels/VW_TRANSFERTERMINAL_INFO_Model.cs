using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 終端移し替え情報VIEWモデル
/// </summary>
public class VW_TRANSFERTERMINAL_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_TRANSFERTERMINAL_INFO";

    /// <summary>
    /// バッチ管理ID
    /// </summary>
    [Column("バッチ管理ID")]
    public string BatchControlId { get; set; } = string.Empty;
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
    /// 専用容器QR
    /// </summary>
    [Column("専用容器QR")]
    public string DedicatedQR { get; set; } = string.Empty;
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
    /// バッチ開始時刻(HHMMSS)
    /// </summary>
    [Column("バッチ開始時刻")]
    public string StartBatchTime { get; set; } = string.Empty;
    /// <summary>
    /// 原料登録No(XXXX)
    /// </summary>
    [Column("原料登録NO")]
    public short MaterialNo { get; set; } = 0;
    /// <summary>
    /// 端数原料移載無し
    /// </summary>
    [Column("端数原料移載無し")]
    public int IsTransferMaterialNothing_Fraction { get; set; } = 0;
    /// <summary>
    /// 代表積載位置
    /// </summary>
    [Column("代表積載位置")]
    public int RepresentsPosition { get; set; } = 0;

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
