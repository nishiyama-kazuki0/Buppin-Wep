namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// 摘取ピック(倉庫配送先別)ViewModel
    /// </summary>
    public class StepItemPickingItemByDeliveryViewModel : BaseViewModel
    {
        //----一覧
        /// <summary>先パレットNo</summary>
        public string SPalletNo { get; set; } = string.Empty;

        /// <summary>指示ｹｰｽ数</summary>
        public string SijiCase { get; set; } = string.Empty;
        /// <summary>指示ﾊﾞﾗ数</summary>
        public string SijiBara { get; set; } = string.Empty;
        /// <summary>残ｹｰｽ数</summary>
        public string ZanCase { get; set; } = string.Empty;
        /// <summary>残ﾊﾞﾗ数</summary>
        public string ZanBara { get; set; } = string.Empty;
        /// <summary>入力ケース数</summary>
        public string InCase { get; set; } = string.Empty;
        /// <summary>入力バラ数</summary>
        public string InBara { get; set; } = string.Empty;
        //----

        /// <summary>出荷予定集約ID</summary>
        public string ShippingAggrId { get; set; } = string.Empty;

        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        /// <summary>元パレットNo</summary>
        public string MPalletNo { get; set; } = string.Empty;
        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>品名ｺｰﾄﾞ</summary>
        public string ProductCd { get; set; } = string.Empty;
        /// <summary>等級-階級</summary>
        public string GradeClass { get; set; } = string.Empty;
        /// <summary>倉庫配送先</summary>
        public string DeliveryName { get; set; } = string.Empty;
        /// <summary>加工場名表示</summary>
        public string ProcPlantFlag { get; set; } = string.Empty;
        /// <summary>加工場名</summary>
        public string ProcPlantName { get; set; } = string.Empty;
        /// <summary>加工場コード</summary>
        public string ProcPlantCd { get; set; } = string.Empty;
        /// <summary>産地</summary>
        public string ProductArea { get; set; } = string.Empty;
        /// <summary>荷印</summary>
        public string PackingMark { get; set; } = string.Empty;
        /// <summary>説明</summary>
        public string Explanation { get; set; } = string.Empty;
        /// <summary>摘要</summary>
        public string Abstract { get; set; } = string.Empty;
        /// <summary>入荷-明細No</summary>
        public string ArrivalDetailNoShow { get; set; } = string.Empty;
        /// <summary>入数</summary>
        public string PackingQuantity { get; set; } = string.Empty;
        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;

        /// <summary>倉庫配送先ｺｰﾄﾞ</summary>
        public string DeliveryCd { get; set; } = string.Empty;
        /// <summary>倉庫配送先\nｺｰﾄﾞ</summary>
        public string DeliveryCdTwoLineTitle { get; set; } = string.Empty;
        /// <summary>倉庫ｺｰﾄﾞ</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ｿﾞｰﾝｺｰﾄﾞ</summary>
        public string ZoneCd { get; set; } = string.Empty;
    }
}
