namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// 店別仕分【摘取】ViewModel
    /// </summary>
    public class StepItemSortingByStoreDeliveryViewModel : BaseViewModel
    {
        /// <summary>取引先コード<summary>
        public string CustomerCd { get; set; } = string.Empty;
        /// <summary>納品先<summary>
        public string DeliveryCd { get; set; } = string.Empty;
        public string Delivery { get; set; } = string.Empty;

        //---
        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;

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

        /// <summary> 入数</summary>
        public string PackingQuantity { get; set; } = string.Empty;
        /// <summary>仕分ケース/バラ<summary>
        public string CaseBara { get; set; } = string.Empty;
        /// <summary>仕分済ケース/バラ<summary>
        public string SortingCaseBara { get; set; } = string.Empty;

        //----
        /// <summary>等階級</summary>
        public string GradeClass { get; set; } = string.Empty;

        /// <summary>ケース指示数<summary>
        public string InstCase { get; set; } = string.Empty;
        /// <summary>バラ指示数<summary>
        public string InstBara { get; set; } = string.Empty;

        /// <summary>ケース指示数<summary>
        public string SumiCase { get; set; } = string.Empty;
        /// <summary>バラ指示数<summary>
        public string SumiBara { get; set; } = string.Empty;

        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;

        /// <summary>店別仕分指示ID</summary>
        public string StoreSortingID { get; set; } = string.Empty;
    }
}
