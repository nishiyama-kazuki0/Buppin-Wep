using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 入出庫予約データ情報内容VIEWモデル
/// </summary>
public class VW_RESERVED_WAREHOUSE_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_RESERVED_WAREHOUSE_INFO";

    /// <summary>
    /// パレットNo
    /// </summary>
    [Column("パレットNO")]
    public string PalletNo { get; set; } = string.Empty;

    // TODO 階(1F／5F) と 入庫／出庫 に分ける？
    /// <summary>
    /// 入出庫位置区分（1：1F入庫／2：1F出庫／3：5F入庫／4：5F出庫）
    /// </summary>
    [Column("入出庫位置区分")]
    public PositionKind PositionType { get; set; } = PositionKind.None;

    /// <summary>
    /// 保管区分（1：常温／2：冷凍）
    /// </summary>
    [Column("保管区分")]
    public SharedConst.StorageKind StorageType { get; set; } = SharedConst.StorageKind.None;

    // TODO 処理する並び順(抽出時のORDER BY想定)
    /// <summary>
    /// 処理順序
    /// </summary>
    [Column("処理順序")]
    public int OrderNo { get; set; } = 0;

    // TODO 入出庫予約データの進行状況は管理する想定
    /// <summary>
    /// 入出庫進行状況（0：予約／1：入出庫中／2：入出庫済）
    /// 自動倉庫内の該当パレットに対する進行状況を指す
    /// </summary>
    [Column("進行状況")]
    public StateKind StateType { get; set; } = StateKind.Reserve;

    // TODO （下記の棚番項目）棚番引き当て後の情報は別VIEWにする？
    /// <summary>
    /// 引当棚番列
    /// </summary>
    [Column("引当棚番列")]
    public string LocationColumnChar { get; set; } = string.Empty;

    /// <summary>
    /// 引当棚番連
    /// </summary>
    [Column("引当棚番連")]
    public string LocationRowChar { get; set; } = string.Empty;

    /// <summary>
    /// 引当棚番段
    /// </summary>
    [Column("引当棚番段")]
    public string LocationTierChar { get; set; } = string.Empty;

    /// <summary>
    /// 直送区分（ 0:非直送、1:直送 ）
    /// 自動倉庫のクレーン取合い位置にて自動倉庫へ指示する条件
    /// </summary>
    [Column("直送区分")]
    public TransferDirectKind TransferDirect { get; set; } = TransferDirectKind.No;

    /// <summary>
    /// FROM_AGVステーションコード<br/>
    /// ※入庫の場合はTO,FROMが同じになるのでAGV搬送指示は作成されない想定。出庫でも同じTO,FROMの場合は搬送指示は作成されない。
    /// </summary>
    [Column("FROM_AGVステーションコード")]
    public string FromAgvSTChar { get; set; } = string.Empty;

    /// <summary>
    /// TO_AGVステーションコード<br/>
    /// ※NULLの場合は入庫
    /// </summary>
    [Column("TO_AGVステーションコード")]
    public string ToAgvSTChar { get; set; } = string.Empty;

    /// <summary>
    /// 作業優先順<br/>
    /// </summary>
    [Column("作業優先順")]
    public string WorkOrderdNo { get; set; } = string.Empty;

    /// <summary>
    /// 生産タスクID
    /// </summary>
    [Column("生産タスクID")]
    public string ProductionTaskId { get; set; } = string.Empty;

    // （下記の棚番項目）デバイスへ設定する数値型へ変換した値
    /// <summary>
    /// 引当棚番列
    /// </summary>
    public short LocationColumn => (short)(string.IsNullOrWhiteSpace(LocationColumnChar) == true ? 0 : short.Parse(LocationColumnChar));

    /// <summary>
    /// 引当棚番連
    /// </summary>
    public short LocationRow => (short)(string.IsNullOrWhiteSpace(LocationRowChar) == true ? 0 : short.Parse(LocationRowChar));

    /// <summary>
    /// 引当棚番段
    /// </summary>
    public short LocationTier => (short)(string.IsNullOrWhiteSpace(LocationTierChar) == true ? 0 : short.Parse(LocationTierChar));

    // （下記のST）デバイスへ設定する数値型へ変換した値
    /// <summary>
    /// FROM_AGVステーションコード
    /// </summary>
    public short FromAgvST => (short)(string.IsNullOrWhiteSpace(FromAgvSTChar) == true ? 0 : short.Parse(FromAgvSTChar));

    /// <summary>
    /// TO_AGVステーションコード
    /// </summary>
    public short ToAgvST => (short)(string.IsNullOrWhiteSpace(ToAgvSTChar) == true ? 0 : short.Parse(ToAgvSTChar));

    /// <summary>
    /// 入出庫位置区分
    /// </summary>
    public enum PositionKind : int
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 1：1F入庫
        /// </summary>
        Storage1F = 1,
        /// <summary>
        /// 2：1F出庫
        /// </summary>
        Retrieval1F = 2,
        /// <summary>
        /// 3：5F入庫
        /// </summary>
        Storage5F = 3,
        /// <summary>
        /// 4：5F出庫
        /// </summary>
        Retrieval5F = 4,
    }

    /// <summary>
    /// 入出庫進行状況
    /// </summary>
    public enum StateKind
    {
        /// <summary>
        /// 0：予約
        /// </summary>
        Reserve = 0,
        /// <summary>
        /// 1：入出庫中
        /// </summary>
        Processing = 1,
        /// <summary>
        /// 2：入出庫済
        /// </summary>
        Done = 2,
    }

    /// <summary>
    /// 直送区分
    /// </summary>
    public enum TransferDirectKind : int
    {
        /// <summary>
        /// 非直送（直送ではない）
        /// </summary>
        No = 0,
        /// <summary>
        /// 直送（直送である）
        /// </summary>
        Yes = 1,
    }
}
