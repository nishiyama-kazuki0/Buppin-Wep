using System.Data;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ExpressionDBBlazorShared.Data;

namespace ExpressionDBBlazorShared.Data
{
    /// <summary>
    /// パレット移動ViewModel
    /// </summary>
    public class StepItemMovePalletViewModel : BaseViewModel
    {
        /// <summary>パレットNo</summary>
        public string PalletNo { get; set; } = string.Empty;//西山 追加

        public string 管理番号 { get; set; } = string.Empty; 

        public string 管理責任者 { get; set; } = string.Empty;//西山 追加

        public string 社員コード { get; set; } = string.Empty;

        public string 部署 { get; set; } = string.Empty;//西山 追加

        public string 部 { get; set; } = string.Empty;//西山 追加

        public string 課 { get; set; } = string.Empty;//西山 追加

        public string 仕掛番号 { get; set; } = string.Empty;//西山 追加

        public string プロジェクト名 { get; set; } = string.Empty;//西山 追加

        public string 内容 { get; set; } = string.Empty;//西山 追加

         public string 保管開始日 { get; set; } = string.Empty;//西山 追加

        public DateTime? 保管開始日2 { get; set; }

        public DateTime? 保管終了日2 { get; set; }

       public string 保管終了日 { get; set; } = string.Empty;//西山 追加

        public string 棚 { get; set; } = string.Empty;//西山 追加

        public string 棚番 { get; set; } = string.Empty;//西山 追加

        public string 元棚 { get; set; } = string.Empty;

        public string 移動先棚 { get; set; } = string.Empty;

        public string 状態 { get; set; } = string.Empty;

        public string 備考 { get; set; } = string.Empty;


        public string 場所 { get; set; } = string.Empty;

        public string 階 { get; set; } = string.Empty;

        /// <summary>倉庫コード</summary>
        public string AreaCd { get; set; } = string.Empty;
        /// <summary>ゾーンコード</summary>
        public string ZoneCd { get; set; } = string.Empty;
        /// <summary>棚番</summary>
        public string LocationCd { get; set; } = string.Empty;

        /// <summary>棚番</summary>
        public string ShelfCd { get; set; } = string.Empty;
        //----
        /// <summary>混載</summary>
        public bool IsMixed { get; set; } = false;
        public string Mixed { get; set; } = string.Empty;
        //----
        /// <summary>アラート</summary>
        public bool IsAlert { get; set; } = false;
        public string Alert { get; set; } = string.Empty;
        /// <summary>アラートタイトル</summary>
        public string AlertTitle { get; set; } = string.Empty;
        /// <summary>アラートテキスト</summary>
        public string AlertText { get; set; } = string.Empty;
        //----
        /// <summary>倉庫</summary>
        public string OutAreaCd { get; set; } = string.Empty;
        /// <summary>ゾーン</summary>
        public string OutZoneCd { get; set; } = string.Empty;
        //----
        /// <summary>高荷推奨ロケ</summary>
        public string RecomLocationH { get; set; } = string.Empty;
        /// <summary>中荷推奨ロケ</summary>
        public string RecomLocationM { get; set; } = string.Empty;
        /// <summary>低荷推奨ロケ</summary>
        public string RecomLocationL { get; set; } = string.Empty;
        //----
        /// <summary>品名</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>等階級</summary>
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
        public string SpecialType { get; set; } = string.Empty;
        /// <summary>エチレン区分</summary>
        public string Ethylene { get; set; } = string.Empty;
        /// <summary>温度帯</summary>
        public string TempRange { get; set; } = string.Empty;
        /// <summary>ケース数</summary>
        public string Case { get; set; } = string.Empty;
        /// <summary>バラ数</summary>
        public string Bara { get; set; } = string.Empty;




        ///// <summary>階級</summary>
        //public string Class { get; set; } = string.Empty;

        /// <summary>摘取ピックの先パレットNo</summary>
        public string SPalletNo { get; set; } = string.Empty;

        public bool IsZanKakuno { get; set; } = false;
        public bool IsCorner { get; set; } = false;


        public StepItemMovePalletViewModel()
        {
            管理責任者 = "";
            社員コード = "";
            部署 = "";
        }
    }
}
