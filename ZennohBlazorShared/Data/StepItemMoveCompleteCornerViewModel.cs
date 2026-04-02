namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// コーナー搬送ViewModel
    /// </summary>
    public class StepItemMoveCompleteCornerViewModel : BaseViewModel
    {
        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;
        //----
        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;
        /// <summary>倉庫配送先</summary>
        public string Delivery { get; set; } = string.Empty;
        /// <summary>倉庫配送コード</summary>
        public string DeliveryNo { get; set; } = string.Empty;
        /// <summary>加工場名表示</summary>
        public string ProcPlantFlag { get; set; } = string.Empty;
        /// <summary>加工場名</summary>
        public string ProcPlantName { get; set; } = string.Empty;
        /// <summary>加工場コード</summary>
        public string ProcPlantNo { get; set; } = string.Empty;
        /// <summary>コーナー</summary>
        public string Corner { get; set; } = string.Empty;

    }
}
