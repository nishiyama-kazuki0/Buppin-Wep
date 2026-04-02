using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// AGV搬送要求VIEWモデル
/// </summary>
public class VW_CARRYREQUEST_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_CARRYREQUEST_INFO";

    /// <summary>
    /// タスクID
    /// </summary>
    [Column("タスクID")]
    public int ReqTaskID { get; set; } = 0;

    /// <summary>
    /// 指示時間年月日時分(yyyyMMddHHmm)
    /// </summary>
    [Column("指示時間")]
    public string ReqTime { get; set; } = string.Empty;

    /// <summary>
    /// 取ステーション番号
    /// </summary>
    [Column("取ステーション番号")]
    public string ReqFromPosString { get; set; } = string.Empty;

    /// <summary>
    /// 置ステーション番号
    /// </summary>
    [Column("置ステーション番号")]
    public string ReqToPosString { get; set; } = string.Empty;

    /// <summary>
    /// 指定ゾーン区分
    /// </summary>
    [Column("指定ゾーン区分")]
    public ReqZoneKind ReqZoneType { get; set; } = ReqZoneKind.None;

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
    /// 生産タスク管理ID
    /// </summary>
    [Column("生産タスク管理ID")]
    public string ProductionTaskId { get; set; } = string.Empty;

    /// <summary>
    /// AGV_CODE
    /// </summary>
    [Column("AGV_CODE")]
    public string AgvCode { get; set; } = string.Empty;

    // TODO AGV搬送要求の進行状況は管理する想定
    /// <summary>
    /// 搬送進行状況（0：予約／1：移動中／2：移動済）
    /// 搬送要求のタスクIDに対する状況
    /// </summary>
    [Column("進行状況")]
    public StateKind StateType { get; set; } = StateKind.Reserve;

    /// <summary>
    /// 到着時反転（0：通常(反転しない)／1：反転(反転する)）
    /// TO到着時のAGV方向
    /// </summary>
    [Column("到着時反転")]
    public int AgvReverse { get; set; } = 0;

    /// <summary>
    /// 指定ゾーン区分
    /// （AGVへの指示切り分け）
    /// </summary>
    public enum ReqZoneKind
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 原料供給エリア
        /// </summary>
        MaterialSupply = 1,
        /// <summary>
        /// 専用容器エリア
        /// </summary>
        Dedicated = 2,
    }

    /// <summary>
    /// 搬送進行状況
    /// </summary>
    public enum StateKind
    {
        /// <summary>
        /// 0：予約
        /// </summary>
        Reserve = 0,
        /// <summary>
        /// 1：移動中
        /// </summary>
        Processing = 1,
        /// <summary>
        /// 2：移動済
        /// </summary>
        Done = 2,
    }

    /// <summary>
    /// 指示時間(日付)-年月日時分へ変換格納プロパティ
    /// </summary>
    public DateTime ReqTimeDate
    {
        set => ReqTime = value.ToString("yyyyMMddHHmm");
    }

    /// <summary>
    /// 指示時間年(yyyy)
    /// </summary>
    public short ReqTimeYear => string.IsNullOrEmpty(ReqTime) || ReqTime.Length < 4
                ? (short)0
                : short.TryParse(ReqTime[..4], out short year) ? year : (short)0;

    /// <summary>
    /// 指示時間月日(MMdd)
    /// </summary>
    public short ReqTimeMonthDay => string.IsNullOrEmpty(ReqTime) || ReqTime.Length < 8
                ? (short)0
                : short.TryParse(ReqTime.Substring(4, 4), out short monthDay) ? monthDay : (short)0;

    /// <summary>
    /// 指示時間時分(HHmm)
    /// </summary>
    public short ReqTimeHourMinute => string.IsNullOrEmpty(ReqTime) || ReqTime.Length < 12
                ? (short)0
                : short.TryParse(ReqTime.Substring(8, 4), out short hourMinute) ? hourMinute : (short)0;
    /// <summary>
    /// 取ステーション番号
    /// </summary>
    public int ReqFromPos
    {
        get => string.IsNullOrWhiteSpace(ReqFromPosString) ? 0 : int.Parse(ReqFromPosString);
        set => ReqFromPosString = value == 0 ? string.Empty : value.ToString();
    }

    /// <summary>
    /// 置ステーション番号
    /// </summary>
    public int ReqToPos
    {
        get => string.IsNullOrWhiteSpace(ReqToPosString) ? 0 : int.Parse(ReqToPosString);
        set => ReqToPosString = value == 0 ? string.Empty : value.ToString();
    }

    /// <summary>
    /// AGV_CODE
    /// </summary>
    public short AgvCodeNumber
    {
        get => (short)(string.IsNullOrWhiteSpace(AgvCode) ? 0 : int.Parse(AgvCode));
        set => AgvCode = value == 0 ? string.Empty : value.ToString();
    }
}
