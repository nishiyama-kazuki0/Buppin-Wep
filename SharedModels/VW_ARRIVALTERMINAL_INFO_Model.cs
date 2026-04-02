namespace SharedModels;

/// <summary>
/// 終端到着情報内容VIEWモデル
/// </summary>
public class VW_ARRIVALTERMINAL_INFO_Model : VW_DEPALLETIZE_INFO_Model
{
    //デパレ連携情報内容VIEWモデルのプロパティ＋下記プロパティ
    //コンベア終端へ到着した原材料情報(CV管理PLCより取得)

    /// <summary>
    /// デパレカウントNO
    /// </summary>
    public short DepalletizeCountNo { get; set; } = 0;

}
