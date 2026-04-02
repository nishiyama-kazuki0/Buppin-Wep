namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// コーナー割付ViewModel
    /// </summary>
    public class StepItemCornerAllocationsViewModel : BaseViewModel
    {
        /// <summary>倉庫配送先コード</summary>
        public string DeliveryCd { get; set; } = string.Empty;

        /// <summary>倉庫配送先</summary>
        public string Delivery { get; set; } = string.Empty;
        /// <summary>コーナーID</summary>
        public string CornerId { get; set; } = string.Empty;
        /// <summary>コーナー</summary>
        public string Corner { get; set; } = string.Empty;
        //---
        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;

        //----
        /// <summary>アラート</summary>
        public bool IsAlert { get; set; } = false;
        public string Alert { get; set; } = string.Empty;
        /// <summary>アラートタイトル</summary>
        public string AlertTitle { get; set; } = string.Empty;
        /// <summary>アラートテキスト</summary>
        public string AlertText { get; set; } = string.Empty;

        /// <summary>倉庫コード</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ゾーンコード</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>ロケーションコード</summary>
        public string LocationCd { get; set; } = string.Empty;

        /// <summary>倉庫名</summary>
        public string AreaNm { get; set; } = string.Empty;
        /// <summary>ゾーン名</summary>
        public string ZoneNm { get; set; } = string.Empty;
        /// <summary>ロケーション名</summary>
        public string LocationNm { get; set; } = string.Empty;
    }
}
