namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// コーナー別仕分ViewModel
    /// </summary>
    public class StepItemSortingByCornersViewModel : BaseViewModel
    {
        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;
        //----
        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;
        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;
        //---選択カード
        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>倉庫配送先</summary>
        public string Delivery { get; set; } = string.Empty;
        /// <summary>加工場名表示</summary>
        public string ProcPlantFlag { get; set; } = string.Empty;
        /// <summary>加工場名</summary>
        public string ProcPlantName { get; set; } = string.Empty;
        /// <summary>先コーナー</summary>
        public string Corner { get; set; } = string.Empty;
        /// <summary>等階級</summary>
        public string GradeClass { get; set; } = string.Empty;
        /// <summary>出庫ケース数</summary>
        public string ShipCase { get; set; } = string.Empty;
        /// <summary>出庫バラ数</summary>
        public string ShipBara { get; set; } = string.Empty;
        //---
        /// <summary>入荷-明細No.</summary>
        public string ArrivalNo { get; set; } = string.Empty;

        /// <summary>出荷者</summary>
        public string Shipper { get; set; } = string.Empty;

        /// <summary>産地</summary>
        public string ProductArea { get; set; } = string.Empty;
        /// <summary>荷姿</summary>
        public string Packing { get; set; } = string.Empty;
        /// <summary>入数</summary>
        public string PackingQuantity { get; set; } = string.Empty;
        /// <summary>仕分入力ケース数</summary>
        public string SortingCase { get; set; } = string.Empty;
        /// <summary>仕分入力バラ数</summary>
        public string SortingBara { get; set; } = string.Empty;
        /// <summary>引当済ケース数</summary>
        public string ApplyCase { get; set; } = string.Empty;
        /// <summary>引当済バラ数</summary>
        public string ApplyBara { get; set; } = string.Empty;
        /// <summary>荷印</summary>
        public string PackingMark { get; set; } = string.Empty;
        /// <summary>説明</summary>
        public string Explanation { get; set; } = string.Empty;
        /// <summary>摘要</summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>コーナー名</summary>
        public string CornerName { get; set; } = string.Empty;

        /// <summary>出荷予定集約ID</summary>
        public string ShippingAggrId { get; set; } = string.Empty;

        //----
        /// <summary>コーナーパレットNo.</summary>
        public string SortingPalletNo { get; set; } = string.Empty;
        /// <summary>倉庫配送先</summary>
        public string SortingDelivery { get; set; } = string.Empty;
        public string SortingCorner { get; set; } = string.Empty;

        /// <summary>倉庫CD</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ゾーンCD</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>倉庫配送先コード</summary>
        public string DeliveryCode { get; set; } = string.Empty;
        /// <summary>加工場コード</summary>
        public string ProcPlantCode { get; set; } = string.Empty;
    }
}
