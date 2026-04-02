namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// パレット詰合せViewModel
    /// </summary>
    public class StepItemPickingTargetSelectViewModel : BaseViewModel
    {
        /// <summary>倉庫コード</summary>
        public string AreaCd { get; set; } = string.Empty;
        ///// <summary>倉庫名</summary>
        public string AreaNm { get; set; } = string.Empty;

        /// <summary>ゾーンコード</summary>
        public string ZoneCd { get; set; } = string.Empty;
        ///// <summary>ゾーン名</summary>
        public string ZoneNm { get; set; } = string.Empty;
    }
}
