using System.Text;

namespace SharedModels
{
    public class SharedConst
    {
        public const string KEY_SYSTEM_PARAM = "SYSTEM_PARAM";
        public const string KEY_LOGIN_INFO = "LOGIN_INFO";
        public const string KEY_MENU_INFO = "MENU_INFO";
        public const string KEY_OPERATION_DATE = "OPERATION_DATE";

        public const string KEY_BASE_ID = "BASE_ID";
        public const string KEY_BASE_TYPE = "BASE_TYPE";
        public const string KEY_CONSIGNOR_ID = "CONSIGNOR_ID";

        public const string KEY_USER_ID = "USER_ID";
        public const string KEY_DEVICE_ID = "DEVICE_ID";
        public const string KEY_CLASS_NAME = "CLASS_NAME";

        /// <summary>
        /// 呼び出しプログラム名定義
        /// </summary>
        public const string KEY_PROGRAM_NAME_MANAGEMENT_ID = "管理ID取得";

        public enum workCategory
        {
            NyukaUketuke = 11,      // 入荷受付
            NyukaKenpin = 12,       // 入荷検品
            YoteigaiNyuko = 16,     // 予定外入庫
        }

        /// <summary>
        /// 文字数定義
        /// ※基本HT機能で使用します
        /// </summary>
        public const int LEN_ZONE_ID = 2;               // ゾーンNO
        public const int LEN_LOCATION_ID = 8;           // ロケーションNO
        public const int LEN_CAR_NUMBER = 8;            // 車番
        public const int LEN_NYUKA_NO = 6;              // 入荷NO
        public const int LEN_NYUKA_MEISAI_NO = 9;       // 入荷明細NO
        public const int LEN_PALLET_NO = 9;             // パレットNO
        public const int LEN_CASE = 6;                  // ケース数
        public const int LEN_BARA = 6;                  // バラ数
        public const int LEN_SOBARA = 6;                // 総バラ数
        public const int LEN_PALLET_NO_BARCODE = 5;     // パレットNO(バーコード桁数)
        public const int LEN_PALLET_NO_BARCODE2 = 6;    // パレットNO(バーコード桁数)2
        public const int LEN_PALLET_NO_BARCODE3 = 9;    // パレットNO(バーコード桁数)3
        public const int LEN_DELIVER_CD = 6;            // 倉庫配送先コード

        public const string STR_BODY_ID = "contentId_body";//JSでBody指定用のコントロールID

        public const string STR_SESSIONSTORAGE_メニュー遷移 = "メニュー遷移";

        public const string STR_LOCALSTORAGE_遷移履歴 = "遷移履歴";
        public const string STR_LOCALSTORAGE_遷移画面 = "遷移画面";
        public const string STR_LOCALSTORAGE_MANEGEMENT_ID = "入荷検品管理ID";
        public const string STR_LOCALSTORAGE_PALLETE_NO = "パレットNo";
        public const string STR_LOCALSTORAGE_SPALLETE_NO = "先パレットNo";

        public const string STR_LOCALSTORAGE_DELIVERY_ID = "倉庫配送先コード";
        public const string STR_LOCALSTORAGE_AREA_ID = "倉庫コード";
        public const string STR_LOCALSTORAGE_ZONE_ID = "ゾーンコード";

        public const string STR_LOCALSTORAGE_DELIVERY_NM = "倉庫配送先名";
        public const string STR_LOCALSTORAGE_AREA_NM = "倉庫名";
        public const string STR_LOCALSTORAGE_ZONE_NM = "ゾーン名";

        public const string STR_LOCALSTORAGE_CUT_CONVEY_AREA_ID = "切出倉庫コード";
        public const string STR_LOCALSTORAGE_CUT_CONVEY_ZONE_ID = "切出ゾーンコード";
        public const string STR_LOCALSTORAGE_CUT_CONVEY_LOCATION_ID = "切出ロケーションコード";

        public const string STR_SESSIONSTORAGE_ARRIVAL_AREA_ID = "入庫倉庫コード";
        public const string STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID = "入庫ゾーンコード";
        public const string STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID = "入庫ロケーションコード";
        public const string STR_SESSIONSTORAGE_ARRIVAL_CARNUMBER = "入庫車番";
        public const string STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO = "入庫入荷明細NO";
        public const string STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_NO = "入庫入荷NO";
        public const string STR_SESSIONSTORAGE_ARRIVAL_INCASE = "入庫ケース数";
        public const string STR_SESSIONSTORAGE_ARRIVAL_INBARA = "入庫バラ数";

        // 作業完了または、戻る時に機能に渡す際に使用する※戻り先の機能がローカルストレージに保持しておくこと
        public const string STR_LOCALSTORAGE_SHIP_DELIVERY_ID = "出庫倉庫配送先コード";
        public const string STR_LOCALSTORAGE_SHIP_AREA_ID = "出庫倉庫コード";
        public const string STR_LOCALSTORAGE_SHIP_ZONE_ID = "出庫ゾーンコード";

        // パレット分割Step2の明細ボタン押下時にストレージに保持しておく
        public const string STR_SESSIONSTORAGE_STOCK_ARRIVAL_DETAIL_NO = "在庫入荷明細NO";

        public const string STR_HT_BARA_COLOR_CONTAINS = "ﾊﾞﾗ";
        public const string STR_HT_CASE_PACKING_QUANTITY_CONST = "1";//入数1はケース数のみ入力させたいので判断用

        public const int DEFAULT_NOTIFY_DURATION = 6000;

        public const string STR_VARIDATE_FULL_WIDTH_CHAR_ONLY = @"^[^\x00-\x7F\uFF61-\uFF9F]+$";//全角のみの正規表現　備考入力などに使用
        public const string STR_VARIDATE_NUM = @"^([1-9]\d*|0)?$";//数値の正規表現　HtTitleValueTextBoxなどのRegexPatternに与える
        public const string STR_VARIDATE_PALLETE_NO = @"^[0-9a-zA-Z]{5}$|^[0-9a-zA-Z]{9}$";//パレットNo対象のみの正規表現(英数5桁または9桁)　HtTitleValueTextBoxなどのRegexPatternに与える

        // ゼロサプレスを適応列判定用
        // ※DEFINE_COMPONENT_COLUMNSのFORMAT_STRINGに設定すればゼロサプレイス対象の列となります。
        public const string FORMAT_ZERO_SUPPRESS = "ZERO_SUPPRESS";

        /// <summary>
        /// セパレートファイル情報
        /// </summary>
        /// <remarks>
        /// Export、取込機能は下記の定義を元にセパレートファイルを出力、取込します。
        /// Extensionはピリオド(.)なしを定義してください。
        /// </remarks>
        public static readonly List<SeparatedFileInfo> SeparatedFileInfos
            = new()
            {
                new SeparatedFileInfo(){ Name="CSV", Extension="csv", Delimiter=",", FileEncoding = Encoding.GetEncoding("shift_jis") },
                new SeparatedFileInfo(){ Name="TSV", Extension="tsv", Delimiter="\t", FileEncoding =  Encoding.GetEncoding("shift_jis")},
            };
    }
}
