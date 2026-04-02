using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// デパレ連携情報内容VIEWモデル
/// </summary>
public class VW_DEPALLETIZE_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_DEPALLETIZE_INFO";

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
    /// 外装区分（1:クラフト袋/2:段ボール/3:ペール缶）
    /// </summary>
    [Column("外装区分")]
    public short ExteriorType { get; set; } = 0;
    /// <summary>
    /// 保管区分（1：常温/2：冷凍）
    /// </summary>
    [Column("保管区分")]
    public SharedConst.StorageKind StorageType { get; set; } = SharedConst.StorageKind.None;
    /// <summary>
    /// 原料固有No1
    /// </summary>
    [Column("原料固有NO1")]
    public short MaterialSpecificNo1 { get; set; } = 0;
    /// <summary>
    /// 正梱在庫数
    /// </summary>
    [Column("正梱在庫数")]
    public short OriginalStockQTY { get; set; } = 0;
    /// <summary>
    /// デパレ予定数
    /// </summary>
    [Column("デパレ予定数")]
    public short DepalletizePlanQTY { get; set; } = 0;
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
    /// 解凍パターン
    /// </summary>
    [Column("解凍パターン")]
    public short ThawPattern { get; set; } = 0;
    /// <summary>
    /// AGVタスクID
    /// (AGV搬送時のAGVタスクID)
    /// </summary>
    [Column("AGVタスクID")]
    public int AGVTaskID { get; set; } = 0;

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
