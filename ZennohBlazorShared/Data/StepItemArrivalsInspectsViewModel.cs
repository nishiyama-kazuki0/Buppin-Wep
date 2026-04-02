namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// 入荷予定ViewModel
    /// </summary>
    public class StepItemArrivalsInspectsViewModel : BaseViewModel
    {
        /// <summary>倉庫コード</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ゾーンコード</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>ロケーションコード</summary>
        public string LocationCd { get; set; } = string.Empty;

        /// <summary>車番</summary>
        public string CarNumber { get; set; } = string.Empty;
        /// <summary>入荷-明細No</summary>
        public string ArrivalDetailNo { get; set; } = string.Empty;
        /// <summary>入荷No</summary>
        public string ArrivalNo { get; set; } = string.Empty;
        /// <summary>入荷No検索用</summary>
        public string ArrivalNoSearch { get; set; } = string.Empty;
        /// <summary>明細No</summary>
        public string DetailNo { get; set; } = string.Empty;
        /// <summary>入荷-明細No</summary>
        public string ArrivalDetailNoShow { get; set; } = string.Empty;
        /// <summary>済残</summary>
        public string Remain { get; set; } = string.Empty;
        /// <summary>出荷指示数量</summary>
        public string ShipQuantity { get; set; } = string.Empty;
        //-------------------------------------------
        /// <summary>ケース数</summary>
        public string Case = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara = string.Empty;

        //-------------------------------------------

        /// <summary>入荷検品指示ケース数(指示母数のケース数)</summary>
        public string InspectCase { get; set; } = string.Empty;
        /// <summary>入荷検品指示バラ数(指示母数のバラ数)</summary>
        public string InspectBara { get; set; } = string.Empty;

        /// <summary>未検品ケース数(未検品のケース数)</summary>
        public string RemainCase { get; set; } = string.Empty;
        /// <summary>未検品バラ数(未検品のバラ数)</summary>
        public string RemainBara { get; set; } = string.Empty;

        /// <summary>出庫ケース数</summary>
        public string ShipCase { get; set; } = string.Empty;
        /// <summary>出庫バラ数</summary>
        public string ShipBara { get; set; } = string.Empty;
        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>等級-階級</summary>
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
        /// <summary>特管品</summary>
        public string SpeciallyItem { get; set; } = string.Empty;
        /// <summary>エチレン</summary>
        public string Ethylene { get; set; } = string.Empty;

        public bool IsDetail { get; set; } = true;

        /// <summary>入荷検品管理ID</summary>
        public string ArrivalManagementId { get; set; } = string.Empty;


        /// <summary>車番</summary>
        public string CarNumberDisp { get; set; } = string.Empty;

        /// <summary>入荷-明細No（表示用)</summary>
        public string ArrivalDetailNoDisp { get; set; } = string.Empty;
        /// <summary>受付No</summary>
        public string ReceptNo { get; set; } = string.Empty;
    }
}
