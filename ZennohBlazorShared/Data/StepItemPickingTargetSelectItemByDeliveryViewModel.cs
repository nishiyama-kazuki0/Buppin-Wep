namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// ピッキング【倉庫配送先】/倉庫配送先別ViewModel
    /// </summary>
    public class StepItemPickingTargetSelectItemByDeliveryViewModel : BaseViewModel
    {
        /// <summary>検索用倉庫配送先コード</summary>
        public string SearchDeliveryCd { get; set; } = string.Empty;

        /// <summary>倉庫配送先</summary>
        public string Delivery { get; set; } = string.Empty;

        /// <summary>倉庫配送先コード</summary>
        public string DeliveryCd { get; set; } = string.Empty;
        /// <summary>倉庫配送先名</summary>
        public string DeliveryNm { get; set; } = string.Empty;

        /// <summary>倉庫</summary>
        public string Area { get; set; } = string.Empty;

        /// <summary>倉庫コード</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>倉庫名</summary>
        public string AreaNm { get; set; } = string.Empty;

        /// <summary>ゾーン</summary>
        public string Zone { get; set; } = string.Empty;

        /// <summary>ゾーンコード</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>ゾーン名</summary>
        public string ZoneNm { get; set; } = string.Empty;

    }
}
