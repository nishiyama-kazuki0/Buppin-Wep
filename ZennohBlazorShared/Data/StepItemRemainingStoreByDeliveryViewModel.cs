namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// 残格納(倉庫配送先別)ViewModel
    /// </summary>
    public class StepItemRemainingStoreByDeliveryViewModel : BaseViewModel
    {
        //--- 倉庫別からの引継ぎ
        /// <summary>倉庫配送先ｺｰﾄﾞ</summary>
        public string DeliveryCd { get; set; } = string.Empty;
        /// <summary>倉庫ｺｰﾄﾞ</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ｿﾞｰﾝｺｰﾄﾞ</summary>
        public string ZoneCd { get; set; } = string.Empty;

        /// <summary>パレットNo</summary>
        public string PalletNo { get; set; } = string.Empty;

        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;

        /// <summary>ﾋﾟｯｸ数</summary>
        public string Pick { get; set; } = string.Empty;
        /// <summary>残数</summary>
        public string Remain { get; set; } = string.Empty;

        /// <summary>指示ｹｰｽ数</summary>
        public string SijiCase { get; set; } = string.Empty;
        /// <summary>指示ﾊﾞﾗ数</summary>
        public string SijiBara { get; set; } = string.Empty;
        /// <summary>残ｹｰｽ数</summary>
        public string ZanCase { get; set; } = string.Empty;
        /// <summary>残ﾊﾞﾗ数</summary>
        public string ZanBara { get; set; } = string.Empty;

        /// <summary>先パレットNo</summary>
        public string SPalletNo { get; set; } = string.Empty;
    }
}
