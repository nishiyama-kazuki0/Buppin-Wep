using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 端数ステーション到着原料情報VIEWモデル
/// </summary>
public class VW_ARRIVALMATERIAL_FRACTION_INFO_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_ARRIVALMATERIAL_FRACTION_INFO";

    /// <summary>
    /// AGVタスクID
    /// (AGV搬送時のAGVタスクID)
    /// </summary>
    [Column("AGVタスクID")]
    public int AGVTaskID { get; set; } = 0;
    /// <summary>
    /// 保管区分（1：常温/2：冷凍）
    /// 自動倉庫へ戻り搬送指示用（作業後の端数PLを自動倉庫格納する先）
    /// </summary>
    [Column("保管区分")]
    public SharedConst.StorageKind StorageType { get; set; } = SharedConst.StorageKind.None;

    /// <summary>
    /// ［端数作業］バッチNo
    /// </summary>
    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;
    /// <summary>
    /// ［端数作業］原料品目コード
    /// </summary>
    [Column("原料品目コード")]
    public string ItemCode { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫／端数作業］投入フェーズ
    /// </summary>
    [Column("投入フェーズ")]
    public short EntryPhase { get; set; } = 0;
    /// <summary>
    /// ［端数作業］バッチ開始年月日(YYYYMMDD)
    /// </summary>
    [Column("バッチ開始年月日")]
    public string StartBatch { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫］パレットNo
    /// </summary>
    [Column("パレットNO")]
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫／端数作業］原料登録No1(XXXX)
    /// </summary>
    [Column("原料登録NO1")]
    public short MaterialNo1 { get; set; } = 0;
    /// <summary>
    /// ［到着原料在庫／端数作業］積載位置1（1:右／2:左（STを正面に見たとき)）
    /// </summary>
    [Column("積載位置1")]
    public short LoadPosition1 { get; set; } = 0;
    /// <summary>
    /// ［到着原料在庫／端数作業］原料登録No2(XXXX)
    /// </summary>
    [Column("原料登録NO2")]
    public short MaterialNo2 { get; set; } = 0;
    /// <summary>
    /// ［到着原料在庫／端数作業］積載位置2（1:右／2:左（STを正面に見たとき)）
    /// </summary>
    [Column("積載位置2")]
    public short LoadPosition2 { get; set; } = 0;
    /// <summary>
    /// ［到着原料在庫］専用容器積載数
    /// </summary>
    [Column("専用容器積載数")]
    public short DedicatedLoadQTY { get; set; } = 0;
    /// <summary>
    /// ［到着原料在庫］積載位置1_専用容器QR
    /// </summary>
    [Column("専用容器QR1")]
    public string DedicatedQR_P1 { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫］積載位置1_原料品目コード
    /// </summary>
    [Column("原料品目コード1")]
    public string ItemCode_P1 { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫］積載位置1_端数在庫量kg
    /// </summary>
    [Column("端数在庫量1")]
    public double FractionStockWT_P1 { get; set; } = 0.0;
    /// <summary>
    /// ［到着原料在庫］積載位置2_専用容器QR
    /// </summary>
    [Column("専用容器QR2")]
    public string DedicatedQR_P2 { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫］積載位置2_原料品目コード
    /// </summary>
    [Column("原料品目コード2")]
    public string ItemCode_P2 { get; set; } = string.Empty;
    /// <summary>
    /// ［到着原料在庫］積載位置2_端数在庫量kg
    /// </summary>
    [Column("端数在庫量2")]
    public double FractionStockWT_P2 { get; set; } = 0.0;

    /// <summary>
    /// ［到着原料在庫］原料登録No(XXXX)
    /// </summary>
    public short MaterialNo
    {
        get
        {
            if (MaterialNo1 != 0)
            {
                return MaterialNo1;
            }
            else if (MaterialNo2 != 0)
            {
                return MaterialNo2;
            }
            return 0;
        }
    }
    /// <summary>
    /// ［端数作業］バッチ開始年(YYYY)
    /// </summary>
    public short StartBatchYear => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) / 10000);
    /// <summary>
    /// ［端数作業］バッチ開始月(MM)
    /// </summary>
    public short StartBatchMonth => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) / 100 % 100);
    /// <summary>
    /// ［端数作業］バッチ開始日(DD)
    /// </summary>
    public short StartBatchDay => (short)(string.IsNullOrEmpty(StartBatch) ? 0 : int.Parse(StartBatch) % 100);

}
