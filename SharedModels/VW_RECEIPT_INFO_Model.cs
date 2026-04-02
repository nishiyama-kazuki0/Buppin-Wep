using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 容器受取情報VIEWモデル
/// </summary>
public class VW_RECEIPT_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    private const string _VIEW_NAME = "VW_RECEIPT_INFO";
    /// <summary>
    /// VIEW名
    /// </summary>
    public static string VIEW_NAME => VW_RECEIPT_INFO_Model._VIEW_NAME;

    // TODO 項目内容及び型等不明
    /// <summary>
    /// バッチNO
    /// </summary>
    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;
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
    /// 原料登録No
    /// </summary>
    [Column("原料登録NO")]
    public short MaterialNo { get; set; } = 0;
    /// <summary>
    /// 専用容器QR
    /// ※実容器受取時はブランク、投入ST時は設定あり
    /// </summary>
    [Column("専用容器QR")]
    public string DedicatedQR { get; set; } = string.Empty;
    /// <summary>
    /// 投入ST（60002：投入ST1、60003：投入ST2）
    /// ※AGV搬送指示の行き先にて使用
    /// </summary>
    [Column("投入ST")]
    public int EntryST { get; set; } = 0;
    /// <summary>
    /// 生産タスク管理ID
    /// ※搬送指示と情報紐づけ用
    /// </summary>
    [Column("生産タスク管理ID")]
    public string ProductionTaskId { get; set; } = string.Empty;
    /// <summary>
    /// 優先順<br/>
    /// </summary>
    [Column("優先順")]
    public long OrderdNo { get; set; } = 0;

    /// <summary>
    /// デバイスより取得した各年月日をバッチ開始年月日へセットする
    /// </summary>
    /// <param name="yyyy">バッチ開始年</param>
    /// <param name="mm">バッチ開始月</param>
    /// <param name="dd">バッチ開始日</param>
    /// <returns>true:正常／false:異常</returns>
    public bool SetStartBatch(short yyyy, short mm, short dd)
    {
        StartBatch = $"{yyyy:D4}{mm:D2}{dd:D2}";
        return true;
    }
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
