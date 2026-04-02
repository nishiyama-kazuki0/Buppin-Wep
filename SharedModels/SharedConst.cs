using System.Text;

namespace SharedModels;

public class SharedConst//西山
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
    /// 西山
    /// </summary>
    public const string KEY_PROGRAM_NAME_MANAGEMENT_ID = "管理ID取得";

    public enum WorkCategory : int
    {
        NyukaUketuke = 11,      // 入荷受付
        NyukaKenpin = 12,       // 入荷検品
        YoteigaiNyuko = 16,     // 予定外入庫
    }

    /// <summary>
    /// 保管区分
    /// </summary>
    public enum StorageKind : int
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 1：常温
        /// </summary>
        Dry = 1,
        /// <summary>
        /// 2：冷凍
        /// </summary>
        Frozen = 2,
    }

    /// <summary>
    /// 浸漬タンクNO
    /// </summary>
    public enum ImmersionTankNos : int
    {
        /// <summary>
        /// なし
        /// </summary>
        None = 0,
        /// <summary>
        /// 浸漬タンクNO（9211）
        /// </summary>
        Tank9211 = 9211,
        /// <summary>
        /// 浸漬タンクNO（9212）
        /// </summary>
        Tank9212 = 9212,
        /// <summary>
        /// 浸漬タンクNO（9311）
        /// </summary>
        Tank9311 = 9311,
        /// <summary>
        /// 浸漬タンクNO（9312）
        /// </summary>
        Tank9312 = 9312,
        /// <summary>
        /// 浸漬タンクNO（9411）
        /// </summary>
        Tank9411 = 9411,
        /// <summary>
        /// 浸漬タンクNO（9511）
        /// </summary>
        Tank9511 = 9511,
    }

    /// <summary>
    /// デバイスタイプ定義
    /// userAgentで定義されるとする
    /// </summary>
    public enum TYPE_DEVICE_TYPE : int
    {
        NONE = 0,
        KEYENCE_DEVICE = 1,
        CASIO_DEVICE = 2,
        DENSOWAVE_DEVICE = 3,
        ANDROID = 4,
        IOS_DEVICE = 5,
        MAC_OS = 6,
        LINUX_PC = 7,
        WINDOWS_PC = 8,
    }
    public enum TYPE_DEVICE_TYPE_GROUP : int
    {
        NONE = 0,
        PC = 1,
        HT = 2,
    }

    public enum TYPE_LOGGER : int
    {
        NONE = 0,
        INFO = 1,
        WARM = 2,
        FATAL = 3,
    }

    public enum TYPE_DB_TYPE : int
    {
        SQL = 0,
        ORACLE = 1,
    }

    /// <summary>
    /// 通知区分 DEFINE_NOTIFY_CATEGORIES
    /// </summary>
    public enum TYPE_NOTIFY_CATEGORY : int
    {
        NONE = 0,
        INFO = 1,
        WARNING = 2,
        DANGER = 3,
    }

    /// <summary>
    /// 文字数定義
    /// ※基本HT機能で使用します
    /// 西山
    /// １：バーコード桁数指定
    /// </summary>
    public const int LEN_ZONE_ID = 2;               // ゾーンNO
    public const int LEN_LOCATION_ID = 8;           // ロケーションNO
    public const int LEN_CAR_NUMBER = 8;            // 車番
    public const int LEN_NYUKA_NO = 6;              // 入荷NO
    public const int LEN_NYUKA_MEISAI_NO = 9;       // 入荷明細NO
    public const int LEN_PALLET_NO = 50;             // パレットNO//TextBoxの桁数制限

    public const int 文字数制限 = 150;




    public const int LEN_CASE = 6;                  // ケース数
    public const int LEN_BARA = 6;                  // バラ数
    public const int LEN_SOBARA = 6;                // 総バラ数

    public const int LEN_PALLET_NO_BARCODE = 1;     // パレットNO(バーコード桁数)
    public const int LEN_PALLET_NO_BARCODE2 = 2;    // パレットNO(バーコード桁数)2
    public const int LEN_PALLET_NO_BARCODE3 = 13;    // パレットNO(バーコード桁数)3
    public const int LEN_PALLET_NO_BARCODE4 = 19;    //　今テスト的に追加している 

    public const int LEN_DELIVER_CD = 6;            // 倉庫配送先コード

    public const string STR_BODY_ID = "contentId_body";//JSでBody指定用のコントロールID

    public const string STR_SESSIONSTORAGE_メニュー遷移 = "メニュー遷移";
    //public const string STR_SESSIONSTORAGE_メニュー遷移 = "Mst_ShelfTest";


    public const string STR_LOCALSTORAGE_遷移履歴 = "遷移履歴";
    public const string STR_LOCALSTORAGE_遷移画面 = "遷移画面";
    public const string STR_LOCALSTORAGE_MANEGEMENT_ID = "入荷検品管理ID";
    public const string STR_LOCALSTORAGE_PALLETE_NO = "パレットNo";//
    //public const string STR_LOCALSTORAGE_PALLETE_NO = "ID";//このパレットNOと照らし合わせる?


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

    //
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

    public const string STR_VARIDATE_NUM = @"^([1-9]\d*|0)?$";//数値の正規表現　HtTitleValueTextBoxなどのRegexPatternに与える
    public const string STR_VARIDATE_PALLETE_NO = @"^[0-9a-zA-Z]{5}$|^[0-9a-zA-Z]{9}$";//パレットNo対象のみの正規表現(英数5桁または9桁)　HtTitleValueTextBoxなどのRegexPatternに与える
    public DateTime STR_VARIDATE_PALLETE_NO2 = DateTime.Now;//パレットNo対象のみの正規表現(英数5桁または9桁)　HtTitleValueTextBoxなどのRegexPatternに与える



    // ゼロサプレスを適応列判定用
    // ※DEFINE_COMPONENT_COLUMNSのFORMAT_STRINGに設定すればゼロサプレイス対象の列となります。
    //ゼロサプレスとは…数値を文字列として表示する時に、不要な０を取り除く処理
    public const string FORMAT_ZERO_SUPPRESS = "ZERO_SUPPRESS";

    /// <summary>
    /// セパレートファイル情報
    /// </summary>
    /// <remarks>
    /// Export、取込機能は下記の定義を元にセパレートファイルを出力、取込します。
    /// Extensionはピリオド(.)なしを定義してください。
    /// </remarks>
    public static readonly List<SeparatedFileInfo> SeparatedFileInfos
        =
        [
            new SeparatedFileInfo(){ Name="CSV", Extension="csv", Delimiter=",", FileEncoding = Encoding.GetEncoding("shift_jis") },
            new SeparatedFileInfo(){ Name="TSV", Extension="tsv", Delimiter="\t", FileEncoding =  Encoding.GetEncoding("shift_jis")},
        ];
}
