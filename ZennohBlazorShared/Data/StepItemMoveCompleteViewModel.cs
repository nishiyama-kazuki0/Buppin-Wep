namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// コーナー搬送/コーナーパレットNo.読取ViewModel
    /// </summary>
    public class StepItemMoveCompleteViewModel : BaseViewModel
    {
        /// <summary>パレットNo.</summary>
        public string PalletNo { get; set; } = string.Empty;
        //----
        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;
        //--- 入力
        /// <summary>倉庫CD</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ゾーンCD</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>ロケーションCD</summary>
        public string LocationCd { get; set; } = string.Empty;

        //---表示
        /// <summary>現在倉庫</summary>
        public string CurArea { get; set; } = string.Empty;
        /// <summary>現在ゾーン</summary>
        public string CurZone { get; set; } = string.Empty;
        /// <summary>現在ロケ</summary>
        public string CurLocation { get; set; } = string.Empty;

    }
}
