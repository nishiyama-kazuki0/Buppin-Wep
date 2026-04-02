namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// パレット紐付ViewModel
    /// </summary>
    public class StepItemStockupWorkPlansViewModel : BaseViewModel
    {
        /// <summary>入荷-明細No</summary>
        public string ArrivalDetailNo { get; set; } = string.Empty;
        /// <summary>入荷No</summary>
        public string ArrivalNo { get; set; } = string.Empty;
        /// <summary>明細No</summary>
        public string DetailNo { get; set; } = string.Empty;
        /// <summary>入荷-明細No</summary>
        public string ArrivalDetailNoShow { get; set; } = string.Empty;

        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>等級-階級</summary>
        public string GradeClass { get; set; } = string.Empty;
        /// <summary>特別品区分</summary>
        public string SpecialType { get; set; } = string.Empty;
        /// <summary>出荷者</summary>
        public string Shipper { get; set; } = string.Empty;
        /// <summary>産地</summary>
        public string ProductArea { get; set; } = string.Empty;
        /// <summary>荷姿</summary>
        public string Packing { get; set; } = string.Empty;
        /// <summary>荷印</summary>
        public string PackingMark { get; set; } = string.Empty;
        /// <summary>入数</summary>
        public string PackingQuantity { get; set; } = string.Empty;
        /// <summary>説明</summary>
        public string Explanation { get; set; } = string.Empty;
        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;
        /// <summary>紐付ケース数</summary>
        public string LinkCase { get; set; } = string.Empty;
        /// <summary>紐付バラ数</summary>
        public string LinkBara { get; set; } = string.Empty;
        /// <summary>出荷指示数量</summary>
        public string ShipQuantity { get; set; } = string.Empty;
        /// <summary>エチレン区分</summary>
        public string Ethylene { get; set; } = string.Empty;
        /// <summary>温度帯</summary>
        public string TempRange { get; set; } = string.Empty;
        //----------
        /// <summary>呼出元</summary>
        public string Parent { get; set; } = string.Empty;
        /// <summary>入荷検品管理ID</summary>
        public string ArrivalManagementId { get; set; } = string.Empty;
        /// <summary>パレットNo</summary>
        public string PalletNo { get; set; } = string.Empty;
        /// <summary>ケース数入力</summary>
        public string CaseIn { get; set; } = string.Empty;
        /// <summary>バラ数入力</summary>
        public string BaraIn { get; set; } = string.Empty;
        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;

        public bool IsInitParam { get; set; } = false;
    }
}
