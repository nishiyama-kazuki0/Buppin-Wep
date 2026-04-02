namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// 店別仕分【種まき】ViewModel
    /// </summary>
    public class StepItemSortingByStoreViewModel : BaseViewModel
    {
        /// <summary>取引先コード</summary>
        public string CustomerCd { get; set; } = string.Empty;

        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;
        //----

        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>品名コード</summary>
        public string ProductCd { get; set; } = string.Empty;

        /// <summary>産地コード</summary>
        public string ProductAreaCd { get; set; } = string.Empty;
        /// <summary>出荷者コード</summary>
        public string ShipperCd { get; set; } = string.Empty;

        /// <summary>産地/等階級</summary>
        public string ProductArea_GradeClass { get; set; } = string.Empty;
        //----
        ///// <summary>等級</summary>
        //public string Grade { get; set; } = string.Empty;
        ///// <summary>階級</summary>
        //public string Class { get; set; } = string.Empty;

        /// <summary>等階級</summary>
        public string GradeClass { get; set; } = string.Empty;


        /// <summary>納品先<summary>
        public string Delivery { get; set; } = string.Empty;
        /// <summary>納品先コード<summary>
        public string DeliveryCd { get; set; } = string.Empty;

        /// <summary> 入数</summary>
        public string PackingQuantity { get; set; } = string.Empty;

        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;

        /// <summary>ケース指示数</summary>
        public string SijiCase { get; set; } = string.Empty;
        /// <summary>バラ指示数</summary>
        public string SijiBara { get; set; } = string.Empty;

        /// <summary>ケース済数</summary>
        public string SumiCase { get; set; } = string.Empty;
        /// <summary>バラ済数</summary>
        public string SumiBara { get; set; } = string.Empty;

        /// <summary>店別仕分指示ID</summary>
        public string StoreSortingID { get; set; } = string.Empty;

        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;


    }
}
