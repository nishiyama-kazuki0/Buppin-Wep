namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// パレット詰合せViewModel
/// </summary>
public class StepItemPalletInventoryInquiryViewModel : BaseViewModel
{
    /// <summary>遷移元パレットNo</summary>
    public string MotoPalletNo { get; set; } = string.Empty;

    /// <summary>パレットNo</summary>
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>入荷明細No</summary>
    public string ArrivalDetailNo { get; set; } = string.Empty;
    //----
    /// <summary>混載</summary>
    public bool IsMixed { get; set; } = false;
    public string Mixed { get; set; } = string.Empty;
    /// <summary>倉庫</summary>
    public string Area { get; set; } = string.Empty;
    /// <summary>ソーン</summary>
    public string Zone { get; set; } = string.Empty;
    /// <summary>ロケ</summary>
    public string Location { get; set; } = string.Empty;
    /// <summary>状態</summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>割付コーナー</summary>
    public string AllocCorner { get; set; } = string.Empty;

    /// <summary>入荷-明細No</summary>
    public string ArrivalDetailNoShow { get; set; } = string.Empty;
    /// <summary>品名</summary>
    public string ProductName { get; set; } = string.Empty;
    /// <summary>等級</summary>
    public string Grade { get; set; } = string.Empty;
    /// <summary>階級</summary>
    public string Class { get; set; } = string.Empty;
    /// <summary>ケース数</summary>
    public string Case { get; set; } = string.Empty;
    /// <summary>バラ数</summary>
    public string Bara { get; set; } = string.Empty;

    /// <summary>倉庫配送先コード</summary>
    public string DeliveryCd { get; set; } = string.Empty;
    /// <summary>倉庫配送先名</summary>
    public string DeliveryNm { get; set; } = string.Empty;
    /// <summary>引当済ケース数</summary>
    public string ApplyCase { get; set; } = string.Empty;
    /// <summary>引当済バラ数</summary>
    public string ApplyBara { get; set; } = string.Empty;
    /// <summary>CARD_LIST_KEY</summary>
    public string CardListKey { get; set; } = string.Empty;

    //----
    /// <summary>等階級</summary>
    public string GradeClass { get; set; } = string.Empty;
    /// <summary>出荷者</summary>
    public string Shipper { get; set; } = string.Empty;
    /// <summary>産地</summary>
    public string ProductArea { get; set; } = string.Empty;
    /// <summary>荷姿</summary>
    public string Packing { get; set; } = string.Empty;
    /// <summary>入数</summary>
    public string PackingQuantity { get; set; } = string.Empty;
    /// <summary>荷印</summary>
    public string PackingMark { get; set; } = string.Empty;
    /// <summary>説明</summary>
    public string Explanation { get; set; } = string.Empty;
    /// <summary>エチレン区分</summary>
    public string Ethylene { get; set; } = string.Empty;
    /// <summary>温度帯</summary>
    public string TempRange { get; set; } = string.Empty;

    /// <summary>特管品</summary>
    public string SpecialType { get; set; } = string.Empty;

    /// <summary>呼出元</summary>
    public new string Caller { get; set; } = string.Empty;
}
