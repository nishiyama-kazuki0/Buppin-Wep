using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// 解凍機受渡判定２VIEWモデル<br/>
/// ※条件なしで１件目の解凍パターンを取得する<br/>
/// ※解凍機へ搬送するコンベア上に１つもない場合に参照するVIEW<br/>
///   ・次のデパレにて解凍機へ搬送する予定の原料を対象に抽出<br/>
///   ・解凍機前に到着している1つ目の原料の解凍パターンと異なる、または無い場合は解凍機へ１つ目のみ渡す（奇数対応）<br/>
/// </summary>
public class VW_MATERIALNO_THAWPATTERN2_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_MATERIALNO_THAWPATTERN2";

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
