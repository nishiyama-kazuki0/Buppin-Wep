using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 投入原料情報VIEWモデル
/// </summary>
public class VW_ARRIVALENTRY_INFO_Model : VW_RECEIPT_INFO_Model
{
    /// <summary>
    /// VIEW名
    /// </summary>
    private const string _VIEW_NAME = "VW_ARRIVALENTRY_INFO";
    /// <summary>
    /// VIEW名
    /// </summary>
    public static new string VIEW_NAME => VW_ARRIVALENTRY_INFO_Model._VIEW_NAME;

    /// <summary>
    /// 浸漬タンクNO（9211、9212、9311、9312、9411、9511）
    /// ※デバイス値セットのエリア特定に使用
    /// </summary>
    [Column("浸漬タンクNO")]
    public string ImmersionTankNoChar { get; set; } = string.Empty;
    /// <summary>
    /// AGVタスクID
    /// (AGV搬送時のAGVタスクID)
    /// </summary>
    [Column("AGVタスクID")]
    public int AGVTaskID { get; set; } = 0;
    /// <summary>
    /// バッチ内投入区分（1：初回投入／0：左記以外）
    /// ( 同一バッチ内で原料投入の初回投入を特定し計装システムへの連携デバイスの条件とする )
    /// </summary>
    [Column("バッチ内投入区分")]
    public BatchInputKind BatchInputType { get; set; } = BatchInputKind.None;

    /// <summary>
    /// バッチ最終区分(0:最終以外／1:最終) 
    /// </summary>
    [Column("バッチ最終区分")]
    public LastBatchKind LastBatchType { get; set; } = 0;

    /// <summary>
    /// 次投入フェーズ
    /// </summary>
    [Column("次投入フェーズ")]
    public short NextInputPhaseNo { get; set; } = 0;

    /// <summary>
    /// バッチ内投入区分
    /// </summary>
    public enum BatchInputKind : int
    {
        /// <summary>
        /// 定義なし（その他）
        /// </summary>
        None = 0,
        /// <summary>
        /// 初回投入
        /// </summary>
        First = 1,
        // 下記の「最終投入（全完了）」は廃止する
        /// <summary>
        /// 最終投入（全完了）
        /// </summary>
        Last = 2,
    }

    /// <summary>
    /// バッチ最終区分
    /// </summary>
    public enum LastBatchKind : int
    {
        /// <summary>
        /// 最終以外
        /// </summary>
        None = 0,
        /// <summary>
        /// 最終
        /// </summary>
        Last = 1,
    }

    /// <summary>
    /// 浸漬タンクNO（9211、9212、9311、9312、9411、9511）
    /// ※デバイス値セットのエリア特定に使用
    /// </summary>
    public SharedConst.ImmersionTankNos ImmersionTankNo => string.IsNullOrWhiteSpace(ImmersionTankNoChar) == true
                ? SharedConst.ImmersionTankNos.None
                : Enum.IsDefined(typeof(SharedConst.ImmersionTankNos), int.Parse(ImmersionTankNoChar)) == true
                ? (SharedConst.ImmersionTankNos)int.Parse(ImmersionTankNoChar)
                : SharedConst.ImmersionTankNos.None;
}
