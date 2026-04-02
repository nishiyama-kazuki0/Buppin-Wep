using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 解凍機受渡判定１VIEWモデル<br/>
/// ※原料登録NOを条件に解凍パターンを取得する<br/>
/// ※解凍機へ搬送するコンベア上に原料がある場合に参照するVIEW<br/>
///   ・解凍機へ搬送する冷凍コンベア上にの原料が対象<br/>
///   ・２つ目として解凍機前に到着する原料が1つ目の解凍パターンと同じか判断する<br/>
/// </summary>
public class VW_MATERIALNO_THAWPATTERN1_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_MATERIALNO_THAWPATTERN1";

    /// <summary>
    /// バッチNo
    /// TODO ※抽出時の並び順に必要？不要なら削除
    /// </summary>
    [Column("バッチNO")]
    public string BatchNo { get; set; } = string.Empty;
    /// <summary>
    /// 原料品目コード
    /// TODO ※抽出時の並び順に必要？不要なら削除
    /// </summary>
    [Column("原料品目コード")]
    public string ItemCode { get; set; } = string.Empty;
    /// <summary>
    /// 原料固有No1
    /// TODO ※抽出時の並び順に必要？不要なら削除
    /// </summary>
    [Column("原料固有NO1")]
    public short MaterialSpecificNo1 { get; set; } = 0;
    /// <summary>
    /// 投入フェーズ
    /// TODO ※抽出時の並び順に必要？不要なら削除
    /// </summary>
    [Column("投入フェーズ")]
    public short EntryPhase { get; set; } = 0;
    /// <summary>
    /// バッチ開始年月日(YYYYMMDD)
    /// TODO ※抽出時の並び順に必要？不要なら削除
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

}
