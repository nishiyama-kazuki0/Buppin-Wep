using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Util;
using SharedModels;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// DEFINE_SYSTEM_PARAMETERSテーブル情報
/// </summary>
public sealed class SystemParameterService
{
    #region パラメータプロパティ

    /// <summary>
    /// 支所場コード
    /// </summary>
    public string BaseId { get; set; } = "5200";
    /// <summary>
    /// 所場区分
    /// </summary>
    public int BaseType { get; set; } = 0;
    /// <summary>
    /// 荷主ID
    /// </summary>
    public string ConsignorId { get; set; } = "000000";

    /// <summary>
    /// 自動ログアウトモード（0:無効,1:PCのみ適用,2:HTのみ適用,999:全機に適用(DEVICE_GROUP_IDに対応)）
    /// </summary>
    public int AutoLogoutDeviceGroup { get; set; } = 0;
    /// <summary>
    /// 自動ログアウト時間[分]
    /// </summary>
    public int AutoLogoutTime { get; set; } = 30;
    /// <summary>
    /// 自動ログアウトチェック間隔[ミリ秒]
    /// </summary>
    public int AutoLogoutCheckInterval { get; set; } = 60000;


    /// <summary>
    /// システム状態監視間隔[ミリ秒]
    /// </summary>
    public int MonitorSystemStatusInterval { get; set; } = 5000;

    /// <summary>
    /// ピッキング予定更新間隔[ミリ秒]
    /// </summary>
    public int MonitorPickScheduleRefleshInterval { get; set; } = 30000;

    /// <summary>
    /// 通知ログ間隔
    /// </summary>
    public int LogNotifyInterval { get; set; } = 5000;

    /// <summary>
    /// ログインスキャン時ユーザコード判断文字列
    /// </summary>
    public string LogonReadCodeChar { get; set; } = "$$$";

    /// <summary>
    /// メインレイアウト部分のレイアウト情報
    /// </summary>
    public string MainLayoutMenuFontSizePC { get; set; } = "140%";
    public string MainLayoutMenuFontSizeHT { get; set; } = "120%";
    public string MainLayoutMenuFontWeightPC { get; set; } = "bold";
    public string MainLayoutMenuFontWeightHT { get; set; } = "bold";
    public string MainLayoutAffiliationFontSizePC { get; set; } = "100%";
    public string MainLayoutAffiliationFontWeightPC { get; set; } = "normal";
    public string MainLayoutUserFontSizePC { get; set; } = "100%";
    public string MainLayoutUserFontSizeHT { get; set; } = "100%";
    public string MainLayoutUserFontWeightPC { get; set; } = "normal";
    public string MainLayoutUserFontWeightHT { get; set; } = "normal";

    /// <summary>
    /// ログイン画面レイアウト情報
    /// </summary>
    public string LoginFontSize { get; set; } = "125%";
    public string LoginFontWeight { get; set; } = "bold";
    public string LoginFontSizeBtn { get; set; } = "150%";
    public string LoginFontWeightBtn { get; set; } = "bold";

    /// <summary>
    /// HTレイアウト情報
    /// </summary>
    public int ColumnSize { get; set; } = 4;
    public string FontSizeLabel { get; set; } = "125%";
    public string FontSizeTextBox { get; set; } = "150%";
    public string FontSizeComb { get; set; } = "130%";
    public string FontWeightBold { get; set; } = "bold";
    public string BackColorBadge { get; set; } = "#ffa000";
    public string FontSizeBadge { get; set; } = "150%";
    public string LineHeightBadge { get; set; } = "150%";
    public string EmergencyColor { get; set; } = "#ffbeda";
    public string HightComb { get; set; } = "32px";

    public string CaseBaraMargin { get; set; } = string.Empty;
    public int CardColumnSize { get; set; } = 4;
    public string CardFontSizeLabel { get; set; } = "100%";
    public string CardFontSizeTextBox { get; set; } = "100%";
    public string CardFontWeightBold { get; set; } = "normal";
    public string CardCaseBaraMargin { get; set; } = string.Empty;
    public string CardCaseBaraContains { get; set; } = string.Empty;

    //画面ヘッダーの未作業通知アイコン関連
    public string HT_GetUnfinishedDataFlg { get; set; } = "false";
    public string HT_GetUnfinishedArrivalIcon { get; set; } = string.Empty;
    public string HT_GetUnfinishedShipmentIcon { get; set; } = string.Empty;
    public string HT_GetUnfinishedArrivalBackGroundColor { get; set; } = "#ff7f7f";
    public string HT_GetUnfinishedShipmentBackGroundColor { get; set; } = "#7fbfff";
    public string HT_GetUnfinishedArrivalColor { get; set; } = "#ffffff";
    public string HT_GetUnfinishedShipmentColor { get; set; } = "#ffffff";
    public int HT_GetUnfinishedDataInterval { get; set; } = 60000;

    public string MenuBottonMargin { get; set; } = "1px";
    public string MenuFontSize { get; set; } = "200%";
    public string MenuFontweight { get; set; } = "bold";
    public string MenuBackGroundColorCode { get; set; } = "#30445f";
    public string MenuForeColorCode { get; set; } = "#ffffff";
    public string MenuBackGroundColorCodeForFocus { get; set; } = "#ffa500";
    public string MenuForeColorCodeCodeForFocus { get; set; } = "#ffffff";

    public int HT_DefaultBuzzerTone { get; set; } = 8;
    public int HT_DefaultBuzzerOnPeriod { get; set; } = 100;
    public int HT_DefaultBuzzerOffPeriod { get; set; } = 100;
    public int HT_DefaultBuzzerReratCount { get; set; } = 1;
    public int HT_DefaultVibrationOnPeriod { get; set; } = 100;
    public int HT_DefaultVibrationOffPeriod { get; set; } = 100;
    public int HT_DefaultVibrationReratCount { get; set; } = 1;

    public int HT_ErrorBuzzerTone { get; set; } = 3;
    public int HT_ErrorBuzzerOnPeriod { get; set; } = 100;
    public int HT_ErrorBuzzerOffPeriod { get; set; } = 100;
    public int HT_ErrorBuzzerReratCount { get; set; } = 3;
    public int HT_ErrorVibrationOnPeriod { get; set; } = 100;
    public int HT_ErrorVibrationOffPeriod { get; set; } = 100;
    public int HT_ErrorVibrationReratCount { get; set; } = 1;

    public string DataGridHeaderFontSizePC { get; set; } = "100%";
    public string DataGridHeaderFontSizeHT { get; set; } = "100%";
    public string DataGridColumnFontSizePC { get; set; } = "100%";
    public string DataGridColumnFontSizeHT { get; set; } = "100%";
    public string DataGridCellFontSizePC { get; set; } = "100%";
    public string DataGridCellFontSizeHT { get; set; } = "100%";
    public string DataGridCellFontWeightPC { get; set; } = "normal";
    public string DataGridCellFontWeightHT { get; set; } = "normal";
    public string DataGridFooterFontSizePC { get; set; } = "100%";
    public string DataGridFooterFontSizeHT { get; set; } = "100%";
    public string DataGridHeightPC { get; set; } = string.Empty;
    public string DataGridHeightHT { get; set; } = string.Empty;

    public string PC_ValidationSummaryFontSize { get; set; } = "100%";
    public string HT_ValidationSummaryFontSize { get; set; } = "100%";
    public string PC_ValidationSummaryFontWeight { get; set; } = "normal";
    public string HT_ValidationSummaryFontWeight { get; set; } = "normal";
    public string HTArrivalInspectStatusColor { get; set; } = "rz-background-color-warning-lighter";


    /// <summary>
    /// GroupField タイトルのフォントサイズ
    /// </summary>
    public string PC_GroupFieldTitleFontSize { get; set; } = "100%";
    /// <summary>
    /// GroupField タイトルのフォント幅
    /// </summary>
    public string PC_GroupFieldTitleFontWeight { get; set; } = "bold";
    /// <summary>
    /// GroupField ラベルのフォントサイズ
    /// </summary>
    public string PC_GroupFieldLabelFontSize { get; set; } = "100%";
    /// <summary>
    /// GroupField ラベルのフォント幅
    /// </summary>
    public string PC_GroupFieldLabelFontWeight { get; set; } = "bold";
    /// <summary>
    /// GroupField ラベルの幅
    /// </summary>
    public string PC_GroupFieldLabelWidth { get; set; } = "150px";

    /// <summary>
    /// PC　TextBoxのフォントサイズ
    /// </summary>
    public string PC_TextBoxFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　TextBoxのフォント幅
    /// </summary>
    public string PC_TextBoxFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　TextBoxの幅
    /// </summary>
    public string PC_TextBoxWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　TextBoxの高さ
    /// </summary>
    public string PC_TextBoxHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　TextAreaのフォントサイズ
    /// </summary>
    public string PC_TextAreaFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　TextAreaのフォント幅
    /// </summary>
    public string PC_TextAreaFontWeight { get; set; } = "normal";

    /// <summary>
    /// PC　DatePickerのフォントサイズ
    /// </summary>
    public string PC_DatePickerFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　DatePickerのフォント幅
    /// </summary>
    public string PC_DatePickerFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　DatePickerの幅
    /// </summary>
    public string PC_DatePickerWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　DatePickerの高さ
    /// </summary>
    public string PC_DatePickerHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　TimePickerのフォントサイズ
    /// </summary>
    public string PC_TimePickerFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　TimePickerのフォント幅
    /// </summary>
    public string PC_TimePickerFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　TimePickerの幅
    /// </summary>
    public string PC_TimePickerWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　TimePickerの高さ
    /// </summary>
    public string PC_TimePickerHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　DateTimePickerのフォントサイズ
    /// </summary>
    public string PC_DateTimePickerFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　DateTimePickerのフォント幅
    /// </summary>
    public string PC_DateTimePickerFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　DateTimePickerの幅
    /// </summary>
    public string PC_DateTimePickerWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　DateTimePickerの高さ
    /// </summary>
    public string PC_DateTimePickerHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　DropDownのフォントサイズ
    /// </summary>
    public string PC_DropDownFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　DropDownのフォント幅
    /// </summary>
    public string PC_DropDownFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　DropDownの幅
    /// </summary>
    public string PC_DropDownWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　DropDownの高さ
    /// </summary>
    public string PC_DropDownHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　CheckBoxのフォントサイズ
    /// </summary>
    public string PC_CheckBoxFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　CheckBoxのフォント幅
    /// </summary>
    public string PC_CheckBoxFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　CheckBoxタイトルのフォントサイズ
    /// </summary>
    public string PC_CheckBoxTitleFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　CheckBoxタイトルのフォント幅
    /// </summary>
    public string PC_CheckBoxTitleFontWeight { get; set; } = "normal";

    /// <summary>
    /// PC　RadioButtonのフォントサイズ
    /// </summary>
    public string PC_RadioButtonFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　RadioButtonのフォント幅
    /// </summary>
    public string PC_RadioButtonFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　RadioButtonタイトルのフォントサイズ
    /// </summary>
    public string PC_RadioButtonTitleFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　RadioButtonタイトルのフォント幅
    /// </summary>
    public string PC_RadioButtonTitleFontWeight { get; set; } = "normal";

    /// <summary>
    /// PC　Numericのフォントサイズ
    /// </summary>
    public string PC_NumericFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　Numericのフォント幅
    /// </summary>
    public string PC_NumericFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　Numericの幅
    /// </summary>
    public string PC_NumericWidth { get; set; } = string.Empty;
    /// <summary>
    /// PC　Numericの高さ
    /// </summary>
    public string PC_NumericHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　DropDownDataGridのフォントサイズ
    /// </summary>
    public string PC_DropDownDataGridFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　DropDownDataGridのフォント幅
    /// </summary>
    public string PC_DropDownDataGridFontWeight { get; set; } = "normal";
    /// <summary>
    /// PC　DropDownDataGridの高さ
    /// </summary>
    public string PC_DropDownDataGridHeight { get; set; } = string.Empty;

    /// <summary>
    /// PC　TabsExtendのフォントサイズ
    /// </summary>
    public string PC_TabsExtendFontSize { get; set; } = "100%";
    /// <summary>
    /// PC　TabsExtendのフォント幅
    /// </summary>
    public string PC_TabsExtendFontWeight { get; set; } = "normal";

    /// <summary>
    /// PC　日付コンポーネント初期値がWmsAddの加算日数
    /// </summary>
    public int PC_DateInitWmsAddDays { get; set; } = 1;


    /// <summary>
    /// PC必須表示の付加文字
    /// </summary>
    public string RequiredDisplaySuffixPC { get; set; } = "＊";
    /// <summary>
    /// HT必須表示の付加文字
    /// 西山16
    /// </summary>
    public string RequiredDisplaySuffixHT { get; set; } = "＊";

    /// <summary>
    /// 出荷開始警告時間[分]
    /// </summary>
    public int ShipmentsStartWarnTime { get; set; } = 30;
    /// <summary>
    /// 出荷締切警告時間[分]
    /// </summary>
    public int ShipmentsDeadlineWarnTime { get; set; } = 30;
    /// <summary>
    /// 通知ポップアップ表示時間[ms]
    /// </summary>
    public int NotifyPopupDuration { get; set; } = SharedConst.DEFAULT_NOTIFY_DURATION;



    //PC,HT画面のファンクションボタンのmargin-bottomの高さ
    public string PC_ButtonMarginBottom { get; set; } = "1px";
    public string HT_ButtonMarginBottom { get; set; } = "1px";
    //HT画面のファンクションボタンの背景色の色
    public string HT_Button1BackgroundColor { get; set; } = "#000000";
    public string HT_Button2BackgroundColor { get; set; } = "#D3D3D3";
    public string HT_Button3BackgroundColor { get; set; } = "#FFA500";
    public string HT_Button4BackgroundColor { get; set; } = "#000000";
    //HT画面のファンクションボタンの文字の色
    public string HT_Button1TextColor { get; set; } = "#ffffff";
    public string HT_Button4TextColor { get; set; } = "#ffffff";

    //PC　DropDownDataGridのチェックボックスの幅
    public string PC_DropDownDataGridCheckBoxWidth { get; set; } = "60px";

    //PC 必須表示の付加文字の色
    public string RequiredDisplaySuffixColorPC { get; set; } = "red";

    //HTメニューボタンの幅の大きさ
    public string HT_MenuBottonWidth { get; set; } = "100%";

    //コンポーネントのテキストエリアの行数,列数
    public int CompTextAreaRows { get; set; } = 2;
    public int CompTextAreaCols { get; set; } = 20;

    //ログインフォームのタイトルと入力欄のColumn値
    public int LoginFormTitleColumnSize { get; set; } = 5;
    public int LoginFormTextBoxColumnSize { get; set; } = 7;

    //HT　データカードのmarginの値
    public string HT_DataCardContentMarginTop { get; set; } = "0px";
    public string HT_DataCardContentMarginLeft { get; set; } = "0px";
    public string HT_DataCardContentMarginRight { get; set; } = "0px";
    public string HT_DataCardContentMarginBottom { get; set; } = "0px";
    //HT データカード内のStackのmarginの値
    public string HT_InsideDataCardContentMarginTop { get; set; } = "0px";
    public string HT_InsideDataCardContentMarginLeft { get; set; } = "0px";
    public string HT_InsideDataCardContentMarginRight { get; set; } = "0px";
    //HT　データカードリストの高さ
    public string HT_DataCardListHeight { get; set; } = "400px";

    //HT　データグリッド内のバラの色
    public string HT_DataGridBaraColor { get; set; } = "#ff0000";

    //HT　ロケーションコンボボックスの最大値の数。この値より大きい場合は、テキストボックスで標示のみとする
    public int HT_LocComBoxMaxCount { get; set; } = int.MaxValue;

    //PC　進捗管理の凡例のマーク,テキストの幅
    public int PC_DataGridProgressMarkWidth { get; set; } = 50;
    public int PC_DataGridProgressTextWidth { get; set; } = 150;

    //メッセージダイアログの幅,高さ（処理中、読込中）
    public int MessageDialogWidth { get; set; } = 200;
    public int MessageDialogHeight { get; set; } = 155;
    //確認メッセージダイアログの幅,高さ（OKのみ）
    public int DialogShowOKWidth { get; set; } = 350;
    public int DialogShowOKHeight { get; set; } = 200;
    //確認メッセージダイアログの幅（YesとNo）
    public int DialogShowYesNoWidth { get; set; } = 350;
    public int DialogShowYesNoHeight { get; set; } = 200;

    //編集ダイアログの備考の最大入力数,行,列数
    public int DialogContentMaxlength { get; set; } = 256;
    public int DialogContentCols { get; set; } = 60;
    public int DialogContentRows { get; set; } = 3;

    //編集ダイアログの概要の文字数
    public int DialogContentRemarksRows { get; set; } = 20;

    //データグリッドのページ数
    public int DataGridPageSize { get; set; } = 10;

    //グリッド色
    public string ColorBKGridCellContainColumnName { get; set; } = string.Empty;
    public string ColorBKGridRowContainColumnName { get; set; } = string.Empty;
    public string ColorFRGridCellContainColumnName { get; set; } = string.Empty;
    public string ColorFRGridRowContainColumnName { get; set; } = string.Empty;
    public string ColorGridColumnNameSplitString { get; set; } = string.Empty;

    //グリッドセル選択
    public string UnSelectableGridCellContainColumnName { get; set; } = string.Empty;
    
    //ステップコンポーネントの上段ステップタイトル関連
    public string StepTitleDisplay { get; set; } = string.Empty;
    public string StepNumberDisplay { get; set; } = string.Empty;
    public string StepUlDisplay { get; set; } = string.Empty;

    //※追加のパラメータが必要な場合はこのリージョン内に記述すること。

    #endregion

    /// <summary>
    /// HttpClient
    /// </summary>
    private readonly CommonWebComService _webComService;

    /// <summary>
    /// コンストラクタ
    /// Jsonデシリアライズでエラーとなるため、引数なしのコンストラクタを一旦用意。
    /// </summary>
    public SystemParameterService() { }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SystemParameterService(CommonWebComService webComService)
    {
        _webComService = webComService;
    }

    /// <summary>
    /// システムパラメータをDBより読み込む
    /// </summary>
    public async Task LoadSystemParameters()
    {
        ClassNameSelect select = new()
        {
            viewName = "DEFINE_SYSTEM_PARAMETERS"
            ,
            tsqlHints = EnumTSQLhints.NOLOCK
        };
        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);

        Dictionary<string, SystemParameter> sysParam = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetSystemParameter_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            SystemParameter newRow = new()
            {
                ParameterKey = ConvertUtil.GetValueString(item, "PARAMETER_KEY"),
                KeyName = ConvertUtil.GetValueString(item, "KEY_NAME"),
                ParameterValue = ConvertUtil.GetValueString(item, "PARAMETER_VALUE"),
            };

            sysParam[newRow.ParameterKey] = newRow;
        }

        // DEFINE_SYSTEM_PARAMETERSテーブルのキー情報から情報クラスに展開する
        BaseId = GetSystemParameter(sysParam, "BASE_ID_const", BaseId);
        BaseType = GetSystemParameterInt(sysParam, "BASE_TYPE_const", BaseType);
        ConsignorId = GetSystemParameter(sysParam, "CONSIGNOR_ID_const", ConsignorId);

        // 自動ログアウト関連
        AutoLogoutDeviceGroup = GetSystemParameterInt(sysParam, "AutoLogoutDeviceGroup", AutoLogoutDeviceGroup);
        AutoLogoutTime = GetSystemParameterInt(sysParam, "AutoLogoutTime", AutoLogoutTime);
        AutoLogoutCheckInterval = GetSystemParameterInt(sysParam, "AutoLogoutCheckInterval", AutoLogoutCheckInterval);

        // システム状態監視間隔
        MonitorSystemStatusInterval = GetSystemParameterInt(sysParam, "MonitorSystemStatusInterval", MonitorSystemStatusInterval);

        //ピッキング予定リフレッシュ間隔
        MonitorPickScheduleRefleshInterval = GetSystemParameterInt(sysParam, "MonitorPickScheduleRefleshInterval", MonitorPickScheduleRefleshInterval);

        // ログ通知周期
        LogNotifyInterval = GetSystemParameterInt(sysParam, "LogNotifyInterval", LogNotifyInterval);

        // ログインスキャン時ユーザコード判断文字列
        LogonReadCodeChar = GetSystemParameter(sysParam, "LogonReadCodeChar", LogonReadCodeChar);

        // メインレイアウト関連
        MainLayoutMenuFontSizePC = GetSystemParameter(sysParam, "MainLayoutMenuFontSizePC", MainLayoutMenuFontSizePC);
        MainLayoutMenuFontSizeHT = GetSystemParameter(sysParam, "MainLayoutMenuFontSizeHT", MainLayoutMenuFontSizeHT);
        MainLayoutMenuFontWeightPC = GetSystemParameter(sysParam, "MainLayoutMenuFontWeightPC", MainLayoutMenuFontWeightPC);
        MainLayoutMenuFontWeightHT = GetSystemParameter(sysParam, "MainLayoutMenuFontWeightHT", MainLayoutMenuFontWeightHT);
        MainLayoutAffiliationFontSizePC = GetSystemParameter(sysParam, "MainLayoutAffiliationFontSizePC", MainLayoutAffiliationFontSizePC);
        MainLayoutAffiliationFontWeightPC = GetSystemParameter(sysParam, "MainLayoutAffiliationFontWeightPC", MainLayoutAffiliationFontWeightPC);
        MainLayoutUserFontSizePC = GetSystemParameter(sysParam, "MainLayoutUserFontSizePC", MainLayoutUserFontSizePC);
        MainLayoutUserFontSizeHT = GetSystemParameter(sysParam, "MainLayoutUserFontSizeHT", MainLayoutUserFontSizeHT);
        MainLayoutUserFontWeightPC = GetSystemParameter(sysParam, "MainLayoutUserFontWeightPC", MainLayoutUserFontWeightPC);
        MainLayoutUserFontWeightHT = GetSystemParameter(sysParam, "MainLayoutUserFontWeightHT", MainLayoutUserFontWeightHT);

        // ログイン画面関連
        LoginFontSize = GetSystemParameter(sysParam, "Login_FontSize", LoginFontSize);
        LoginFontWeight = GetSystemParameter(sysParam, "Login_FontWeight", LoginFontWeight);
        LoginFontSizeBtn = GetSystemParameter(sysParam, "Login_FontSizeBtn", LoginFontSizeBtn);
        LoginFontWeightBtn = GetSystemParameter(sysParam, "Login_FontWeightBtn", LoginFontWeightBtn);

        // HT関連
        ColumnSize = GetSystemParameterInt(sysParam, "HT_TitleColumnSize", ColumnSize);
        FontSizeLabel = GetSystemParameter(sysParam, "HT_TitleFontSize", FontSizeLabel);
        FontSizeTextBox = GetSystemParameter(sysParam, "HT_TextBoxFontSize", FontSizeTextBox);
        FontSizeComb = GetSystemParameter(sysParam, "HT_CombBoxFontSize", FontSizeComb);
        HightComb = GetSystemParameter(sysParam, "HT_CombBoxHight", HightComb);
        FontWeightBold = GetSystemParameter(sysParam, "HT_FontWeightBold", FontWeightBold);
        BackColorBadge = GetSystemParameter(sysParam, "HT_MixedBackColor", BackColorBadge);
        FontSizeBadge = GetSystemParameter(sysParam, "HT_MixedFontSize", FontSizeBadge);
        LineHeightBadge = GetSystemParameter(sysParam, "HT_MixedLineHeight", LineHeightBadge);
        EmergencyColor = GetSystemParameter(sysParam, "HT_EmergencyColor", EmergencyColor);
        HTArrivalInspectStatusColor = GetSystemParameter(sysParam, "HT_ArrivalInspectStatusColor", HTArrivalInspectStatusColor);
        CaseBaraMargin = GetSystemParameter(sysParam, "HT_CaseBaraMargin", CaseBaraMargin);
        CardColumnSize = GetSystemParameterInt(sysParam, "HT_CardTitleColumnSize", CardColumnSize);
        CardFontSizeLabel = GetSystemParameter(sysParam, "HT_CardTitleFontSize", CardFontSizeLabel);
        CardFontSizeTextBox = GetSystemParameter(sysParam, "HT_CardTextBoxFontSize", CardFontSizeTextBox);
        CardFontWeightBold = GetSystemParameter(sysParam, "HT_CardFontWeightBold", CardFontWeightBold);
        CardCaseBaraMargin = GetSystemParameter(sysParam, "HT_CardCaseBaraMargin", CardCaseBaraMargin);
        CardCaseBaraContains = GetSystemParameter(sysParam, "HT_CardCaseBaraContains", CardCaseBaraContains);
        HT_GetUnfinishedDataFlg = GetSystemParameter(sysParam, "HT_GetUnfinishedDataFlg", HT_GetUnfinishedDataFlg);
        HT_GetUnfinishedArrivalIcon = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalIcon", HT_GetUnfinishedArrivalIcon);
        HT_GetUnfinishedArrivalBackGroundColor = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalBackGroundColor", HT_GetUnfinishedArrivalBackGroundColor);
        HT_GetUnfinishedArrivalColor = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalColor", HT_GetUnfinishedArrivalColor);
        HT_GetUnfinishedShipmentIcon = GetSystemParameter(sysParam, "HT_GetUnfinishedSipmentIcon", HT_GetUnfinishedShipmentIcon);
        HT_GetUnfinishedShipmentBackGroundColor = GetSystemParameter(sysParam, "HT_GetUnfinishedShipmentBackGroundColor", HT_GetUnfinishedShipmentBackGroundColor);
        HT_GetUnfinishedShipmentColor = GetSystemParameter(sysParam, "HT_GetUnfinishedShipmentColor", HT_GetUnfinishedShipmentColor);
        HT_GetUnfinishedDataInterval = GetSystemParameterInt(sysParam, "HT_GetUnfinishedDataInterval", HT_GetUnfinishedDataInterval);

        // HTメニュー関連
        MenuBottonMargin = GetSystemParameter(sysParam, "HT_MenuBottonMargin", MenuBottonMargin);
        MenuFontSize = GetSystemParameter(sysParam, "HT_MenuFontSize", MenuFontSize);
        MenuFontweight = GetSystemParameter(sysParam, "HT_MenuFontweight", MenuFontweight);
        MenuBackGroundColorCode = GetSystemParameter(sysParam, "HT_MenuBackGroundColorCode", MenuBackGroundColorCode);
        MenuForeColorCode = GetSystemParameter(sysParam, "HT_MenuForeColorCode", MenuForeColorCode);
        MenuBackGroundColorCodeForFocus = GetSystemParameter(sysParam, "HT_MenuBackGroundColorCodeForFocus", MenuBackGroundColorCodeForFocus);
        MenuForeColorCodeCodeForFocus = GetSystemParameter(sysParam, "HT_MenuForeColorCodeCodeForFocus", MenuForeColorCodeCodeForFocus);
        // HTブザー、バイブレーション関連
        HT_DefaultBuzzerTone = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerTone", HT_DefaultBuzzerTone);
        HT_DefaultBuzzerOnPeriod = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerOnPeriod", HT_DefaultBuzzerOnPeriod);
        HT_DefaultBuzzerOffPeriod = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerOffPeriod", HT_DefaultBuzzerOffPeriod);
        HT_DefaultBuzzerReratCount = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerReratCount", HT_DefaultBuzzerReratCount);
        HT_DefaultVibrationOnPeriod = GetSystemParameterInt(sysParam, "HT_DefaultVibrationOnPeriod", HT_DefaultVibrationOnPeriod);
        HT_DefaultVibrationOffPeriod = GetSystemParameterInt(sysParam, "HT_DefaultVibrationOffPeriod", HT_DefaultVibrationOffPeriod);
        HT_DefaultVibrationReratCount = GetSystemParameterInt(sysParam, "HT_DefaultVibrationReratCount", HT_DefaultVibrationReratCount);

        HT_ErrorBuzzerTone = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerTone", HT_ErrorBuzzerTone);
        HT_ErrorBuzzerOnPeriod = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerOnPeriod", HT_ErrorBuzzerOnPeriod);
        HT_ErrorBuzzerOffPeriod = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerOffPeriod", HT_ErrorBuzzerOffPeriod);
        HT_ErrorBuzzerReratCount = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerReratCount", HT_ErrorBuzzerReratCount);
        HT_ErrorVibrationOnPeriod = GetSystemParameterInt(sysParam, "HT_ErrorVibrationOnPeriod", HT_ErrorVibrationOnPeriod);
        HT_ErrorVibrationOffPeriod = GetSystemParameterInt(sysParam, "HT_ErrorVibrationOffPeriod", HT_ErrorVibrationOffPeriod);
        HT_ErrorVibrationReratCount = GetSystemParameterInt(sysParam, "HT_ErrorVibrationReratCount", HT_ErrorVibrationReratCount);

        // データグリッド関係
        DataGridHeaderFontSizeHT = GetSystemParameter(sysParam, "DataGridHeaderFontSizeHT", DataGridHeaderFontSizeHT);
        DataGridHeaderFontSizePC = GetSystemParameter(sysParam, "DataGridHeaderFontSizePC", DataGridHeaderFontSizePC);
        DataGridColumnFontSizeHT = GetSystemParameter(sysParam, "DataGridColumnFontSizeHT", DataGridColumnFontSizeHT);
        DataGridColumnFontSizePC = GetSystemParameter(sysParam, "DataGridColumnFontSizePC", DataGridColumnFontSizePC);
        DataGridCellFontSizeHT = GetSystemParameter(sysParam, "DataGridCellFontSizeHT", DataGridCellFontSizeHT);
        DataGridCellFontSizePC = GetSystemParameter(sysParam, "DataGridCellFontSizePC", DataGridCellFontSizePC);
        DataGridCellFontWeightHT = GetSystemParameter(sysParam, "DataGridCellFontWeightHT", DataGridCellFontWeightHT);
        DataGridCellFontWeightPC = GetSystemParameter(sysParam, "DataGridCellFontWeightPC", DataGridCellFontWeightPC);
        DataGridFooterFontSizeHT = GetSystemParameter(sysParam, "DataGridFooterFontSizeHT", DataGridFooterFontSizeHT);
        DataGridFooterFontSizePC = GetSystemParameter(sysParam, "DataGridFooterFontSizePC", DataGridFooterFontSizePC);
        DataGridHeightHT = GetSystemParameter(sysParam, "DataGridHeightHT", DataGridHeightHT);
        DataGridHeightPC = GetSystemParameter(sysParam, "DataGridHeightPC", DataGridHeightPC);

        //バリデーション関連
        PC_ValidationSummaryFontSize = GetSystemParameter(sysParam, "PC_ValidationSummaryFontSize", PC_ValidationSummaryFontSize);
        HT_ValidationSummaryFontSize = GetSystemParameter(sysParam, "HT_ValidationSummaryFontSize", HT_ValidationSummaryFontSize);
        PC_ValidationSummaryFontWeight = GetSystemParameter(sysParam, "PC_ValidationSummaryFontWeight", PC_ValidationSummaryFontWeight);
        HT_ValidationSummaryFontWeight = GetSystemParameter(sysParam, "HT_ValidationSummaryFontWeight", HT_ValidationSummaryFontWeight);

        // PCコンポーネント関連
        PC_GroupFieldTitleFontSize = GetSystemParameter(sysParam, "PC_GroupFieldTitleFontSize", PC_GroupFieldTitleFontSize);
        PC_GroupFieldTitleFontWeight = GetSystemParameter(sysParam, "PC_GroupFieldTitleFontWeight", PC_GroupFieldTitleFontWeight);
        PC_GroupFieldLabelFontSize = GetSystemParameter(sysParam, "PC_GroupFieldLabelFontSize", PC_GroupFieldLabelFontSize);
        PC_GroupFieldLabelFontWeight = GetSystemParameter(sysParam, "PC_GroupFieldLabelFontWeight", PC_GroupFieldLabelFontWeight);
        PC_GroupFieldLabelWidth = GetSystemParameter(sysParam, "PC_GroupFieldLabelWidth", PC_GroupFieldLabelWidth);
        PC_TextBoxFontSize = GetSystemParameter(sysParam, "PC_TextBoxFontSize", PC_TextBoxFontSize);
        PC_TextBoxFontWeight = GetSystemParameter(sysParam, "PC_TextBoxFontWeight", PC_TextBoxFontWeight);
        PC_TextBoxWidth = GetSystemParameter(sysParam, "PC_TextBoxWidth", PC_TextBoxWidth);
        PC_TextBoxHeight = GetSystemParameter(sysParam, "PC_TextBoxHeight", PC_TextBoxHeight);
        PC_TextAreaFontSize = GetSystemParameter(sysParam, "PC_TextAreaFontSize", PC_TextAreaFontSize);
        PC_TextAreaFontWeight = GetSystemParameter(sysParam, "PC_TextAreaFontWeight", PC_TextAreaFontWeight);
        PC_DatePickerFontSize = GetSystemParameter(sysParam, "PC_DatePickerFontSize", PC_DatePickerFontSize);
        PC_DatePickerFontWeight = GetSystemParameter(sysParam, "PC_DatePickerFontWeight", PC_DatePickerFontWeight);
        PC_DatePickerWidth = GetSystemParameter(sysParam, "PC_DatePickerWidth", PC_DatePickerWidth);
        PC_DatePickerHeight = GetSystemParameter(sysParam, "PC_DatePickerHeight", PC_DatePickerHeight);
        PC_TimePickerFontSize = GetSystemParameter(sysParam, "PC_TimePickerFontSize", PC_TimePickerFontSize);
        PC_TimePickerFontWeight = GetSystemParameter(sysParam, "PC_TimePickerFontWeight", PC_TimePickerFontWeight);
        PC_TimePickerWidth = GetSystemParameter(sysParam, "PC_TimePickerWidth", PC_TimePickerWidth);
        PC_TimePickerHeight = GetSystemParameter(sysParam, "PC_TimePickerHeight", PC_TimePickerHeight);
        PC_DateTimePickerFontSize = GetSystemParameter(sysParam, "PC_DateTimePickerFontSize", PC_DateTimePickerFontSize);
        PC_DateTimePickerFontWeight = GetSystemParameter(sysParam, "PC_DateTimePickerFontWeight", PC_DateTimePickerFontWeight);
        PC_DateTimePickerWidth = GetSystemParameter(sysParam, "PC_DateTimePickerWidth", PC_DateTimePickerWidth);
        PC_DateTimePickerHeight = GetSystemParameter(sysParam, "PC_DateTimePickerHeight", PC_DateTimePickerHeight);
        PC_DropDownFontSize = GetSystemParameter(sysParam, "PC_DropDownFontSize", PC_DropDownFontSize);
        PC_DropDownFontWeight = GetSystemParameter(sysParam, "PC_DropDownFontWeight", PC_DropDownFontWeight);
        PC_DropDownWidth = GetSystemParameter(sysParam, "PC_DropDownWidth", PC_DropDownWidth);
        PC_DropDownHeight = GetSystemParameter(sysParam, "PC_DropDownHeight", PC_DropDownHeight);
        PC_CheckBoxFontSize = GetSystemParameter(sysParam, "PC_CheckBoxFontSize", PC_CheckBoxFontSize);
        PC_CheckBoxFontWeight = GetSystemParameter(sysParam, "PC_CheckBoxFontWeight", PC_CheckBoxFontWeight);
        PC_CheckBoxTitleFontSize = GetSystemParameter(sysParam, "PC_CheckBoxTitleFontSize", PC_CheckBoxTitleFontSize);
        PC_CheckBoxTitleFontWeight = GetSystemParameter(sysParam, "PC_CheckBoxTitleFontWeight", PC_CheckBoxTitleFontWeight);
        PC_RadioButtonFontSize = GetSystemParameter(sysParam, "PC_RadioButtonFontSize", PC_RadioButtonFontSize);
        PC_RadioButtonFontWeight = GetSystemParameter(sysParam, "PC_RadioButtonFontWeight", PC_RadioButtonFontWeight);
        PC_RadioButtonTitleFontSize = GetSystemParameter(sysParam, "PC_RadioButtonTitleFontSize", PC_RadioButtonTitleFontSize);
        PC_RadioButtonTitleFontWeight = GetSystemParameter(sysParam, "PC_RadioButtonTitleFontWeight", PC_RadioButtonTitleFontWeight);
        PC_NumericFontSize = GetSystemParameter(sysParam, "PC_NumericFontSize", PC_NumericFontSize);
        PC_NumericFontWeight = GetSystemParameter(sysParam, "PC_NumericFontWeight", PC_NumericFontWeight);
        PC_NumericWidth = GetSystemParameter(sysParam, "PC_NumericWidth", PC_NumericWidth);
        PC_NumericHeight = GetSystemParameter(sysParam, "PC_NumericHeight", PC_NumericHeight);
        PC_DropDownDataGridFontSize = GetSystemParameter(sysParam, "PC_DropDownDataGridFontSize", PC_DropDownDataGridFontSize);
        PC_DropDownDataGridFontWeight = GetSystemParameter(sysParam, "PC_DropDownDataGridFontWeight", PC_DropDownDataGridFontWeight);
        PC_TabsExtendFontSize = GetSystemParameter(sysParam, "PC_TabsExtendFontSize", PC_TabsExtendFontSize);
        PC_TabsExtendFontWeight = GetSystemParameter(sysParam, "PC_TabsExtendFontWeight", PC_TabsExtendFontWeight);
        PC_DateInitWmsAddDays = GetSystemParameterInt(sysParam, "PC_DateInitWmsAddDays", PC_DateInitWmsAddDays);

        // 必須表示の付加文字
        RequiredDisplaySuffixHT = GetSystemParameter(sysParam, "RequiredDisplaySuffixHT", RequiredDisplaySuffixHT);
        RequiredDisplaySuffixPC = GetSystemParameter(sysParam, "RequiredDisplaySuffixPC", RequiredDisplaySuffixPC);

        // 出荷開始警告時間
        ShipmentsStartWarnTime = GetSystemParameterInt(sysParam, "ShipmentsStartWarnTime", ShipmentsStartWarnTime);
        // 出荷締切警告時間
        ShipmentsDeadlineWarnTime = GetSystemParameterInt(sysParam, "ShipmentsDeadlineWarnTime", ShipmentsDeadlineWarnTime);
        //Notify表示時間
        NotifyPopupDuration = GetSystemParameterInt(sysParam, "NotifyPopupDuration", NotifyPopupDuration);

        //2023/12/27 追加

        //PC,HT画面のボタンのmargin-bottomの高さ
        PC_ButtonMarginBottom = GetSystemParameter(sysParam, "PC_ButtonMarginBottom", PC_ButtonMarginBottom);
        HT_ButtonMarginBottom = GetSystemParameter(sysParam, "HT_ButtonMarginBottom", HT_ButtonMarginBottom);
        //HT画面のファンクションボタンの背景色の色
        HT_Button1BackgroundColor = GetSystemParameter(sysParam, "HT_Button1BackgroundColor", HT_Button1BackgroundColor);
        HT_Button2BackgroundColor = GetSystemParameter(sysParam, "HT_Button2BackgroundColor", HT_Button2BackgroundColor);
        HT_Button3BackgroundColor = GetSystemParameter(sysParam, "HT_Button3BackgroundColor", HT_Button3BackgroundColor);
        HT_Button4BackgroundColor = GetSystemParameter(sysParam, "HT_Button4BackgroundColor", HT_Button4BackgroundColor);
        //HT画面のファンクションボタンの文字の色
        HT_Button1TextColor = GetSystemParameter(sysParam, "HT_Button1TextColor", HT_Button1TextColor);
        HT_Button4TextColor = GetSystemParameter(sysParam, "HT_Button4TextColor", HT_Button4TextColor);

        //PC　DropDownDataGridのチェックボックスの幅
        PC_DropDownDataGridCheckBoxWidth = GetSystemParameter(sysParam, "PC_DropDownDataGridCheckBoxWidth", PC_DropDownDataGridCheckBoxWidth);

        //PC 必須表示の付加文字の色
        RequiredDisplaySuffixColorPC = GetSystemParameter(sysParam, "RequiredDisplaySuffixColorPC", RequiredDisplaySuffixColorPC);

        //HTメニューボタンの幅の大きさ
        HT_MenuBottonWidth = GetSystemParameter(sysParam, "HT_MenuBottonWidth", HT_MenuBottonWidth);

        //コンポーネントのテキストエリアの行数,列数
        CompTextAreaRows = GetSystemParameterInt(sysParam, "CompTextAreaRows", CompTextAreaRows);
        CompTextAreaCols = GetSystemParameterInt(sysParam, "CompTextAreaCols", CompTextAreaCols);

        //ログインフォームのタイトルと入力欄のColumn値
        LoginFormTitleColumnSize = GetSystemParameterInt(sysParam, "LoginFormTitleColumnSize", LoginFormTitleColumnSize);
        LoginFormTextBoxColumnSize = GetSystemParameterInt(sysParam, "LoginFormTextBoxColumnSize", LoginFormTextBoxColumnSize);

        //HT　データカードのmarginの値
        HT_DataCardContentMarginTop = GetSystemParameter(sysParam, "HT_DataCardContentMarginTop", HT_DataCardContentMarginTop);
        HT_DataCardContentMarginLeft = GetSystemParameter(sysParam, "HT_DataCardContentMarginLeft", HT_DataCardContentMarginLeft);
        HT_DataCardContentMarginRight = GetSystemParameter(sysParam, "HT_DataCardContentMarginRight", HT_DataCardContentMarginRight);
        HT_DataCardContentMarginBottom = GetSystemParameter(sysParam, "HT_DataCardContentMarginBottom", HT_DataCardContentMarginBottom);
        //HT　データカード内のStackのmarginの値
        HT_InsideDataCardContentMarginTop = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginTop", HT_InsideDataCardContentMarginTop);
        HT_InsideDataCardContentMarginLeft = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginLeft", HT_InsideDataCardContentMarginLeft);
        HT_InsideDataCardContentMarginRight = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginRight", HT_InsideDataCardContentMarginRight);
        //HT　データカードリストの高さ
        HT_DataCardListHeight = GetSystemParameter(sysParam, "HT_DataCardListHeight", HT_DataCardListHeight);

        //HT　データグリッド内のバラの色
        HT_DataGridBaraColor = GetSystemParameter(sysParam, "HT_DataGridBaraColor", HT_DataGridBaraColor);
        //HT ロケーションコンボボックスの最大値件数。
        HT_LocComBoxMaxCount = GetSystemParameterInt(sysParam, "HT_LocComBoxMaxCount", HT_LocComBoxMaxCount);

        //PC　進捗管理の凡例のマーク,テキストの幅
        PC_DataGridProgressMarkWidth = GetSystemParameterInt(sysParam, "PC_DataGridProgressMarkWidth", PC_DataGridProgressMarkWidth);
        PC_DataGridProgressTextWidth = GetSystemParameterInt(sysParam, "PC_DataGridProgressTextWidth", PC_DataGridProgressTextWidth);

        //メッセージダイアログの幅,高さ（処理中、読込中）
        MessageDialogWidth = GetSystemParameterInt(sysParam, "MessageDialogWidth", MessageDialogWidth);
        MessageDialogHeight = GetSystemParameterInt(sysParam, "MessageDialogHeight", MessageDialogHeight);
        //確認メッセージダイアログの幅（OKのみ）
        DialogShowOKWidth = GetSystemParameterInt(sysParam, "DialogShowOKWidth", DialogShowOKWidth);
        DialogShowOKHeight = GetSystemParameterInt(sysParam, "DialogShowOKHeight", DialogShowOKHeight);
        //確認メッセージダイアログの幅（YesとNo）
        DialogShowYesNoWidth = GetSystemParameterInt(sysParam, "DialogShowYesNoWidth", DialogShowYesNoWidth);
        DialogShowYesNoHeight = GetSystemParameterInt(sysParam, "DialogShowYesNoHeight", DialogShowYesNoHeight);

        //編集ダイアログの備考の最大入力数,行,列数
        DialogContentMaxlength = GetSystemParameterInt(sysParam, "DialogContentMaxlength", DialogContentMaxlength);
        DialogContentCols = GetSystemParameterInt(sysParam, "DialogContentCols", DialogContentCols);
        DialogContentRows = GetSystemParameterInt(sysParam, "DialogContentRows", DialogContentRows);

        //編集ダイアログの概要の文字数
        DialogContentRemarksRows = GetSystemParameterInt(sysParam, "DialogContentRemarksRows", DialogContentRemarksRows);

        //データグリッドのページ数
        DataGridPageSize = GetSystemParameterInt(sysParam, "DataGridPageSize", DataGridPageSize);

        MessageDialogWidth = MessageDialogWidth;
        MessageDialogHeight = MessageDialogHeight;

        DialogShowOKWidth = DialogShowOKWidth;
        DialogShowOKHeight = DialogShowOKHeight;

        DialogShowYesNoWidth = DialogShowYesNoWidth;
        DialogShowYesNoHeight = DialogShowYesNoHeight;

        ColorBKGridCellContainColumnName = GetSystemParameter(sysParam, "ColorBKGridCellContainColumnName", ColorBKGridCellContainColumnName);
        ColorBKGridRowContainColumnName = GetSystemParameter(sysParam, "ColorBKGridRowContainColumnName", ColorBKGridRowContainColumnName);
        ColorFRGridCellContainColumnName = GetSystemParameter(sysParam, "ColorFRGridCellContainColumnName", ColorFRGridCellContainColumnName);
        ColorFRGridRowContainColumnName = GetSystemParameter(sysParam, "ColorFRGridRowContainColumnName", ColorFRGridRowContainColumnName);
        ColorGridColumnNameSplitString = GetSystemParameter(sysParam, "ColorGridColumnNameSplitString", ColorGridColumnNameSplitString);
        UnSelectableGridCellContainColumnName = GetSystemParameter(sysParam, "UnSelectableGridCellContainColumnName", UnSelectableGridCellContainColumnName);

        StepTitleDisplay = GetSystemParameter(sysParam, "StepTitleDisplay", StepTitleDisplay);
        StepNumberDisplay = GetSystemParameter(sysParam, "StepNumberDisplay", StepNumberDisplay);
        StepUlDisplay = GetSystemParameter(sysParam, "StepUlDisplay", StepUlDisplay);
        //return this;//自分自身を返す
    }

    private string GetSystemParameter(Dictionary<string, SystemParameter> sysParam, string key, string def)
    {
        string val = def;
        if (sysParam.TryGetValue(key, out SystemParameter? param))
        {
            if (null != param)
            {
                val = param.ParameterValue;
            }
        }
        return val;
    }
    private int GetSystemParameterInt(Dictionary<string, SystemParameter> sysParam, string key, int def)
    {
        int val = def;
        if (sysParam.TryGetValue(key, out SystemParameter? param))
        {
            if (null != param)
            {
                val = ConvertUtil.GetValueInt(param.ParameterValue);
            }
        }
        return val;
    }

    //TODO 念のため、セッションストレージから取得するメソッド、セットするメソッドを作成しておく。
    public async Task<SystemParameterService> GenerateSystemParametersObj()
    {
        //パラメータを詰めたオブジェクトを作成する。
        //TODO DBから再度取得せず、値のみをクローンするような処理にしたい。

        SystemParameterService s = new(_webComService);
        await s.LoadSystemParameters();
        return s;
    }

}
