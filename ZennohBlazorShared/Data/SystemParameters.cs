using SharedModels;

namespace ZennohBlazorShared.Data
{
    /// <summary>
    /// DEFINE_SYSTEM_PARAMETERSテーブル情報
    /// </summary>
    public class SystemParameters
    {
        /// <summary>
        /// パラメータKEY
        /// </summary>
        public string ParameterKey { get; set; }
        /// <summary>
        /// パラメータKEY名称
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// 値
        /// </summary>
        public string ParameterValue { get; set; }
        public SystemParameters()
        {
            ParameterKey = "";
            KeyName = "";
            ParameterValue = "";
        }
    }

    /// <summary>
    /// DEFINE_SYSTEM_PARAMETERSテーブル情報
    /// </summary>
    public class SystemParameter
    {
        /// <summary>
        /// 支所場コード
        /// </summary>
        public string BaseId { get; set; }
        /// <summary>
        /// 所場区分
        /// </summary>
        public int BaseType { get; set; }
        /// <summary>
        /// 荷主ID
        /// </summary>
        public string ConsignorId { get; set; }

        /// <summary>
        /// 自動ログアウトモード（0:無効,1:PCのみ適用,2:HTのみ適用,999:全機に適用(DEVICE_GROUP_IDに対応)）
        /// </summary>
        public int AutoLogoutDeviceGroup { get; set; }
        /// <summary>
        /// 自動ログアウト時間[分]
        /// </summary>
        public int AutoLogoutTime { get; set; }
        /// <summary>
        /// 自動ログアウトチェック間隔[ミリ秒]
        /// </summary>
        public int AutoLogoutCheckInterval { get; set; }

        /// <summary>
        /// システム状態監視間隔[ミリ秒]
        /// </summary>
        public int MonitorSystemStatusInterval { get; set; }

        /// <summary>
        /// ピッキング予定更新間隔[ミリ秒]
        /// </summary>
        public int MonitorPickScheduleRefleshInterval { get; set; }

        /// <summary>
        /// 通知ログ間隔
        /// </summary>
        public int LogNotifyInterval { get; set; }

        /// <summary>
        /// ログインスキャン時ユーザコード判断文字列
        /// </summary>
        public string LogonReadCodeChar { get; set; }

        /// <summary>
        /// メインレイアウト部分のレイアウト情報
        /// </summary>
        public string MainLayoutMenuFontSizePC { get; set; }
        public string MainLayoutMenuFontSizeHT { get; set; }
        public string MainLayoutMenuFontWeightPC { get; set; }
        public string MainLayoutMenuFontWeightHT { get; set; }
        public string MainLayoutAffiliationFontSizePC { get; set; }
        public string MainLayoutAffiliationFontWeightPC { get; set; }
        public string MainLayoutUserFontSizePC { get; set; }
        public string MainLayoutUserFontSizeHT { get; set; }
        public string MainLayoutUserFontWeightPC { get; set; }
        public string MainLayoutUserFontWeightHT { get; set; }

        /// <summary>
        /// ログイン画面レイアウト情報
        /// </summary>
        public string LoginFontSize { get; set; }
        public string LoginFontWeight { get; set; }
        public string LoginFontSizeBtn { get; set; }
        public string LoginFontWeightBtn { get; set; }

        /// <summary>
        /// HTレイアウト情報
        /// </summary>
        public int ColumnSize { get; set; }
        public string FontSizeLabel { get; set; }
        public string FontSizeTextBox { get; set; }
        public string FontSizeComb { get; set; }
        public string FontWeightBold { get; set; }
        public string BackColorBadge { get; set; }
        public string FontSizeBadge { get; set; }
        public string LineHeightBadge { get; set; }
        public string EmergencyColor { get; set; }
        public string HightComb { get; set; }
        public string CaseBaraMargin { get; set; }
        public int CardColumnSize { get; set; }
        public string CardFontSizeLabel { get; set; }
        public string CardFontSizeTextBox { get; set; }
        public string CardFontWeightBold { get; set; }
        public string CardCaseBaraMargin { get; set; }
        public string CardCaseBaraContains { get; set; }
        public string HT_GetUnfinishedDataFlg { get; set; }
        public string HT_GetUnfinishedArrivalIcon { get; set; }
        public string HT_GetUnfinishedShipmentIcon { get; set; }
        public string HT_GetUnfinishedArrivalBackGroundColor { get; set; }
        public string HT_GetUnfinishedShipmentBackGroundColor { get; set; }
        public string HT_GetUnfinishedArrivalColor { get; set; }
        public string HT_GetUnfinishedShipmentColor { get; set; }
        public int HT_GetUnfinishedDataInterval { get; set; }

        public string MenuBottonMargin { get; set; }
        public string MenuFontSize { get; set; }
        public string MenuFontweight { get; set; }
        public string MenuBackGroundColorCode { get; set; }
        public string MenuForeColorCode { get; set; }
        public string MenuBackGroundColorCodeForFocus { get; set; }
        public string MenuForeColorCodeCodeForFocus { get; set; }

        public int HT_DefaultBuzzerTone { get; set; }
        public int HT_DefaultBuzzerOnPeriod { get; set; }
        public int HT_DefaultBuzzerOffPeriod { get; set; }
        public int HT_DefaultBuzzerReratCount { get; set; }
        public int HT_DefaultVibrationOnPeriod { get; set; }
        public int HT_DefaultVibrationOffPeriod { get; set; }
        public int HT_DefaultVibrationReratCount { get; set; }

        public int HT_ErrorBuzzerTone { get; set; }
        public int HT_ErrorBuzzerOnPeriod { get; set; }
        public int HT_ErrorBuzzerOffPeriod { get; set; }
        public int HT_ErrorBuzzerReratCount { get; set; }
        public int HT_ErrorVibrationOnPeriod { get; set; }
        public int HT_ErrorVibrationOffPeriod { get; set; }
        public int HT_ErrorVibrationReratCount { get; set; }

        public string DataGridHeaderFontSizePC { get; set; }
        public string DataGridHeaderFontSizeHT { get; set; }
        public string DataGridColumnFontSizePC { get; set; }
        public string DataGridColumnFontSizeHT { get; set; }
        public string DataGridCellFontSizePC { get; set; }
        public string DataGridCellFontSizeHT { get; set; }
        public string DataGridCellFontWeightPC { get; set; }
        public string DataGridCellFontWeightHT { get; set; }
        public string DataGridFooterFontSizePC { get; set; }
        public string DataGridFooterFontSizeHT { get; set; }
        public string DataGridHeightPC { get; set; }
        public string DataGridHeightHT { get; set; }

        public string PC_ValidationSummaryFontSize { get; set; }
        public string HT_ValidationSummaryFontSize { get; set; }
        public string PC_ValidationSummaryFontWeight { get; set; }
        public string HT_ValidationSummaryFontWeight { get; set; }
        public string HTArrivalInspectStatusColor { get; set; }

        #region PCコンポーネント関連
        /// <summary>
        /// GroupField タイトルのフォントサイズ
        /// </summary>
        public string PC_GroupFieldTitleFontSize { get; set; }
        /// <summary>
        /// GroupField タイトルのフォント幅
        /// </summary>
        public string PC_GroupFieldTitleFontWeight { get; set; }
        /// <summary>
        /// GroupField ラベルのフォントサイズ
        /// </summary>
        public string PC_GroupFieldLabelFontSize { get; set; }
        /// <summary>
        /// GroupField ラベルのフォント幅
        /// </summary>
        public string PC_GroupFieldLabelFontWeight { get; set; }
        /// <summary>
        /// GroupField ラベルの幅
        /// </summary>
        public string PC_GroupFieldLabelWidth { get; set; }

        /// <summary>
        /// PC　TextBoxのフォントサイズ
        /// </summary>
        public string PC_TextBoxFontSize { get; set; }
        /// <summary>
        /// PC　TextBoxのフォント幅
        /// </summary>
        public string PC_TextBoxFontWeight { get; set; }
        /// <summary>
        /// PC　TextBoxの幅
        /// </summary>
        public string PC_TextBoxWidth { get; set; }
        /// <summary>
        /// PC　TextBoxの高さ
        /// </summary>
        public string PC_TextBoxHeight { get; set; }

        /// <summary>
        /// PC　TextAreaのフォントサイズ
        /// </summary>
        public string PC_TextAreaFontSize { get; set; }
        /// <summary>
        /// PC　TextAreaのフォント幅
        /// </summary>
        public string PC_TextAreaFontWeight { get; set; }

        /// <summary>
        /// PC　DatePickerのフォントサイズ
        /// </summary>
        public string PC_DatePickerFontSize { get; set; }
        /// <summary>
        /// PC　DatePickerのフォント幅
        /// </summary>
        public string PC_DatePickerFontWeight { get; set; }
        /// <summary>
        /// PC　DatePickerの幅
        /// </summary>
        public string PC_DatePickerWidth { get; set; }
        /// <summary>
        /// PC　DatePickerの高さ
        /// </summary>
        public string PC_DatePickerHeight { get; set; }

        /// <summary>
        /// PC　TimePickerのフォントサイズ
        /// </summary>
        public string PC_TimePickerFontSize { get; set; }
        /// <summary>
        /// PC　TimePickerのフォント幅
        /// </summary>
        public string PC_TimePickerFontWeight { get; set; }
        /// <summary>
        /// PC　TimePickerの幅
        /// </summary>
        public string PC_TimePickerWidth { get; set; }
        /// <summary>
        /// PC　TimePickerの高さ
        /// </summary>
        public string PC_TimePickerHeight { get; set; }

        /// <summary>
        /// PC　DateTimePickerのフォントサイズ
        /// </summary>
        public string PC_DateTimePickerFontSize { get; set; }
        /// <summary>
        /// PC　DateTimePickerのフォント幅
        /// </summary>
        public string PC_DateTimePickerFontWeight { get; set; }
        /// <summary>
        /// PC　DateTimePickerの幅
        /// </summary>
        public string PC_DateTimePickerWidth { get; set; }
        /// <summary>
        /// PC　DateTimePickerの高さ
        /// </summary>
        public string PC_DateTimePickerHeight { get; set; }

        /// <summary>
        /// PC　DropDownのフォントサイズ
        /// </summary>
        public string PC_DropDownFontSize { get; set; }
        /// <summary>
        /// PC　DropDownのフォント幅
        /// </summary>
        public string PC_DropDownFontWeight { get; set; }
        /// <summary>
        /// PC　DropDownの幅
        /// </summary>
        public string PC_DropDownWidth { get; set; }
        /// <summary>
        /// PC　DropDownの高さ
        /// </summary>
        public string PC_DropDownHeight { get; set; }

        /// <summary>
        /// PC　CheckBoxのフォントサイズ
        /// </summary>
        public string PC_CheckBoxFontSize { get; set; }
        /// <summary>
        /// PC　CheckBoxのフォント幅
        /// </summary>
        public string PC_CheckBoxFontWeight { get; set; }
        /// <summary>
        /// PC　CheckBoxタイトルのフォントサイズ
        /// </summary>
        public string PC_CheckBoxTitleFontSize { get; set; }
        /// <summary>
        /// PC　CheckBoxタイトルのフォント幅
        /// </summary>
        public string PC_CheckBoxTitleFontWeight { get; set; }

        /// <summary>
        /// PC　RadioButtonのフォントサイズ
        /// </summary>
        public string PC_RadioButtonFontSize { get; set; }
        /// <summary>
        /// PC　RadioButtonのフォント幅
        /// </summary>
        public string PC_RadioButtonFontWeight { get; set; }
        /// <summary>
        /// PC　RadioButtonタイトルのフォントサイズ
        /// </summary>
        public string PC_RadioButtonTitleFontSize { get; set; }
        /// <summary>
        /// PC　RadioButtonタイトルのフォント幅
        /// </summary>
        public string PC_RadioButtonTitleFontWeight { get; set; }

        /// <summary>
        /// PC　Numericのフォントサイズ
        /// </summary>
        public string PC_NumericFontSize { get; set; }
        /// <summary>
        /// PC　Numericのフォント幅
        /// </summary>
        public string PC_NumericFontWeight { get; set; }
        /// <summary>
        /// PC　Numericの幅
        /// </summary>
        public string PC_NumericWidth { get; set; }
        /// <summary>
        /// PC　Numericの高さ
        /// </summary>
        public string PC_NumericHeight { get; set; }

        /// <summary>
        /// PC　DropDownDataGridのフォントサイズ
        /// </summary>
        public string PC_DropDownDataGridFontSize { get; set; }
        /// <summary>
        /// PC　DropDownDataGridのフォント幅
        /// </summary>
        public string PC_DropDownDataGridFontWeight { get; set; }
        /// <summary>
        /// PC　DropDownDataGridの高さ
        /// </summary>
        public string PC_DropDownDataGridHeight { get; set; }

        /// <summary>
        /// PC　TabsExtendのフォントサイズ
        /// </summary>
        public string PC_TabsExtendFontSize { get; set; }
        /// <summary>
        /// PC　TabsExtendのフォント幅
        /// </summary>
        public string PC_TabsExtendFontWeight { get; set; }

        /// <summary>
        /// PC　日付コンポーネント初期値がWmsAddの加算日数
        /// </summary>
        public int PC_DateInitWmsAddDays { get; set; }

        #endregion

        /// <summary>
        /// PC必須表示の付加文字
        /// </summary>
        public string RequiredDisplaySuffixPC { get; set; }
        /// <summary>
        /// HT必須表示の付加文字
        /// </summary>
        public string RequiredDisplaySuffixHT { get; set; }

        /// <summary>
        /// 出荷開始警告時間[分]
        /// </summary>
        public int ShipmentsStartWarnTime { get; set; }
        /// <summary>
        /// 出荷締切警告時間[分]
        /// </summary>
        public int ShipmentsDeadlineWarnTime { get; set; }
        /// <summary>
        /// 通知ポップアップ表示時間[ms]
        /// </summary>
        public int NotifyPopupDuration { get; set; }



        //PC,HT画面のファンクションボタンのmargin-bottomの高さ
        public string PC_ButtonMarginBottom { get; set; }
        public string HT_ButtonMarginBottom { get; set; }
        //HT画面のファンクションボタンの背景色の色
        public string HT_Button1BackgroundColor { get; set; }
        public string HT_Button2BackgroundColor { get; set; }
        public string HT_Button3BackgroundColor { get; set; }
        public string HT_Button4BackgroundColor { get; set; }
        //HT画面のファンクションボタンの文字の色
        public string HT_Button1TextColor { get; set; }
        public string HT_Button4TextColor { get; set; }

        //PC　DropDownDataGridのチェックボックスの幅
        public string PC_DropDownDataGridCheckBoxWidth { get; set; }

        //PC 必須表示の付加文字の色
        public string RequiredDisplaySuffixColorPC { get; set; }

        //HTメニューボタンの幅の大きさ
        public string HT_MenuBottonWidth { get; set; }

        //コンポーネントのテキストエリアの行数,列数
        public int CompTextAreaRows { get; set; }
        public int CompTextAreaCols { get; set; }

        //ログインフォームのタイトルと入力欄のColumn値
        public int LoginFormTitleColumnSize { get; set; }
        public int LoginFormTextBoxColumnSize { get; set; }

        //HT　データカードのmarginの値
        public string HT_DataCardContentMarginTop { get; set; }
        public string HT_DataCardContentMarginLeft { get; set; }
        public string HT_DataCardContentMarginRight { get; set; }
        public string HT_DataCardContentMarginBottom { get; set; }
        //HT データカード内のStackのmarginの値
        public string HT_InsideDataCardContentMarginTop { get; set; }
        public string HT_InsideDataCardContentMarginLeft { get; set; }
        public string HT_InsideDataCardContentMarginRight { get; set; }
        //HT　データカードリストの高さ
        public string HT_DataCardListHeight { get; set; }

        //HT　データグリッド内のバラの色
        public string HT_DataGridBaraColor { get; set; }

        //HT　ロケーションコンボボックスの最大値の数。この値より大きい場合は、テキストボックスで標示のみとする
        public int HT_LocComBoxMaxCount { get; set; }

        //PC　進捗管理の凡例のマーク,テキストの幅
        public int PC_DataGridProgressMarkWidth { get; set; }
        public int PC_DataGridProgressTextWidth { get; set; }

        //メッセージダイアログの幅,高さ（処理中、読込中）
        public int MessageDialogWidth { get; set; }
        public int MessageDialogHeight { get; set; }
        //確認メッセージダイアログの幅,高さ（OKのみ）
        public int DialogShowOKWidth { get; set; }
        public int DialogShowOKHeight { get; set; }
        //確認メッセージダイアログの幅（YesとNo）
        public int DialogShowYesNoWidth { get; set; }
        public int DialogShowYesNoHeight { get; set; }

        //編集ダイアログの備考の最大入力数,行,列数
        public int DialogContentMaxlength { get; set; }
        public int DialogContentCols { get; set; }
        public int DialogContentRows { get; set; }

        //編集ダイアログの概要の文字数
        public int DialogContentRemarksRows { get; set; }

        //データグリッドのページ数
        public int DataGridPageSize { get; set; }

        public SystemParameter()
        {
            BaseId = "5200";
            BaseType = 0;
            ConsignorId = "000000";

            AutoLogoutDeviceGroup = 0;
            AutoLogoutTime = 30;
            AutoLogoutCheckInterval = 60000;

            MonitorSystemStatusInterval = 5000;

            MonitorPickScheduleRefleshInterval = 30000;

            LogNotifyInterval = 5000;

            LogonReadCodeChar = "$$$";

            MainLayoutMenuFontSizePC = "140%";
            MainLayoutMenuFontSizeHT = "120%";
            MainLayoutMenuFontWeightPC = "bold";
            MainLayoutMenuFontWeightHT = "bold";
            MainLayoutAffiliationFontSizePC = "100%";
            MainLayoutAffiliationFontWeightPC = "normal";
            MainLayoutUserFontSizePC = "100%";
            MainLayoutUserFontSizeHT = "100%";
            MainLayoutUserFontWeightPC = "normal";
            MainLayoutUserFontWeightHT = "normal";

            LoginFontSize = "125%";
            LoginFontWeight = "bold";
            LoginFontSizeBtn = "150%";
            LoginFontWeightBtn = "bold";

            ColumnSize = 4;
            FontSizeLabel = "125%";
            FontSizeTextBox = "150%";
            FontSizeComb = "130%";
            FontWeightBold = "bold";
            BackColorBadge = "#ffa000";
            FontSizeBadge = "150%";
            LineHeightBadge = "150%";
            EmergencyColor = "#ffbeda";
            HightComb = "32px";

            MenuBottonMargin = "1px";
            MenuFontSize = "200%";
            MenuFontweight = "bold";
            MenuBackGroundColorCode = "#30445f";
            MenuForeColorCode = "#ffffff";
            MenuBackGroundColorCodeForFocus = "#ffa500";
            MenuForeColorCodeCodeForFocus = "#ffffff";

            HT_DefaultBuzzerTone = 8;
            HT_DefaultBuzzerOnPeriod = 100;
            HT_DefaultBuzzerOffPeriod = 100;
            HT_DefaultBuzzerReratCount = 1;
            HT_DefaultVibrationOnPeriod = 100;
            HT_DefaultVibrationOffPeriod = 100;
            HT_DefaultVibrationReratCount = 1;

            HT_ErrorBuzzerTone = 3;
            HT_ErrorBuzzerOnPeriod = 100;
            HT_ErrorBuzzerOffPeriod = 100;
            HT_ErrorBuzzerReratCount = 3;
            HT_ErrorVibrationOnPeriod = 100;
            HT_ErrorVibrationOffPeriod = 100;
            HT_ErrorVibrationReratCount = 1;

            HT_GetUnfinishedDataFlg = "false";
            HT_GetUnfinishedArrivalIcon = string.Empty;
            HT_GetUnfinishedShipmentIcon = string.Empty;
            HT_GetUnfinishedArrivalBackGroundColor = "#ff7f7f";
            HT_GetUnfinishedShipmentBackGroundColor = "#7fbfff";
            HT_GetUnfinishedArrivalColor = "#ffffff";
            HT_GetUnfinishedShipmentColor = "#ffffff";
            HT_GetUnfinishedDataInterval = 60000;

            DataGridHeaderFontSizePC = "100%";
            DataGridColumnFontSizeHT = "100%";
            DataGridColumnFontSizePC = "100%";
            DataGridHeaderFontSizeHT = "100%";
            DataGridCellFontSizePC = "100%";
            DataGridCellFontSizeHT = "100%";
            DataGridCellFontWeightPC = "normal";
            DataGridCellFontWeightHT = "normal";
            DataGridFooterFontSizePC = "100%";
            DataGridFooterFontSizeHT = "100%";
            DataGridHeightPC = string.Empty;
            DataGridHeightHT = string.Empty;

            PC_ValidationSummaryFontSize = "100%";
            HT_ValidationSummaryFontSize = "100%";
            PC_ValidationSummaryFontWeight = "normal";
            HT_ValidationSummaryFontWeight = "normal";

            PC_GroupFieldTitleFontSize = "100%";
            PC_GroupFieldTitleFontWeight = "bold";
            PC_GroupFieldLabelFontSize = "100%";
            PC_GroupFieldLabelFontWeight = "bold";
            PC_GroupFieldLabelWidth = "150px";
            PC_TextBoxFontSize = "100%";
            PC_TextBoxFontWeight = "normal";
            PC_TextBoxWidth = string.Empty;
            PC_TextBoxHeight = string.Empty;
            PC_TextAreaFontSize = "100%";
            PC_TextAreaFontWeight = "normal";
            PC_DatePickerFontSize = "100%";
            PC_DatePickerFontWeight = "normal";
            PC_DatePickerWidth = string.Empty;
            PC_DatePickerHeight = string.Empty;
            PC_TimePickerFontSize = "100%";
            PC_TimePickerFontWeight = "normal";
            PC_TimePickerWidth = string.Empty;
            PC_TimePickerHeight = string.Empty;
            PC_DateTimePickerFontSize = "100%";
            PC_DateTimePickerFontWeight = "normal";
            PC_DateTimePickerWidth = string.Empty;
            PC_DateTimePickerHeight = string.Empty;
            PC_DropDownFontSize = "100%";
            PC_DropDownFontWeight = "normal";
            PC_DropDownWidth = string.Empty;
            PC_DropDownHeight = string.Empty;
            PC_CheckBoxFontSize = "100%";
            PC_CheckBoxFontWeight = "normal";
            PC_CheckBoxTitleFontSize = "100%";
            PC_CheckBoxTitleFontWeight = "normal";
            PC_RadioButtonFontSize = "100%";
            PC_RadioButtonFontWeight = "normal";
            PC_RadioButtonTitleFontSize = "100%";
            PC_RadioButtonTitleFontWeight = "normal";
            PC_NumericFontSize = "100%";
            PC_NumericFontWeight = "normal";
            PC_NumericWidth = string.Empty;
            PC_NumericHeight = string.Empty;
            PC_DropDownDataGridFontSize = "100%";
            PC_DropDownDataGridFontWeight = "normal";
            PC_DropDownDataGridHeight = string.Empty;
            PC_TabsExtendFontSize = "100%";
            PC_TabsExtendFontWeight = "normal";
            PC_DateInitWmsAddDays = 1;

            RequiredDisplaySuffixPC = "＊";
            RequiredDisplaySuffixHT = "＊";

            HTArrivalInspectStatusColor = "rz-background-color-warning-lighter";
            ShipmentsStartWarnTime = 30;
            ShipmentsDeadlineWarnTime = 30;
            NotifyPopupDuration = SharedConst.DEFAULT_NOTIFY_DURATION;

            PC_ButtonMarginBottom = "1px";
            HT_ButtonMarginBottom = "1px";

            HT_Button1BackgroundColor = "#000000";
            HT_Button2BackgroundColor = "#D3D3D3";
            HT_Button3BackgroundColor = "#FFA500";
            HT_Button4BackgroundColor = "#000000";

            HT_Button1TextColor = "#ffffff";
            HT_Button4TextColor = "#ffffff";

            PC_DropDownDataGridCheckBoxWidth = "60px";

            RequiredDisplaySuffixColorPC = "red";

            HT_MenuBottonWidth = "100%";

            CompTextAreaRows = 2;
            CompTextAreaCols = 20;

            LoginFormTitleColumnSize = 5;
            LoginFormTitleColumnSize = 7;

            HT_DataCardContentMarginTop = "0px";
            HT_DataCardContentMarginRight = "0px";
            HT_DataCardContentMarginLeft = "0px";
            HT_DataCardContentMarginBottom = "0px";

            HT_InsideDataCardContentMarginTop = "0px";
            HT_InsideDataCardContentMarginRight = "0px";
            HT_InsideDataCardContentMarginLeft = "0px";

            HT_DataCardListHeight = "400px";

            HT_DataGridBaraColor = "#ff0000";
            HT_LocComBoxMaxCount = int.MaxValue;

            PC_DataGridProgressMarkWidth = 50;
            PC_DataGridProgressMarkWidth = 150;

            MessageDialogWidth = 200;
            MessageDialogHeight = 155;
            DialogShowOKWidth = 350;
            DialogShowOKHeight = 200;
            DialogShowYesNoWidth = 350;
            DialogShowYesNoHeight = 200;

            DialogContentMaxlength = 256;
            DialogContentCols = 60;
            DialogContentRows = 3;

            DialogContentRemarksRows = 20;

            DataGridPageSize = 10;

            //Todo 下記必要？
            CaseBaraMargin = string.Empty;
            CardFontSizeLabel = string.Empty;
            CardFontSizeTextBox = string.Empty;
            CardFontWeightBold = string.Empty;
            CardCaseBaraMargin = string.Empty;
            CardCaseBaraContains = string.Empty;
        }
    }
}
