using Blazored.LocalStorage;
using Blazored.SessionStorage;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Services
{
    public class CommonService : WebAPIService
    {
        #region Enum定義
        private enum TYPE_DEVICE_GROUP_ID
        {
            NONE = 0,
            PC = 1,
            HT = 2,
        }
        #endregion

        // 画面毎の設定関連情報はログイン時に保持しておく
        public Dictionary<string, string> PageNameAll { get; set; }
        public Dictionary<string, List<ComponentsInfo>> ComponentsInfoAll { get; set; }
        public Dictionary<string, List<ComponentColumnsInfo>> ComponentColumnsAll { get; set; }
        public Dictionary<string, List<ComponentProgramInfo>> ComponentProgramAll { get; set; }

        // 画面ごとに読み込まないマスタ情報を保持
        public List<MstAreaData> MstAreaInfoAll { get; set; } = new List<MstAreaData>();
        public List<MstZoneData> MstZoneInfoAll { get; set; } = new List<MstZoneData>();
        public List<MstLocationData> MstLocationInfoAll { get; set; } = new List<MstLocationData>();

        //ここの値のみDBから取得不可

        //メッセージダイアログの幅,高さ（処理中、読込中）
        public int MessageDialogWidth { get; set; } = 200;
        public int MessageDialogHeight { get; set; } = 155;
        //確認メッセージダイアログの幅,高さ（OKのみ）
        public int DialogShowOKWidth { get; set; } = 350;
        public int DialogShowOKHeight { get; set; } = 200;
        //確認メッセージダイアログの幅（YesとNo）
        public int DialogShowYesNoWidth { get; set; } = 350;
        public int DialogShowYesNoHeight { get; set; } = 200;

        private readonly string strUrl = $"/Common";
        private readonly string strExecutionUrl = $"/Execution";
        /// <summary>
        /// ILocalStorageService
        /// </summary>
        private readonly ILocalStorageService _localStorage;
        /// <summary>
        /// ISessionStorageService
        /// </summary>
        private readonly ISessionStorageService _sessionStorage;
        /// <summary>
        /// DialogService
        /// </summary>
        private readonly DialogService _dialogService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="httpClient"></param>
        public CommonService(HttpClient httpClient, ILocalStorageService localStorage, ISessionStorageService sessionStorage, DialogService dialogService) : base(httpClient)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _dialogService = dialogService;
        }

        public async Task<ResponseValue[]?> GetResponseValue(ClassNameSelect select, string path = "", int timeout = 100000)
        {
            // データを取得する場合、WHERE句の条件に支所場コード、所場区分、荷主IDは必ず追加するように対応
            // WebAPI側でSELECTする列情報に支所場コード等が含まれている場合は、WHERE句で使用するようにしています。
            if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
            {
                SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
                select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
                select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
                select.whereParam[SharedConst.KEY_CONSIGNOR_ID] = new WhereParam { val = sysParams.ConsignorId };
            }
            return await GetResponseValue(select, strUrl, path, timeout);
        }

        public async Task<ExecResult[]?> SetRequestValue(string className, RequestValue request, string path = "", int timeout = 100000)
        {
            // クラス名を設定
            _ = request.SetArgumentValue(SharedConst.KEY_CLASS_NAME, className, "");
            // ストアドを実行するときは、ユーザID、支所場コード、所場区分、荷主IDは必ず追加するように対応
            // ストアドに必要な場合は、DEFINE_PROCESS_FUNCTIONのARGUMENT_NAME1～30にそれぞれ設定すること
            // WebAPI側でARGUMENT_NAME1～30に設定されている名称がキーとなっている値を使用してストアドを実行しています。
            if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
            {
                LoginInfo login = await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
                _ = request.SetArgumentValue(SharedConst.KEY_USER_ID, login.Id, "");
            }
            if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
            {
                SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
                _ = request.SetArgumentValue(SharedConst.KEY_BASE_ID, sysParams.BaseId, "");
                _ = request.SetArgumentValue(SharedConst.KEY_BASE_TYPE, sysParams.BaseType.ToString(), "");
                _ = request.SetArgumentValue(SharedConst.KEY_CONSIGNOR_ID, sysParams.ConsignorId, "");
            }
            return await base.SetRequestValue(request, strExecutionUrl, path, timeout);
        }

        /// <summary>
        /// ログインストアド実行
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecLoginFunc(string className, string versionString)
        {
            RequestValue rv
                = RequestValue.CreateRequestProgram("ユーザーログイン")
                .SetArgumentValue("CLIENT_APP_VERSION", versionString, "");
            ExecResult[]? results = await SetRequestValue(className, rv);

            if (results == null || results.Length <= 0)
            {
                //TODO webAPI異常終了。電波強度の確認メッセージを表示したい
                return false;
            }
            //ストアドでエラー扱いの場合は、エラーメッセージを表示する
            if (results.Min(_ => _.RetCode) < 0)
            {
                // ログイン失敗メッセージ
                await DialogShowOK(results[0].Message, height: 260);//TODO 高さは暫定
                return false;
            }
            return true;
        }

        /// <summary>
        /// ログアウトストアド実行
        /// </summary>
        /// <returns></returns>
        public async Task ExecLogoutFunc(string className, bool isCancelIgnore = false)
        {
            RequestValue rv = RequestValue.CreateRequestProgram("ユーザーログアウト");
            rv.IsCancelTokenIgnore = isCancelIgnore;
            _ = await SetRequestValue(className, rv);
        }
        /// <summary>
        /// 排他ロック情報削除ストアドの実行
        /// </summary>
        /// <param name="className"></param>
        /// <param name="isCancelIgnore"></param>
        /// <returns></returns>
        public async Task ExecDleteLockInfoFunc(string className, bool isCancelIgnore = false)
        {
            RequestValue rv = RequestValue.CreateRequestProgram("排他ロック情報削除");
            rv.IsCancelTokenIgnore = isCancelIgnore;
            _ = await SetRequestValue(className, rv);
        }

        /// <summary>
        /// IDから採番された文字列を取得する
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<string> GetManagementId(string className, SharedConst.workCategory category)
        {
            string id = string.Empty;
            RequestValue rv = RequestValue.CreateRequestProgram(SharedConst.KEY_PROGRAM_NAME_MANAGEMENT_ID);
            _ = rv.SetArgumentValue("WORK_CATEGORY", (short)category, "");
            ExecResult[]? results = await SetRequestValue(className, rv);
            if (results != null && results.Length > 0)
            {
                id = results[0].Message;
            }
            return id;
        }

        /// <summary>
        /// MST_ZONEテーブル情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstAreaData>> GetArea(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
        {
            ClassNameSelect select = new()
            {
                viewName = "MST_AREA"
            };
            SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
            select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
            if (wpList is not null)
            {
                foreach ((string key, WhereParam wp) in wpList)
                {
                    select.whereParam.Add(key, wp);
                }
            }
            select.orderByParam.Add(new OrderByParam { field = "AREA_ID" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<MstAreaData> data = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetArea_resItems is null");
            }

            foreach (ResponseValue item in resItems)
            {
                MstAreaData newRow = new()
                {
                    AreaId = GetValueString(item, "AREA_ID"),
                    AreaName = dispId ? GetValueString(item, "AREA_ID") + " " + GetValueString(item, "AREA_NAME") : GetValueString(item, "AREA_NAME"),
                };

                data.Add(newRow);
            }

            return data;
        }

        /// <summary>
        /// MST_ZONEテーブル情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstZoneData>> GetZone(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
        {
            ClassNameSelect select = new()
            {
                viewName = "MST_ZONE"
            };
            SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
            select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
            if (wpList is not null)
            {
                foreach ((string key, WhereParam wp) in wpList)
                {
                    select.whereParam.Add(key, wp);
                }
            }
            select.orderByParam.Add(new OrderByParam { field = "AREA_ID" });
            select.orderByParam.Add(new OrderByParam { field = "ZONE_ID" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<MstZoneData> data = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetZone_resItems is null");
            }

            foreach (ResponseValue item in resItems)
            {
                MstZoneData newRow = new()
                {
                    AreaId = GetValueString(item, "AREA_ID"),
                    ZoneId = GetValueString(item, "ZONE_ID"),
                    ZoneName = dispId ? GetValueString(item, "ZONE_ID") + " " + GetValueString(item, "ZONE_NAME") : GetValueString(item, "ZONE_NAME"),
                };

                data.Add(newRow);
            }

            return data;
        }

        /// <summary>
        /// MST_LOCATIONSテーブル情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstLocationData>> GetLocation(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
        {
            ClassNameSelect select = new()
            {
                viewName = "MST_LOCATIONS"
            };
            SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
            select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
            if (wpList is not null)
            {
                foreach ((string key, WhereParam wp) in wpList)
                {
                    select.whereParam.Add(key, wp);
                }
            }
            select.orderByParam.Add(new OrderByParam { field = "AREA_ID" });
            select.orderByParam.Add(new OrderByParam { field = "ZONE_ID" });
            select.orderByParam.Add(new OrderByParam { field = "LOCATION_ID" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<MstLocationData> data = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetLocation_resItems is null");
            }

            foreach (ResponseValue item in resItems)
            {
                MstLocationData newRow = new()
                {
                    AreaId = GetValueString(item, "AREA_ID"),
                    ZoneId = GetValueString(item, "ZONE_ID"),
                    LocationId = GetValueString(item, "LOCATION_ID"),
                    LocationName = dispId ? GetValueString(item, "LOCATION_ID") + " " + GetValueString(item, "LOCATION_NAME") : GetValueString(item, "LOCATION_NAME"),
                };

                data.Add(newRow);
            }

            return data;
        }

        /// <summary>
        /// DEFINE_SYSTEM_PARAMETERSテーブル情報を取得する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<SystemParameter> GetSystemParameter()
        {
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_SYSTEM_PARAMETERS"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            ResponseValue[]? resItems = await GetResponseValue(select);
            SystemParameter data = new();
            List<SystemParameters> sysParam = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetSystemParameter_resItems is null");
            }

            foreach (ResponseValue item in resItems)
            {
                SystemParameters newRow = new()
                {
                    ParameterKey = GetValueString(item, "PARAMETER_KEY"),
                    KeyName = GetValueString(item, "KEY_NAME"),
                    ParameterValue = GetValueString(item, "PARAMETER_VALUE"),
                };

                sysParam.Add(newRow);
            }

            // DEFINE_SYSTEM_PARAMETERSテーブルのキー情報から情報クラスに展開する
            data.BaseId = GetSystemParameter(sysParam, "BASE_ID_const", data.BaseId);
            data.BaseType = GetSystemParameterInt(sysParam, "BASE_TYPE_const", data.BaseType);
            data.ConsignorId = GetSystemParameter(sysParam, "CONSIGNOR_ID_const", data.ConsignorId);

            // 自動ログアウト関連
            data.AutoLogoutDeviceGroup = GetSystemParameterInt(sysParam, "AutoLogoutDeviceGroup", data.AutoLogoutDeviceGroup);
            data.AutoLogoutTime = GetSystemParameterInt(sysParam, "AutoLogoutTime", data.AutoLogoutTime);
            data.AutoLogoutCheckInterval = GetSystemParameterInt(sysParam, "AutoLogoutCheckInterval", data.AutoLogoutCheckInterval);

            // システム状態監視間隔
            data.MonitorSystemStatusInterval = GetSystemParameterInt(sysParam, "MonitorSystemStatusInterval", data.MonitorSystemStatusInterval);

            //ピッキング予定リフレッシュ間隔
            data.MonitorPickScheduleRefleshInterval = GetSystemParameterInt(sysParam, "MonitorPickScheduleRefleshInterval", data.MonitorPickScheduleRefleshInterval);

            // ログ通知周期
            data.LogNotifyInterval = GetSystemParameterInt(sysParam, "LogNotifyInterval", data.LogNotifyInterval);

            // ログインスキャン時ユーザコード判断文字列
            data.LogonReadCodeChar = GetSystemParameter(sysParam, "LogonReadCodeChar", data.LogonReadCodeChar);

            // メインレイアウト関連
            data.MainLayoutMenuFontSizePC = GetSystemParameter(sysParam, "MainLayoutMenuFontSizePC", data.MainLayoutMenuFontSizePC);
            data.MainLayoutMenuFontSizeHT = GetSystemParameter(sysParam, "MainLayoutMenuFontSizeHT", data.MainLayoutMenuFontSizeHT);
            data.MainLayoutMenuFontWeightPC = GetSystemParameter(sysParam, "MainLayoutMenuFontWeightPC", data.MainLayoutMenuFontWeightPC);
            data.MainLayoutMenuFontWeightHT = GetSystemParameter(sysParam, "MainLayoutMenuFontWeightHT", data.MainLayoutMenuFontWeightHT);
            data.MainLayoutAffiliationFontSizePC = GetSystemParameter(sysParam, "MainLayoutAffiliationFontSizePC", data.MainLayoutAffiliationFontSizePC);
            data.MainLayoutAffiliationFontWeightPC = GetSystemParameter(sysParam, "MainLayoutAffiliationFontWeightPC", data.MainLayoutAffiliationFontWeightPC);
            data.MainLayoutUserFontSizePC = GetSystemParameter(sysParam, "MainLayoutUserFontSizePC", data.MainLayoutUserFontSizePC);
            data.MainLayoutUserFontSizeHT = GetSystemParameter(sysParam, "MainLayoutUserFontSizeHT", data.MainLayoutUserFontSizeHT);
            data.MainLayoutUserFontWeightPC = GetSystemParameter(sysParam, "MainLayoutUserFontWeightPC", data.MainLayoutUserFontWeightPC);
            data.MainLayoutUserFontWeightHT = GetSystemParameter(sysParam, "MainLayoutUserFontWeightHT", data.MainLayoutUserFontWeightHT);

            // ログイン画面関連
            data.LoginFontSize = GetSystemParameter(sysParam, "Login_FontSize", data.LoginFontSize);
            data.LoginFontWeight = GetSystemParameter(sysParam, "Login_FontWeight", data.LoginFontWeight);
            data.LoginFontSizeBtn = GetSystemParameter(sysParam, "Login_FontSizeBtn", data.LoginFontSizeBtn);
            data.LoginFontWeightBtn = GetSystemParameter(sysParam, "Login_FontWeightBtn", data.LoginFontWeightBtn);

            // HT関連
            data.ColumnSize = GetSystemParameterInt(sysParam, "HT_TitleColumnSize", data.ColumnSize);
            data.FontSizeLabel = GetSystemParameter(sysParam, "HT_TitleFontSize", data.FontSizeLabel);
            data.FontSizeTextBox = GetSystemParameter(sysParam, "HT_TextBoxFontSize", data.FontSizeTextBox);
            data.FontSizeComb = GetSystemParameter(sysParam, "HT_CombBoxFontSize", data.FontSizeComb);
            data.HightComb = GetSystemParameter(sysParam, "HT_CombBoxHight", data.HightComb);
            data.FontWeightBold = GetSystemParameter(sysParam, "HT_FontWeightBold", data.FontWeightBold);
            data.BackColorBadge = GetSystemParameter(sysParam, "HT_MixedBackColor", data.BackColorBadge);
            data.FontSizeBadge = GetSystemParameter(sysParam, "HT_MixedFontSize", data.FontSizeBadge);
            data.LineHeightBadge = GetSystemParameter(sysParam, "HT_MixedLineHeight", data.LineHeightBadge);
            data.EmergencyColor = GetSystemParameter(sysParam, "HT_EmergencyColor", data.EmergencyColor);
            data.HTArrivalInspectStatusColor = GetSystemParameter(sysParam, "HT_ArrivalInspectStatusColor", data.HTArrivalInspectStatusColor);
            data.CaseBaraMargin = GetSystemParameter(sysParam, "HT_CaseBaraMargin", data.CaseBaraMargin);
            data.CardColumnSize = GetSystemParameterInt(sysParam, "HT_CardTitleColumnSize", data.CardColumnSize);
            data.CardFontSizeLabel = GetSystemParameter(sysParam, "HT_CardTitleFontSize", data.CardFontSizeLabel);
            data.CardFontSizeTextBox = GetSystemParameter(sysParam, "HT_CardTextBoxFontSize", data.CardFontSizeTextBox);
            data.CardFontWeightBold = GetSystemParameter(sysParam, "HT_CardFontWeightBold", data.CardFontWeightBold);
            data.CardCaseBaraMargin = GetSystemParameter(sysParam, "HT_CardCaseBaraMargin", data.CardCaseBaraMargin);
            data.CardCaseBaraContains = GetSystemParameter(sysParam, "HT_CardCaseBaraContains", data.CardCaseBaraContains);
            data.HT_GetUnfinishedDataFlg = GetSystemParameter(sysParam, "HT_GetUnfinishedDataFlg", data.HT_GetUnfinishedDataFlg);
            data.HT_GetUnfinishedArrivalIcon = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalIcon", data.HT_GetUnfinishedArrivalIcon);
            data.HT_GetUnfinishedArrivalBackGroundColor = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalBackGroundColor", data.HT_GetUnfinishedArrivalBackGroundColor);
            data.HT_GetUnfinishedArrivalColor = GetSystemParameter(sysParam, "HT_GetUnfinishedArrivalColor", data.HT_GetUnfinishedArrivalColor);
            data.HT_GetUnfinishedShipmentIcon = GetSystemParameter(sysParam, "HT_GetUnfinishedSipmentIcon", data.HT_GetUnfinishedShipmentIcon);
            data.HT_GetUnfinishedShipmentBackGroundColor = GetSystemParameter(sysParam, "HT_GetUnfinishedShipmentBackGroundColor", data.HT_GetUnfinishedShipmentBackGroundColor);
            data.HT_GetUnfinishedShipmentColor = GetSystemParameter(sysParam, "HT_GetUnfinishedShipmentColor", data.HT_GetUnfinishedShipmentColor);
            data.HT_GetUnfinishedDataInterval = GetSystemParameterInt(sysParam, "HT_GetUnfinishedDataInterval", data.HT_GetUnfinishedDataInterval);

            // HTメニュー関連
            data.MenuBottonMargin = GetSystemParameter(sysParam, "HT_MenuBottonMargin", data.MenuBottonMargin);
            data.MenuFontSize = GetSystemParameter(sysParam, "HT_MenuFontSize", data.MenuFontSize);
            data.MenuFontweight = GetSystemParameter(sysParam, "HT_MenuFontweight", data.MenuFontweight);
            data.MenuBackGroundColorCode = GetSystemParameter(sysParam, "HT_MenuBackGroundColorCode", data.MenuBackGroundColorCode);
            data.MenuForeColorCode = GetSystemParameter(sysParam, "HT_MenuForeColorCode", data.MenuForeColorCode);
            data.MenuBackGroundColorCodeForFocus = GetSystemParameter(sysParam, "HT_MenuBackGroundColorCodeForFocus", data.MenuBackGroundColorCodeForFocus);
            data.MenuForeColorCodeCodeForFocus = GetSystemParameter(sysParam, "HT_MenuForeColorCodeCodeForFocus", data.MenuForeColorCodeCodeForFocus);
            // HTブザー、バイブレーション関連
            data.HT_DefaultBuzzerTone = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerTone", data.HT_DefaultBuzzerTone);
            data.HT_DefaultBuzzerOnPeriod = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerOnPeriod", data.HT_DefaultBuzzerOnPeriod);
            data.HT_DefaultBuzzerOffPeriod = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerOffPeriod", data.HT_DefaultBuzzerOffPeriod);
            data.HT_DefaultBuzzerReratCount = GetSystemParameterInt(sysParam, "HT_DefaultBuzzerReratCount", data.HT_DefaultBuzzerReratCount);
            data.HT_DefaultVibrationOnPeriod = GetSystemParameterInt(sysParam, "HT_DefaultVibrationOnPeriod", data.HT_DefaultVibrationOnPeriod);
            data.HT_DefaultVibrationOffPeriod = GetSystemParameterInt(sysParam, "HT_DefaultVibrationOffPeriod", data.HT_DefaultVibrationOffPeriod);
            data.HT_DefaultVibrationReratCount = GetSystemParameterInt(sysParam, "HT_DefaultVibrationReratCount", data.HT_DefaultVibrationReratCount);

            data.HT_ErrorBuzzerTone = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerTone", data.HT_ErrorBuzzerTone);
            data.HT_ErrorBuzzerOnPeriod = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerOnPeriod", data.HT_ErrorBuzzerOnPeriod);
            data.HT_ErrorBuzzerOffPeriod = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerOffPeriod", data.HT_ErrorBuzzerOffPeriod);
            data.HT_ErrorBuzzerReratCount = GetSystemParameterInt(sysParam, "HT_ErrorBuzzerReratCount", data.HT_ErrorBuzzerReratCount);
            data.HT_ErrorVibrationOnPeriod = GetSystemParameterInt(sysParam, "HT_ErrorVibrationOnPeriod", data.HT_ErrorVibrationOnPeriod);
            data.HT_ErrorVibrationOffPeriod = GetSystemParameterInt(sysParam, "HT_ErrorVibrationOffPeriod", data.HT_ErrorVibrationOffPeriod);
            data.HT_ErrorVibrationReratCount = GetSystemParameterInt(sysParam, "HT_ErrorVibrationReratCount", data.HT_ErrorVibrationReratCount);

            // データグリッド関係
            data.DataGridHeaderFontSizeHT = GetSystemParameter(sysParam, "DataGridHeaderFontSizeHT", data.DataGridHeaderFontSizeHT);
            data.DataGridHeaderFontSizePC = GetSystemParameter(sysParam, "DataGridHeaderFontSizePC", data.DataGridHeaderFontSizePC);
            data.DataGridColumnFontSizeHT = GetSystemParameter(sysParam, "DataGridColumnFontSizeHT", data.DataGridColumnFontSizeHT);
            data.DataGridColumnFontSizePC = GetSystemParameter(sysParam, "DataGridColumnFontSizePC", data.DataGridColumnFontSizePC);
            data.DataGridCellFontSizeHT = GetSystemParameter(sysParam, "DataGridCellFontSizeHT", data.DataGridCellFontSizeHT);
            data.DataGridCellFontSizePC = GetSystemParameter(sysParam, "DataGridCellFontSizePC", data.DataGridCellFontSizePC);
            data.DataGridCellFontWeightHT = GetSystemParameter(sysParam, "DataGridCellFontWeightHT", data.DataGridCellFontWeightHT);
            data.DataGridCellFontWeightPC = GetSystemParameter(sysParam, "DataGridCellFontWeightPC", data.DataGridCellFontWeightPC);
            data.DataGridFooterFontSizeHT = GetSystemParameter(sysParam, "DataGridFooterFontSizeHT", data.DataGridFooterFontSizeHT);
            data.DataGridFooterFontSizePC = GetSystemParameter(sysParam, "DataGridFooterFontSizePC", data.DataGridFooterFontSizePC);
            data.DataGridHeightHT = GetSystemParameter(sysParam, "DataGridHeightHT", data.DataGridHeightHT);
            data.DataGridHeightPC = GetSystemParameter(sysParam, "DataGridHeightPC", data.DataGridHeightPC);

            //バリデーション関連
            data.PC_ValidationSummaryFontSize = GetSystemParameter(sysParam, "PC_ValidationSummaryFontSize", data.PC_ValidationSummaryFontSize);
            data.HT_ValidationSummaryFontSize = GetSystemParameter(sysParam, "HT_ValidationSummaryFontSize", data.HT_ValidationSummaryFontSize);
            data.PC_ValidationSummaryFontWeight = GetSystemParameter(sysParam, "PC_ValidationSummaryFontWeight", data.PC_ValidationSummaryFontWeight);
            data.HT_ValidationSummaryFontWeight = GetSystemParameter(sysParam, "HT_ValidationSummaryFontWeight", data.HT_ValidationSummaryFontWeight);

            // PCコンポーネント関連
            data.PC_GroupFieldTitleFontSize = GetSystemParameter(sysParam, "PC_GroupFieldTitleFontSize", data.PC_GroupFieldTitleFontSize);
            data.PC_GroupFieldTitleFontWeight = GetSystemParameter(sysParam, "PC_GroupFieldTitleFontWeight", data.PC_GroupFieldTitleFontWeight);
            data.PC_GroupFieldLabelFontSize = GetSystemParameter(sysParam, "PC_GroupFieldLabelFontSize", data.PC_GroupFieldLabelFontSize);
            data.PC_GroupFieldLabelFontWeight = GetSystemParameter(sysParam, "PC_GroupFieldLabelFontWeight", data.PC_GroupFieldLabelFontWeight);
            data.PC_GroupFieldLabelWidth = GetSystemParameter(sysParam, "PC_GroupFieldLabelWidth", data.PC_GroupFieldLabelWidth);
            data.PC_TextBoxFontSize = GetSystemParameter(sysParam, "PC_TextBoxFontSize", data.PC_TextBoxFontSize);
            data.PC_TextBoxFontWeight = GetSystemParameter(sysParam, "PC_TextBoxFontWeight", data.PC_TextBoxFontWeight);
            data.PC_TextBoxWidth = GetSystemParameter(sysParam, "PC_TextBoxWidth", data.PC_TextBoxWidth);
            data.PC_TextBoxHeight = GetSystemParameter(sysParam, "PC_TextBoxHeight", data.PC_TextBoxHeight);
            data.PC_TextAreaFontSize = GetSystemParameter(sysParam, "PC_TextAreaFontSize", data.PC_TextAreaFontSize);
            data.PC_TextAreaFontWeight = GetSystemParameter(sysParam, "PC_TextAreaFontWeight", data.PC_TextAreaFontWeight);
            data.PC_DatePickerFontSize = GetSystemParameter(sysParam, "PC_DatePickerFontSize", data.PC_DatePickerFontSize);
            data.PC_DatePickerFontWeight = GetSystemParameter(sysParam, "PC_DatePickerFontWeight", data.PC_DatePickerFontWeight);
            data.PC_DatePickerWidth = GetSystemParameter(sysParam, "PC_DatePickerWidth", data.PC_DatePickerWidth);
            data.PC_DatePickerHeight = GetSystemParameter(sysParam, "PC_DatePickerHeight", data.PC_DatePickerHeight);
            data.PC_TimePickerFontSize = GetSystemParameter(sysParam, "PC_TimePickerFontSize", data.PC_TimePickerFontSize);
            data.PC_TimePickerFontWeight = GetSystemParameter(sysParam, "PC_TimePickerFontWeight", data.PC_TimePickerFontWeight);
            data.PC_TimePickerWidth = GetSystemParameter(sysParam, "PC_TimePickerWidth", data.PC_TimePickerWidth);
            data.PC_TimePickerHeight = GetSystemParameter(sysParam, "PC_TimePickerHeight", data.PC_TimePickerHeight);
            data.PC_DateTimePickerFontSize = GetSystemParameter(sysParam, "PC_DateTimePickerFontSize", data.PC_DateTimePickerFontSize);
            data.PC_DateTimePickerFontWeight = GetSystemParameter(sysParam, "PC_DateTimePickerFontWeight", data.PC_DateTimePickerFontWeight);
            data.PC_DateTimePickerWidth = GetSystemParameter(sysParam, "PC_DateTimePickerWidth", data.PC_DateTimePickerWidth);
            data.PC_DateTimePickerHeight = GetSystemParameter(sysParam, "PC_DateTimePickerHeight", data.PC_DateTimePickerHeight);
            data.PC_DropDownFontSize = GetSystemParameter(sysParam, "PC_DropDownFontSize", data.PC_DropDownFontSize);
            data.PC_DropDownFontWeight = GetSystemParameter(sysParam, "PC_DropDownFontWeight", data.PC_DropDownFontWeight);
            data.PC_DropDownWidth = GetSystemParameter(sysParam, "PC_DropDownWidth", data.PC_DropDownWidth);
            data.PC_DropDownHeight = GetSystemParameter(sysParam, "PC_DropDownHeight", data.PC_DropDownHeight);
            data.PC_CheckBoxFontSize = GetSystemParameter(sysParam, "PC_CheckBoxFontSize", data.PC_CheckBoxFontSize);
            data.PC_CheckBoxFontWeight = GetSystemParameter(sysParam, "PC_CheckBoxFontWeight", data.PC_CheckBoxFontWeight);
            data.PC_CheckBoxTitleFontSize = GetSystemParameter(sysParam, "PC_CheckBoxTitleFontSize", data.PC_CheckBoxTitleFontSize);
            data.PC_CheckBoxTitleFontWeight = GetSystemParameter(sysParam, "PC_CheckBoxTitleFontWeight", data.PC_CheckBoxTitleFontWeight);
            data.PC_RadioButtonFontSize = GetSystemParameter(sysParam, "PC_RadioButtonFontSize", data.PC_RadioButtonFontSize);
            data.PC_RadioButtonFontWeight = GetSystemParameter(sysParam, "PC_RadioButtonFontWeight", data.PC_RadioButtonFontWeight);
            data.PC_RadioButtonTitleFontSize = GetSystemParameter(sysParam, "PC_RadioButtonTitleFontSize", data.PC_RadioButtonTitleFontSize);
            data.PC_RadioButtonTitleFontWeight = GetSystemParameter(sysParam, "PC_RadioButtonTitleFontWeight", data.PC_RadioButtonTitleFontWeight);
            data.PC_NumericFontSize = GetSystemParameter(sysParam, "PC_NumericFontSize", data.PC_NumericFontSize);
            data.PC_NumericFontWeight = GetSystemParameter(sysParam, "PC_NumericFontWeight", data.PC_NumericFontWeight);
            data.PC_NumericWidth = GetSystemParameter(sysParam, "PC_NumericWidth", data.PC_NumericWidth);
            data.PC_NumericHeight = GetSystemParameter(sysParam, "PC_NumericHeight", data.PC_NumericHeight);
            data.PC_DropDownDataGridFontSize = GetSystemParameter(sysParam, "PC_DropDownDataGridFontSize", data.PC_DropDownDataGridFontSize);
            data.PC_DropDownDataGridFontWeight = GetSystemParameter(sysParam, "PC_DropDownDataGridFontWeight", data.PC_DropDownDataGridFontWeight);
            data.PC_TabsExtendFontSize = GetSystemParameter(sysParam, "PC_TabsExtendFontSize", data.PC_TabsExtendFontSize);
            data.PC_TabsExtendFontWeight = GetSystemParameter(sysParam, "PC_TabsExtendFontWeight", data.PC_TabsExtendFontWeight);
            data.PC_DateInitWmsAddDays = GetSystemParameterInt(sysParam, "PC_DateInitWmsAddDays", data.PC_DateInitWmsAddDays);

            // 必須表示の付加文字
            data.RequiredDisplaySuffixHT = GetSystemParameter(sysParam, "RequiredDisplaySuffixHT", data.RequiredDisplaySuffixHT);
            data.RequiredDisplaySuffixPC = GetSystemParameter(sysParam, "RequiredDisplaySuffixPC", data.RequiredDisplaySuffixPC);

            // 出荷開始警告時間
            data.ShipmentsStartWarnTime = GetSystemParameterInt(sysParam, "ShipmentsStartWarnTime", data.ShipmentsStartWarnTime);
            // 出荷締切警告時間
            data.ShipmentsDeadlineWarnTime = GetSystemParameterInt(sysParam, "ShipmentsDeadlineWarnTime", data.ShipmentsDeadlineWarnTime);
            //Notify表示時間
            data.NotifyPopupDuration = GetSystemParameterInt(sysParam, "NotifyPopupDuration", data.NotifyPopupDuration);

            //2023/12/27 追加

            //PC,HT画面のボタンのmargin-bottomの高さ
            data.PC_ButtonMarginBottom = GetSystemParameter(sysParam, "PC_ButtonMarginBottom", data.PC_ButtonMarginBottom);
            data.HT_ButtonMarginBottom = GetSystemParameter(sysParam, "HT_ButtonMarginBottom", data.HT_ButtonMarginBottom);
            //HT画面のファンクションボタンの背景色の色
            data.HT_Button1BackgroundColor = GetSystemParameter(sysParam, "HT_Button1BackgroundColor", data.HT_Button1BackgroundColor);
            data.HT_Button2BackgroundColor = GetSystemParameter(sysParam, "HT_Button2BackgroundColor", data.HT_Button2BackgroundColor);
            data.HT_Button3BackgroundColor = GetSystemParameter(sysParam, "HT_Button3BackgroundColor", data.HT_Button3BackgroundColor);
            data.HT_Button4BackgroundColor = GetSystemParameter(sysParam, "HT_Button4BackgroundColor", data.HT_Button4BackgroundColor);
            //HT画面のファンクションボタンの文字の色
            data.HT_Button1TextColor = GetSystemParameter(sysParam, "HT_Button1TextColor", data.HT_Button1TextColor);
            data.HT_Button4TextColor = GetSystemParameter(sysParam, "HT_Button4TextColor", data.HT_Button4TextColor);

            //PC　DropDownDataGridのチェックボックスの幅
            data.PC_DropDownDataGridCheckBoxWidth = GetSystemParameter(sysParam, "PC_DropDownDataGridCheckBoxWidth", data.PC_DropDownDataGridCheckBoxWidth);

            //PC 必須表示の付加文字の色
            data.RequiredDisplaySuffixColorPC = GetSystemParameter(sysParam, "RequiredDisplaySuffixColorPC", data.RequiredDisplaySuffixColorPC);

            //HTメニューボタンの幅の大きさ
            data.HT_MenuBottonWidth = GetSystemParameter(sysParam, "HT_MenuBottonWidth", data.HT_MenuBottonWidth);

            //コンポーネントのテキストエリアの行数,列数
            data.CompTextAreaRows = GetSystemParameterInt(sysParam, "CompTextAreaRows", data.CompTextAreaRows);
            data.CompTextAreaCols = GetSystemParameterInt(sysParam, "CompTextAreaCols", data.CompTextAreaCols);

            //ログインフォームのタイトルと入力欄のColumn値
            data.LoginFormTitleColumnSize = GetSystemParameterInt(sysParam, "LoginFormTitleColumnSize", data.LoginFormTitleColumnSize);
            data.LoginFormTextBoxColumnSize = GetSystemParameterInt(sysParam, "LoginFormTextBoxColumnSize", data.LoginFormTextBoxColumnSize);

            //HT　データカードのmarginの値
            data.HT_DataCardContentMarginTop = GetSystemParameter(sysParam, "HT_DataCardContentMarginTop", data.HT_DataCardContentMarginTop);
            data.HT_DataCardContentMarginLeft = GetSystemParameter(sysParam, "HT_DataCardContentMarginLeft", data.HT_DataCardContentMarginLeft);
            data.HT_DataCardContentMarginRight = GetSystemParameter(sysParam, "HT_DataCardContentMarginRight", data.HT_DataCardContentMarginRight);
            data.HT_DataCardContentMarginBottom = GetSystemParameter(sysParam, "HT_DataCardContentMarginBottom", data.HT_DataCardContentMarginBottom);
            //HT　データカード内のStackのmarginの値
            data.HT_InsideDataCardContentMarginTop = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginTop", data.HT_InsideDataCardContentMarginTop);
            data.HT_InsideDataCardContentMarginLeft = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginLeft", data.HT_InsideDataCardContentMarginLeft);
            data.HT_InsideDataCardContentMarginRight = GetSystemParameter(sysParam, "HT_InsideDataCardContentMarginRight", data.HT_InsideDataCardContentMarginRight);
            //HT　データカードリストの高さ
            data.HT_DataCardListHeight = GetSystemParameter(sysParam, "HT_DataCardListHeight", data.HT_DataCardListHeight);

            //HT　データグリッド内のバラの色
            data.HT_DataGridBaraColor = GetSystemParameter(sysParam, "HT_DataGridBaraColor", data.HT_DataGridBaraColor);
            //HT ロケーションコンボボックスの最大値件数。
            data.HT_LocComBoxMaxCount = GetSystemParameterInt(sysParam, "HT_LocComBoxMaxCount", data.HT_LocComBoxMaxCount);

            //PC　進捗管理の凡例のマーク,テキストの幅
            data.PC_DataGridProgressMarkWidth = GetSystemParameterInt(sysParam, "PC_DataGridProgressMarkWidth", data.PC_DataGridProgressMarkWidth);
            data.PC_DataGridProgressTextWidth = GetSystemParameterInt(sysParam, "PC_DataGridProgressTextWidth", data.PC_DataGridProgressTextWidth);

            //メッセージダイアログの幅,高さ（処理中、読込中）
            data.MessageDialogWidth = GetSystemParameterInt(sysParam, "MessageDialogWidth", data.MessageDialogWidth);
            data.MessageDialogHeight = GetSystemParameterInt(sysParam, "MessageDialogHeight", data.MessageDialogHeight);
            //確認メッセージダイアログの幅（OKのみ）
            data.DialogShowOKWidth = GetSystemParameterInt(sysParam, "DialogShowOKWidth", data.DialogShowOKWidth);
            data.DialogShowOKHeight = GetSystemParameterInt(sysParam, "DialogShowOKHeight", data.DialogShowOKHeight);
            //確認メッセージダイアログの幅（YesとNo）
            data.DialogShowYesNoWidth = GetSystemParameterInt(sysParam, "DialogShowYesNoWidth", data.DialogShowYesNoWidth);
            data.DialogShowYesNoHeight = GetSystemParameterInt(sysParam, "DialogShowYesNoHeight", data.DialogShowYesNoHeight);

            //編集ダイアログの備考の最大入力数,行,列数
            data.DialogContentMaxlength = GetSystemParameterInt(sysParam, "DialogContentMaxlength", data.DialogContentMaxlength);
            data.DialogContentCols = GetSystemParameterInt(sysParam, "DialogContentCols", data.DialogContentCols);
            data.DialogContentRows = GetSystemParameterInt(sysParam, "DialogContentRows", data.DialogContentRows);

            //編集ダイアログの概要の文字数
            data.DialogContentRemarksRows = GetSystemParameterInt(sysParam, "DialogContentRemarksRows", data.DialogContentRemarksRows);

            //データグリッドのページ数
            data.DataGridPageSize = GetSystemParameterInt(sysParam, "DataGridPageSize", data.DataGridPageSize);

            MessageDialogWidth = data.MessageDialogWidth;
            MessageDialogHeight = data.MessageDialogHeight;

            DialogShowOKWidth = data.DialogShowOKWidth;
            DialogShowOKHeight = data.DialogShowOKHeight;

            DialogShowYesNoWidth = data.DialogShowYesNoWidth;
            DialogShowYesNoHeight = data.DialogShowYesNoHeight;


            return data;
        }

        private string GetSystemParameter(List<SystemParameters> sysParam, string key, string def)
        {
            string val = def;
            SystemParameters? param = sysParam.FirstOrDefault(_ => _.ParameterKey == key);
            if (null != param)
            {
                val = param.ParameterValue;
            }
            return val;
        }
        private int GetSystemParameterInt(List<SystemParameters> sysParam, string key, int def)
        {
            int val = def;
            SystemParameters? param = sysParam.FirstOrDefault(_ => _.ParameterKey == key);
            if (null != param)
            {
                val = GetValueInt(param.ParameterValue);
            }
            return val;
        }

        /// <summary>
        /// ログイン情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<LoginInfo[]> GetLoginInfoAsync()
        {
            // 取得カラム名を指定する（共通）
            ClassNameSelect select = new()
            {
                viewName = "VW_ログイン情報"
            };

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<LoginInfo> data = new();

            if (null != resItems)
            {
                foreach (ResponseValue item in resItems)
                {
                    LoginInfo newRow = new()
                    {
                        Id = GetValueString(item, "USER_ID"),
                        UserName = GetValueString(item, "USER_NAME"),
                        Password = GetValueString(item, "PASSWORD"),
                        AuthorityLevel = GetValueInt(item, "AUTHORITY_LEVEL"),
                        AffiliationId = GetValueInt(item, "AFFILIATION_ID"),
                        AffiliationName = GetValueString(item, "AFFILIATION_NAME"),
                        AllFeatureEnable = GetValueBool(item, "ALL_FEATURE_ENABLE"),
                    };
                    data.Add(newRow);
                }
            }
            return data.ToArray();
        }



        /// <summary>
        /// クラス名からDEFINE_COMPNENTSテーブル情報を取得する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<List<ComponentsInfo>> GetComponetnsInfo(string className)
        {
            await Task.Delay(0);//警告の抑制
            Dictionary<string, List<ComponentsInfo>> info = ComponentsInfoAll;
            List<ComponentsInfo> data = new();
            if (null != info && info.ContainsKey(className))
            {
                data = info[className];
            }
            return data;
        }

        /// <summary>
        /// クラス名からDEFINE_COMPNENT_COLUMNSテーブル情報を取得する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<List<ComponentColumnsInfo>> GetGridColumnsData(string className)
        {
            await Task.Delay(0);//警告の抑制
            Dictionary<string, List<ComponentColumnsInfo>> info = ComponentColumnsAll;
            List<ComponentColumnsInfo> data = new();
            if (null != info && info.ContainsKey(className))
            {
                data = info[className];
            }

            return data;
        }

        /// <summary>
        /// クラス名からDEFINE_COMPONENT_PROGRAMSの情報を取得する
        /// </summary>
        /// <param name="columnInfos"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public async Task<List<ComponentProgramInfo>> GetComponentProgramInfo(string className)
        {
            await Task.Delay(0);//警告の抑制
            Dictionary<string, List<ComponentProgramInfo>> info = ComponentProgramAll;
            List<ComponentProgramInfo> data = new();
            if (null != info && info.ContainsKey(className))
            {
                data = info[className];
            }

            return data;
        }

        /// <summary>
        /// 倉庫マスタ情報を取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstAreaData>> GetMstAreaInfoAll()
        {
            await Task.Delay(0);//警告の抑制
            return MstAreaInfoAll;
        }

        /// <summary>
        /// エリアマスタ情報を取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstZoneData>> GetMstZoneInfoAll()
        {
            await Task.Delay(0);//警告の抑制
            return MstZoneInfoAll;
        }

        /// <summary>
        /// ロケーションマスタ情報を取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MstLocationData>> GetMstLocationInfoAll()
        {
            await Task.Delay(0);//警告の抑制
            return MstLocationInfoAll;
        }

        

        /// <summary>
        /// クラス名からMST_USER_COMPONENT_SETTINGSテーブル情報を取得する
        /// </summary>
        /// <param name="className"></param>
        /// <param name="componentName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetUserComponentSettingsInfo(string className, string componentName = "", string viewName = "")
        {
            // ログインユーザ情報取得
            LoginInfo login = await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO)
                ? await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO)
                : new LoginInfo();

            // 取得カラム名を指定する（共通）
            ClassNameSelect select = new()
            {
                viewName = "MST_USER_COMPONENT_SETTINGS"
            };
            select.whereParam.Add("USER_ID", new WhereParam { val = login.Id, whereType = enumWhereType.Equal });
            select.whereParam.Add("CLASS_NAME", new WhereParam { val = className, whereType = enumWhereType.Equal });
            if (!string.IsNullOrEmpty(componentName))
            {
                select.whereParam.Add("COMPONENT_NAME", new WhereParam { val = componentName, whereType = enumWhereType.Equal });
            }
            if (!string.IsNullOrEmpty(viewName))
            {
                select.whereParam.Add("VIEW_NAME", new WhereParam { val = viewName, whereType = enumWhereType.Equal });
            }
            select.orderByParam.Add(new OrderByParam { field = "COMPONENT_NAME" });
            select.orderByParam.Add(new OrderByParam { field = "VIEW_NAME" });
            select.orderByParam.Add(new OrderByParam { field = "PROPERTY_KEY" });
            select.orderByParam.Add(new OrderByParam { field = "VALUE_KEY_ID" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<UserComponentSettingsInfo> lstInfo = new();
            Dictionary<string, object> data = new();

            if (null != resItems)
            {
                string strPropertyKey = string.Empty;
                foreach (ResponseValue item in resItems)
                {
                    UserComponentSettingsInfo newRow = new()
                    {
                        ComponentName = GetValueString(item, "COMPONENT_NAME"),
                        ViewName = GetValueString(item, "VIEW_NAME"),
                        PropertyKey = GetValueString(item, "PROPERTY_KEY"),
                        ValueKeyId = GetValueInt(item, "VALUE_KEY_ID"),
                        Value = GetValueString(item, "VALUE"),
                        ValueDataType = GetValueString(item, "VALUE_DATA_TYPE"),
                    };
                    lstInfo.Add(newRow);
                }

                var infos = lstInfo.GroupBy(_ => new
                {
                    _.ComponentName,
                    _.ViewName,
                    _.PropertyKey,
                    _.ValueDataType
                })
                    .OrderBy(_ => _.Key.ComponentName)
                    .ThenBy(_ => _.Key.ViewName)
                    .ThenBy(_ => _.Key.PropertyKey);
                foreach (var items in infos)
                {

                    Type type = Type.GetType(items.Key.ValueDataType);
                    if (type != null)
                    {
                        if (typeof(CompCheckBoxList) == type || typeof(CompDateFromTo) == type || typeof(CompTimeFromTo) == type || typeof(CompDateTimeFromTo) == type || typeof(CompSearchNumeric) == type || typeof(CompDropDownDataGrid) == type)
                        {
                            List<string> lstValue = new();
                            foreach (UserComponentSettingsInfo? item in items)
                            {
                                lstValue.Add(item.Value);
                            }
                            data[items.Key.PropertyKey] = lstValue;
                        }
                        else
                        {
                            foreach (UserComponentSettingsInfo? item in items)
                            {
                                data[item.PropertyKey] = item.Value;
                                break;
                            }
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// ClassNameSelectクラスからテーブルまたはVIEWデータを取得する
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public async Task<List<IDictionary<string, object>>> GetSelectData(ClassNameSelect select)
        {
            ResponseValue[]? resItems = await GetResponseValue(select);
            List<IDictionary<string, object>> data = new();

            if (null != resItems)
            {
                foreach (ResponseValue item in resItems)
                {
                    Dictionary<string, object> newRow = new();
                    for (int i = 0; item.Columns.Count > i; i++)
                    {
                        newRow.Add(item.Columns[i], GetValueString(item, item.Columns[i]));
                    }
                    data.Add(newRow);
                }
            }
            return data;
        }

        /// <summary>
        /// ClassNameSelectクラスからテーブルまたはVIEWデータを取得する
        /// ただし、グリッド列のPropertyに登録されているデータのみ
        /// </summary>
        /// <param name="columnInfos"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public async Task<List<IDictionary<string, object>>> GetSelectGridData(IList<ComponentColumnsInfo> columnInfos, ClassNameSelect select)
        {
            ResponseValue[]? resItems = await GetResponseValue(select);//TODO タイムアウトの設定
            List<IDictionary<string, object>> data = new();

            if (null != resItems)
            {
                foreach (ResponseValue item in resItems)
                {
                    Dictionary<string, object> newRow = new();
                    // グリッド列のプロパティ名と一致するデータのみをDictionaryに追加する
                    // グリッドにデータをバインドする際にエラーになる
                    for (int i = 0; columnInfos.Count > i; i++)
                    {
                        // columnInfosのPropertyの改行はcolumnInfosを取得する時に置換している
                        // View情報の列名と一致させるために"\\n"に変換してデータ取得を行う
                        // キーは改行をコードに置換した値とする
                        string property = columnInfos[i].Property.Replace("\n", "\\n");
                        string value = GetValueString(item, property).Replace("\\n", '\n'.ToString());
                        newRow.Add(columnInfos[i].Property, value);
                    }
                    data.Add(newRow);
                }
            }
            return data;
        }

        /// <summary>
        /// VIEW名称を指定して、ValueTextInfoを取得する
        /// </summary>
        /// <param name="vName"></param>
        /// <returns></returns>
        public async Task<List<ValueTextInfo>> GetValueTextInfo(string vName)
        {
            // 取得カラム名を指定する（共通）
            ClassNameSelect select = new()
            {
                viewName = vName
            };
            select.orderByParam.Add(new OrderByParam { field = "SORT_ORDER" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            List<ValueTextInfo> lstInfo = new();

            if (null != resItems)
            {
                HashSet<string> columnsToExclude = new() { "VALUE_MEMBER", "TEXT_MEMBER", "SORT_ORDER", "BASE_ID", "BASE_TYPE", "CONSIGNOR_ID" };
                foreach (ResponseValue item in resItems)
                {
                    ValueTextInfo info = new()
                    {
                        Value = GetValueString(item, "VALUE_MEMBER"),
                        Text = GetValueString(item, "TEXT_MEMBER"),
                    };
                    List<string> columns = item.Columns.Except(columnsToExclude).ToList();
                    for (int i = 0; columns.Count > i; i++)
                    {
                        //パフォーマンス改善優先のため、ここはリフレクションは使用せずべた書きとした。//TODO もっと良い書き方があればそっちを採用する。
                        switch (i)
                        {
                            case 0: info.Value1 = item.Values[columns[i]]?.ToString()!; break;
                            case 1: info.Value2 = item.Values[columns[i]]?.ToString()!; break;
                            case 2: info.Value3 = item.Values[columns[i]]?.ToString()!; break;
                            case 3: info.Value4 = item.Values[columns[i]]?.ToString()!; break;
                            case 4: info.Value5 = item.Values[columns[i]]?.ToString()!; break;
                            case 5: info.Value6 = item.Values[columns[i]]?.ToString()!; break;
                            case 6: info.Value7 = item.Values[columns[i]]?.ToString()!; break;
                            case 7: info.Value8 = item.Values[columns[i]]?.ToString()!; break;
                            case 8: info.Value9 = item.Values[columns[i]]?.ToString()!; break;
                            case 9: info.Value10 = item.Values[columns[i]]?.ToString()!; break;
                        }
                        //リフレクション使用の元コード
                        //PropertyInfo? p = typeof(ValueTextInfo)?.GetProperty($"Value{i + 1}", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        //p?.SetValue(info, item.Values[columns[i]]?.ToString());
                    }
                    lstInfo.Add(info);
                }
            }
            return lstInfo;
        }

        /// <summary>
        /// ViewNameを取得する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<string> GetViewNameAsync(string className)
        {
            string? viewName = string.Empty;
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "DEFINE_PAGE_VALUES"
                    ,
                    tsqlHints = EnumTSQLhints.NOLOCK
                };
                select.selectParam.Add("VIEW_NAME");
                select.whereParam.Add("CLASS_NAME", new WhereParam { val = className, whereType = enumWhereType.Equal });

                ResponseValue[]? resValues = await GetResponseValue(select);
                if (null != resValues && resValues.Count() > 0)
                {
                    if (resValues[0].Values.TryGetValue("VIEW_NAME", out object? obj))
                    {
                        viewName = obj?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return viewName;
        }

        /// <summary>
        /// ローカルストレージからキーをもとに値を取得する
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public async Task<string> GetLocalStorage(string strKey)
        {
            string param = string.Empty;
            try
            {
                param = await _localStorage.GetItemAsync<string>(strKey.ToString());
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            // 取得したキーはクリアする
            await _localStorage.RemoveItemAsync(strKey);

            return param;
        }

        /// <summary>
        /// ローカルストレージにキーバリューをセットする
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetLocalStorage(string strKey, object value)
        {
            await _localStorage.SetItemAsync(strKey, value?.ToString());
        }
        /// <summary>
        /// すべてのローカルストレージの値をクリアする
        /// </summary>
        /// <returns></returns>
        public async Task ClearAllLocalStorageValue()
        {
            await _localStorage.ClearAsync();
        }

        #region メニュー関連

        /// <summary>
        /// メニュー情報を全て取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<MenuInfo>> GetMenuInfoAllAsync(int login_auth_level)
        {
            List<MenuInfo> lstMenuInfo = new();
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "VW_メニューマスタ"
                    ,
                    tsqlHints = EnumTSQLhints.NOLOCK
                };
                //権限レベルを絞る
                select.whereParam.Add("AUTHORITY_LOWER", new WhereParam { val = login_auth_level.ToString(), whereType = enumWhereType.Below });//最低権限レベル以下で絞る
                ResponseValue[]? resValues = await GetResponseValue(select);
                if (resValues is null)
                {
                    throw new NullReferenceException("GetMenuInfoAllAsync_resValues is null");
                }
                foreach (ResponseValue resValue in resValues)
                {
                    SetMenuInfo(resValue, out MenuInfo menuInfo);
                    if (null != menuInfo)
                    {
                        lstMenuInfo.Add(menuInfo);
                    }
                }

            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return lstMenuInfo;
        }

        /// <summary>
        /// Navメニュー情報一覧取得
        /// </summary>
        /// <param name="isHandy"></param>
        /// <param name="bnGetAll"></param>
        /// <returns></returns>
        public async Task<List<MenuInfo>> GetMenuInfoListNavAsync(bool isHandy, bool bnGetAll = false)
        {
            List<MenuInfo> lstMenuInfo = new();
            try
            {
                List<MenuInfo> lstPc = new();
                List<MenuInfo> lstHt = new();

                if (isHandy)
                {
                    // HTメニュー情報取得・追加
                    // ※HTメニューはトップから２階層目と３階層目を取得
                    MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
                    if (infoHtTop != null)
                    {
                        lstHt = await GetMenuInfo((int)TYPE_DEVICE_GROUP_ID.HT, infoHtTop.menuId);
                        lstMenuInfo.AddRange(lstHt);
                    }
                }
                else
                {
                    if (bnGetAll)
                    {
                        // PCメニュー情報取得・追加
                        lstPc = await GetMenuInfo((int)TYPE_DEVICE_GROUP_ID.PC, string.Empty);
                        lstMenuInfo.AddRange(lstPc);

                        // HTメニュー情報取得・追加
                        // ※HTメニューはトップから２階層目と３階層目を取得
                        MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
                        if (infoHtTop != null)
                        {
                            lstHt = await GetMenuInfo(infoHtTop.DispDivision, infoHtTop.menuId);
                            lstMenuInfo.AddRange(lstHt);
                        }
                    }
                    else
                    {
                        // PCメニュー情報取得・追加
                        lstPc = await GetMenuInfo((int)TYPE_DEVICE_GROUP_ID.PC, string.Empty);
                        lstMenuInfo.AddRange(lstPc);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return lstMenuInfo;
        }

        /// <summary>
        /// HTトップメニュー情報一覧を取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<MenuInfo>> GetMenuInfoListHtTopAsync()
        {
            List<MenuInfo> lstMenuInfo = new();
            try
            {
                // HTメニュー情報取得・追加
                // ※HTメニューはトップから２階層目と３階層目を取得
                MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
                if (infoHtTop != null)
                {
                    lstMenuInfo = await GetMenuInfo((int)TYPE_DEVICE_GROUP_ID.HT, infoHtTop.menuId);
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return lstMenuInfo;
        }

        /// <summary>
        /// HTメニューのトップ情報のみ取得する
        /// </summary>
        /// <returns></returns>
        public async Task<MenuInfo> GetMenuInfoHtTopAsync()
        {
            MenuInfo menuInfo = null!;
            try
            {
                // ストレージからメニュー情報を取得
                List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

                // HTメニューのトップ情報のみ取得する
                List<MenuInfo> lst = menuAll.Where(_ => _.DispDivision == (int)TYPE_DEVICE_GROUP_ID.HT && _.parentMenuId == string.Empty).OrderBy(_ => _.dispOrder).ToList();
                if (lst.Count() > 0)
                {
                    menuInfo = lst[0];
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return menuInfo;
        }

        /// <summary>
        /// 指定したtypeDeviceGroupId,menuIdのメニュー情報一覧を返す
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        private async Task<List<MenuInfo>> GetMenuInfo(int groupId, string menuId)
        {
            List<MenuInfo> lstMenuInfo = new();
            try
            {
                // ストレージからメニュー情報を取得
                List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

                // メニュー情報の取得・追加
                List<MenuInfo> lst = menuAll.Where(_ => _.DispDivision == groupId && _.parentMenuId == menuId).OrderBy(_ => _.dispOrder).ToList();
                lstMenuInfo.AddRange(lst);

                // サブメニュー情報の取得・追加
                foreach (MenuInfo mInfo in lstMenuInfo)
                {
                    List<MenuInfo> lstSub = menuAll.Where(_ => _.DispDivision == mInfo.DispDivision && _.parentMenuId == mInfo.menuId).OrderBy(_ => _.dispOrder).ToList();
                    mInfo.subMenuList.AddRange(lstSub);
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }

            return lstMenuInfo;
        }

        /// <summary>
        /// クラス名からメニュー情報一覧を取得
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<List<MenuInfo>> GetMenuInfoListAtClassAsync(string className)
        {
            List<MenuInfo> lstMenuInfo = new();
            try
            {
                if (string.IsNullOrEmpty(className))
                {
                    return lstMenuInfo;
                }

                // ストレージからメニュー情報を取得
                List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

                // メニュー情報の取得・追加
                List<MenuInfo> lst = menuAll.Where(_ => _.className == className).OrderBy(_ => _.dispOrder).ToList();
                lstMenuInfo.AddRange(lst);

                // サブメニュー情報の取得・追加
                foreach (MenuInfo mInfo in lstMenuInfo)
                {
                    List<MenuInfo> lstSub = menuAll.Where(_ => _.DispDivision == mInfo.DispDivision && _.parentMenuId == mInfo.menuId).OrderBy(_ => _.dispOrder).ToList();
                    mInfo.subMenuList.AddRange(lstSub);
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }

            return lstMenuInfo;
        }

        /// <summary>
        /// MenuInfoにDBから取得した値をセットする
        /// </summary>
        /// <param name="resValue"></param>
        /// <param name="menuInfo"></param>
        private void SetMenuInfo(ResponseValue resValue, out MenuInfo menuInfo)
        {
            menuInfo = null!;
            if (resValue != null && resValue.Values != null && resValue.Columns != null)
            {
                menuInfo = new MenuInfo
                {
                    menuId = GetValueString(resValue, "MENU_ID"),
                    menuName = GetValueString(resValue, "MENU_NAME_STRING_KEY"),
                    menuUrlString = GetValueString(resValue, "URL"),
                    iconName = GetValueString(resValue, "ICON_NAME"),
                    ToolTipMessage = GetValueString(resValue, "TOOL_TIP_MESSAGE"),
                    dispOrder = GetValueInt(resValue, "SORT_ORDER"),
                    className = GetValueString(resValue, "CLASS_NAME"),
                    parentMenuId = GetValueString(resValue, "PARENT_MENU_ID"),
                    DispDivision = GetValueInt(resValue, "DEVICE_GROUP_ID"),
                };
            }
        }

        #endregion

        #region ページタイトル

        /// <summary>
        /// ページタイトルを取得する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<string> GetPageTitleAsync(string className)
        {
            await Task.Delay(0);//警告の抑制
            string? pageName = string.Empty;
            try
            {
                Dictionary<string, string> info = PageNameAll;
                if (null != info && info.ContainsKey(className))
                {
                    pageName = info[className];
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return pageName;
        }

        #endregion

        #region データ変換

        /// <summary>
        /// DB取得データから文字列を取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetValueString(object? obj)
        {
            return (obj is not null) ? obj!.ToString()! : "";
        }
        public string GetValueString(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueString(obj);
        }
        public string GetValueString(ResponseValue resp, string column)
        {
            return GetValueString(resp.Values, column);
        }

        /// <summary>
        /// DB取得データから文字列を取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool GetValueBool(object? obj)
        {
            return (obj is not null) && ConvertBool(obj!.ToString()!);
        }
        public bool GetValueBool(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueBool(obj);
        }
        public bool GetValueBool(ResponseValue resp, string column)
        {
            return GetValueBool(resp.Values, column);
        }
        public bool ConvertBool(string value)
        {
            _ = bool.TryParse(value, out bool result);
            return result;
        }

        /// <summary>
        /// DB取得データからByteを取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public byte GetValueByte(object? obj)
        {
            return (obj is not null) ? ConvertByte(obj!.ToString()!) : (byte)0;
        }
        public byte GetValueByte(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueByte(obj);
        }
        public byte GetValueByte(ResponseValue resp, string column)
        {
            return GetValueByte(resp.Values, column);
        }
        public byte ConvertByte(string value)
        {
            _ = byte.TryParse(value, out byte result);
            return result;
        }

        /// <summary>
        /// DB取得データから数値を取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public int GetValueInt(object? obj)
        {
            return (obj is not null) ? ConvertInt(obj!.ToString()!) : 0;
        }
        public int GetValueInt(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueInt(obj);
        }
        public int GetValueInt(ResponseValue resp, string column)
        {
            return GetValueInt(resp.Values, column);
        }
        public int ConvertInt(string value)
        {
            _ = int.TryParse(value, out int result);
            return result;
        }

        /// <summary>
        /// DB取得データから数値を取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public decimal? GetValueDecimalNull(object? obj)
        {
            return (obj is not null && !string.IsNullOrEmpty(obj.ToString())) ? ConvertDecimal(obj!.ToString()!) : null;
        }
        public decimal? GetValueDecimalNull(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueDecimalNull(obj);
        }
        public decimal? GetValueDecimalNull(ResponseValue resp, string column)
        {
            return GetValueDecimalNull(resp.Values, column);
        }
        public decimal ConvertDecimal(string value)
        {
            _ = decimal.TryParse(value, out decimal result);
            return result;
        }

        /// <summary>
        /// DB取得データから数値を取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public Type GetValueType(object? obj)
        {
            return (obj is not null) ? ConvertType(obj!.ToString()!) : typeof(string);
        }
        public Type GetValueType(IDictionary<string, object> resp, string column)
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueType(obj);
        }
        public Type GetValueType(ResponseValue resp, string column)
        {
            return GetValueType(resp.Values, column);
        }
        public Type ConvertType(string value)
        {
            Type? result = null;
            try
            {
                result = Type.GetType(value);
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// DB取得データからEnumを取得
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public T GetValueEnum<T>(object? obj) where T : new()
        {
            T ret = new();
            if (obj is not null)
            {
                string strEnumStr = obj.ToString()!;
                string strEnumStrPos = strEnumStr[(strEnumStr.LastIndexOf('.') + 1)..];
                ret = GetEnumValue<T>(strEnumStrPos);
            }
            return ret;
        }
        public T GetValueEnum<T>(IDictionary<string, object> resp, string column) where T : new()
        {
            _ = resp.TryGetValue(column, out object? obj);
            return GetValueEnum<T>(obj);
        }
        public T GetValueEnum<T>(ResponseValue resp, string column) where T : new()
        {
            return GetValueEnum<T>(resp.Values, column);
        }

        public static T GetEnumValue<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        #endregion

        #region PC画面遷移用パラメータ

        private readonly Dictionary<string, object> _transParamPC = new();

        /// <summary>
        /// PC画面遷移用パラメータ取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? GetPCTransParam(string key)
        {
            try
            {
                object? ret = null;
                if (_transParamPC.ContainsKey(key))
                {
                    ret = _transParamPC[key];
                }
                return ret;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// PC画面遷移用パラメータ設定
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPCTransParam(string key, object value)
        {
            try
            {
                _ = _transParamPC.Remove(key);
                _transParamPC[key] = value;
                return true;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// PC画面遷移用パラメータクリア
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearPCTransParam(string key)
        {
            try
            {
                _ = _transParamPC.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// PC画面遷移用パラメータ全件クリア
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearAllPCTransParam()
        {
            try
            {
                _transParamPC.Clear();
                return true;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return false;
            }
        }

        #endregion

        public Dictionary<string, (object, WhereParam)> GetCompItemInfoValues(List<List<CompItemInfo>> compItems)
        {
            Dictionary<string, (object, WhereParam)> result = new();

            foreach (List<CompItemInfo> listItem in compItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item.CompObj?.Instance is CompBase compBase)
                    {
                        compBase.AddWhereParam(result, item.TitleLabel);
                    }
                }
            }

            return result;
        }

        public void SetCompItemInputValues(List<List<CompItemInfo>> compItems, Dictionary<string, object> InitialData)
        {
            foreach (List<CompItemInfo> listItem in compItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    // 初期値取得
                    if (InitialData.TryGetValue(item.TitleLabel, out object? initVal))
                    {
                    }
                    if (item.CompObj?.Instance is CompBase compBase)
                    {
                        //TODO Setメソッドの中でnot null判定をしているが、そもそもこのメソッド中でnotnullのチェックしていればチェックする必要なのでは？またはコントロールによってnullをセットしたい場合があるか検討。
                        compBase.SetInitValue(initVal);
                    }
                }
            }
        }

        public async Task<List<List<CompItemInfo>>> GetCompItemInfo(
            List<ComponentColumnsInfo> listInfo
            , Dictionary<string, object> InitialData
            , IList<ComponentColumnsInfo> ComponentColumns
            , IList<ComponentsInfo> Components
            , bool bSearch = true
            , bool bEdit = false
            )
        {
            List<List<CompItemInfo>> result = new();
            int layoutGroup = -1;
            List<CompItemInfo> listItem = new();
            //<ViewName,data>
            Dictionary<string, Task<List<ValueTextInfo>>> d = new();
            foreach (ComponentColumnsInfo info in listInfo)
            {
                string vname = bSearch ? info.SearchValuesViewName : info.EditValuesViewName;
                if (!string.IsNullOrEmpty(vname))
                {
                    //WebAPIにリクエストを送るため、ループより先に非同期で値を取得しておく
                    d[vname] = GetValueTextInfo(vname);
                }
            }

            // WMS作業日、WMS作業加算取得
            DateTime? dtWms = null;
            DateTime? dtWmsAdd = null;
            if (listInfo.Count > 0)
            {
                dtWms = await GetWmsDate();
                dtWmsAdd = await GetWmsDateAdd();
            }

            foreach (ComponentColumnsInfo info in listInfo)
            {
                string typeKey;
                Type type;
                int dialogLayoutGroup;
                bool Required = false;
                bool DispRequired = false;
                string ViewName = string.Empty;
                string AttrName = string.Empty;

                if (bSearch)
                {
                    typeKey = info.SearchDataTypeKey;
                    type = Type.GetType(info.SearchDataTypeKey);
                    dialogLayoutGroup = info.SearchLayoutGroup;
                    Required = info.InputRequired;
                    ViewName = info.SearchValuesViewName;
                    AttrName = ChildPageBase.STR_ATTRIBUTE_SEARCH_DATE_INIT_MODE;
                }
                else
                {
                    typeKey = info.EditDataTypeKey;
                    type = Type.GetType(info.EditDataTypeKey);
                    dialogLayoutGroup = info.EditDialogLayoutGroup;
                    Required = info.EditInputRequired;
                    ViewName = info.EditValuesViewName;
                    AttrName = ChildPageBase.STR_ATTRIBUTE_EDIT_DATE_INIT_MODE;
                }
                if (type is null)
                {
                    continue;
                }
                if (dialogLayoutGroup != layoutGroup)
                {
                    layoutGroup = dialogLayoutGroup;
                    listItem = new List<CompItemInfo>();
                    result.Add(listItem);
                }

                // 検索条件取得用VIEWデータ取得
                List<ValueTextInfo>? data = null;
                if (!string.IsNullOrEmpty(ViewName))
                {
                    //ループ前で取得したDBの値をawaitで取り出す
                    data = await d[ViewName];
                }

                // 初期値取得
                //string initVal = string.Empty;
                //object? initVal = null;
                if (InitialData.TryGetValue(info.Property, out object? initVal))
                {
                    //initVal = obj;
                }

                // タイトルラベル
                string strTitle = info.Title;

                // タイトルラベル表示非表示
                bool bnDispTitle = true;

                // 日付コンポーネントの初期化モードをDEFINE_COMPONENTSから取得する
                enumDateInitMode initMode = enumDateInitMode.None;
                ComponentsInfo? componentsInfo = Components.FirstOrDefault(_ => _.ComponentName == AttrName && _.AttributesName == info.Property);
                if (componentsInfo != null)
                {
                    initMode = GetValueEnum<enumDateInitMode>(componentsInfo.Value);
                }

                // コンポーネントに渡すパラメータ生成
                Dictionary<string, object> param = new()
                    {
                        { "Title", info.Title },
                        { "Required", Required }
                    };
                if (bEdit)
                {
                    param.Add("Disabled", true);
                }

                // 必須Suffixの表示非表示
                if (Required && !bEdit)
                {
                    DispRequired = true;
                }

                if (Activator.CreateInstance(type) is CompBase compBase)
                {
                    //TODO CreateInstanceのパフォーマンスが悪い場合は下記コメント分に戻す。
                    compBase.AddParam(param, info, ComponentColumns, initVal, data, ref bnDispTitle, initMode, dtWms, dtWmsAdd, Components);
                }

                //if (typeof(CompTextBox) == type)
                //{
                //    //------------------------------
                //    // テキストボックス
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    param.Add("MaxLength", info.MaxLength!);
                //    if (info.ValueMin is not null)
                //    {
                //        param.Add("Min", info.ValueMin);
                //    }
                //    if (info.ValueMax is not null)
                //    {
                //        param.Add("Max", info.ValueMax);
                //    }
                //    if (info.RegularExpressionString is not null)
                //    {
                //        param.Add("RegexPattern", info.RegularExpressionString);
                //    }
                //}
                //else if (typeof(CompTextArea) == type)
                //{
                //    //------------------------------
                //    // テキストエリア
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    param.Add("MaxLength", info.MaxLength!);
                //}
                //else if (typeof(CompNumeric) == type)
                //{
                //    //------------------------------
                //    // 数値入力
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    if (info.ValueMin is not null)
                //    {
                //        param.Add("Min", info.ValueMin);
                //    }
                //    if (info.ValueMax is not null)
                //    {
                //        param.Add("Max", info.ValueMax);
                //    }
                //}
                //else if (typeof(CompSearchNumeric) == type)
                //{
                //    //------------------------------
                //    // 数値検索条件入力
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", ((List<string>)initVal)[0]);
                //        param.Add("DropInitialValue", ((List<string>)initVal)[1]);
                //    }
                //    if (info.ValueMin is not null)
                //    {
                //        param.Add("Min", info.ValueMin);
                //    }
                //    if (info.ValueMax is not null)
                //    {
                //        param.Add("Max", info.ValueMax);
                //    }
                //}
                //else if (typeof(CompDropDown) == type)
                //{
                //    //------------------------------
                //    // ドロップダウン
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    param.Add("Data", data!);
                //}
                //else if (typeof(CompCheckBoxList) == type)
                //{
                //    //------------------------------
                //    // チェックボックス
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValues", initVal);
                //    }
                //    param.Add("Data", data!);
                //    param.Add("FieldsetText", strTitle);

                //    bnDispTitle = false;
                //}
                //else if (typeof(CompRadioButtonList) == type)
                //{
                //    //------------------------------
                //    // ラジオボタン
                //    //------------------------------
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    param.Add("Data", data!);
                //    param.Add("FieldsetText", strTitle);

                //    bnDispTitle = false;
                //}
                //else if (typeof(CompDatePicker) == type)
                //{
                //    //------------------------------
                //    // 日付
                //    //------------------------------
                //    if (!bInitSysDate)
                //    {
                //        param.Add("IsInitSysDate", false);
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //}
                //else if (typeof(CompTimePicker) == type)
                //{
                //    //------------------------------
                //    // 時間
                //    //------------------------------
                //    if (!bInitSysDate)
                //    {
                //        param.Add("IsInitSysDate", false);
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //}
                //else if (typeof(CompDateFromTo) == type)
                //{
                //    //------------------------------
                //    // 日付FromTo
                //    //------------------------------
                //    if (!bInitSysDate)
                //    {
                //        param.Add("IsInitSysDate", false);
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValueFrom", ((List<string>)initVal)[0]);
                //        param.Add("InitialValueTo", ((List<string>)initVal)[1]);
                //    }
                //}
                //else if (typeof(CompTimeFromTo) == type)
                //{
                //    //------------------------------
                //    // 時間FromTo
                //    //------------------------------
                //    if (!bInitSysDate)
                //    {
                //        param.Add("IsInitSysDate", false);
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValueFrom", ((List<string>)initVal)[0]);
                //        param.Add("InitialValueTo", ((List<string>)initVal)[1]);
                //    }
                //}
                //else if (typeof(CompDateTimeFromTo) == type)
                //{
                //    //------------------------------
                //    // 日時FromTo
                //    //------------------------------
                //    if (!bInitSysDate)
                //    {
                //        param.Add("IsInitSysDate", false);
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValueFrom", ((List<string>)initVal)[0]);
                //        param.Add("InitialValueTo", ((List<string>)initVal)[1]);
                //    }
                //}
                //else if (typeof(CompDropDownDataGrid) == type)
                //{
                //    //------------------------------
                //    // ドロップダウングリッド
                //    //------------------------------
                //    List<ComponentColumnsInfo> _dropDownColumns = new();
                //    List<ComponentColumnsInfo> columns = ComponentColumns.Where(_ => _.ComponentName == info.Property).ToList();
                //    for (int i = 0; columns.Count > i; i++)
                //    {
                //        _dropDownColumns.Add(new ComponentColumnsInfo() { Property = $"Value{i + 1}", Title = columns[i].Title, Type = columns[i].Type, Width = columns[i].Width, TextAlign = columns[i].TextAlign });
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValues", initVal);
                //    }
                //    param.Add("Columns", _dropDownColumns);
                //    param.Add("Data", data!);
                //}
                //else if (typeof(CompDropDownDataGridSingle) == type)
                //{
                //    //------------------------------
                //    // ドロップダウングリッド(Single)
                //    //------------------------------
                //    List<ComponentColumnsInfo> _dropDownColumns = new();
                //    List<ComponentColumnsInfo> columns = ComponentColumns.Where(_ => _.ComponentName == info.Property).ToList();
                //    for (int i = 0; columns.Count > i; i++)
                //    {
                //        _dropDownColumns.Add(new ComponentColumnsInfo() { Property = $"Value{i + 1}", Title = columns[i].Title, Type = columns[i].Type, Width = columns[i].Width, TextAlign = columns[i].TextAlign });
                //    }
                //    if (initVal is not null)
                //    {
                //        param.Add("InitialValue", initVal);
                //    }
                //    param.Add("Columns", _dropDownColumns);
                //    param.Add("Data", data!);
                //}

                // コンポーネント情報生成
                CompItemInfo item = new()
                {
                    CompType = type,
                    CompParam = param,
                    TitleLabel = strTitle,
                    DispTitleLabel = bnDispTitle,
                    DispRequiredSuffix = DispRequired,
                    ComponentName = info.ComponentName,
                    ViewName = info.ViewName,
                    ValueDataType = typeKey,
                };
                listItem.Add(item);

            }
            return result;
        }

        /// <summary>
        /// コンポーネントの入力値をDictionary<string, object>で取得
        /// ※複数選択値があるコンポーネントのobjectはList<string>で返る
        /// </summary>
        /// <param name="compItems"></param>
        /// <param name="bnGetEmpty">未入力の値も取得するか。true：取得する、false：取得しない</param>
        /// <returns></returns>
        public Dictionary<string, object> GetCompInputValues(List<List<CompItemInfo>> compItems, bool bnGetEmpty = false)
        {
            Dictionary<string, object> result = new();

            foreach (List<CompItemInfo> listItem in compItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item.CompObj?.Instance is CompBase compBase)
                    {
                        compBase.AddkeyValuePair(result, item.TitleLabel, bnGetEmpty);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// コンポーネントの入力値をList<UserComponentSettingsInfo>で取得する
        /// </summary>
        /// <param name="compItems"></param>
        /// <param name="exclusionKeys"></param>
        /// <returns></returns>
        public List<UserComponentSettingsInfo> GetSettingsInfo(List<List<CompItemInfo>> compItems, List<ComponentsInfo> exclusionInfos)
        {
            List<UserComponentSettingsInfo> result = new();
            Dictionary<string, object> inputValues = GetCompInputValues(compItems);

            foreach (List<CompItemInfo> listItem in compItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    // 除外リストに含まれる場合はスキップ
                    if (exclusionInfos.Where(_ => _.AttributesName == item.TitleLabel).Count() > 0)
                    {
                        continue;
                    }

                    if (inputValues.TryGetValue(item.TitleLabel, out object? value))
                    {
                        if (value is List<string> vals)
                        {
                            for (int i = 0; i < vals.Count; i++)
                            {
                                UserComponentSettingsInfo info = new()
                                {
                                    ComponentName = item.ComponentName,
                                    ViewName = item.ViewName,
                                    PropertyKey = item.TitleLabel,
                                    ValueKeyId = i + 1,
                                    Value = vals[i],
                                    ValueDataType = item.ValueDataType,
                                };
                                result.Add(info);
                            }
                        }
                        else
                        {
                            UserComponentSettingsInfo info = new()
                            {
                                ComponentName = item.ComponentName,
                                ViewName = item.ViewName,
                                PropertyKey = item.TitleLabel,
                                ValueKeyId = 1,
                                Value = value == null ? string.Empty : value.ToString(),
                                ValueDataType = item.ValueDataType,
                            };
                            result.Add(info);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// コンポーネントにDisabled属性を設定する
        /// </summary>
        /// <param name="compItems"></param>
        /// <param name="bnDisabled"></param>
        public void SetCompItemListDisabled(List<List<CompItemInfo>> compItems, bool bnDisabled)
        {
            foreach (List<CompItemInfo> listItem in compItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item.CompObj != null && item.CompObj?.Instance is CompBase compBase)
                    {
                        compBase.Disabled = bnDisabled;
                        compBase.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// ピック予定情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<List<PickPlansInfo>> GetPickPlansInfoAsync()
        {
            List<PickPlansInfo> lstPickPlansInfo = new();
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_ピッキング_残予定"
                };
                ResponseValue[]? resValues = await GetResponseValue(select);
                if (null != resValues)
                {
                    foreach (ResponseValue resValue in resValues)
                    {
                        if (resValue != null && resValue.Values != null && resValue.Columns != null)
                        {
                            PickPlansInfo pickPlansInfo = new()
                            {
                                DeliveryCd = GetValueString(resValue, "倉庫配送先ｺｰﾄﾞ"),
                                AreaCd = GetValueString(resValue, "倉庫ｺｰﾄﾞ"),
                                ZoneCd = GetValueString(resValue, "ｿﾞｰﾝｺｰﾄﾞ")
                            };
                            lstPickPlansInfo.Add(pickPlansInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return lstPickPlansInfo;
        }

        /// <summary>
        /// WMS作業日を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime?> GetWmsDate()
        {
            DateTime? dtRet = null;
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "WMS_STATUS"
                };
                ResponseValue[]? resValues = await GetResponseValue(select);
                if (null != resValues && resValues.Length > 0)
                {
                    string strDate = GetValueString(resValues[0], "WMS_DATE");
                    if (strDate.Length == 8)
                    {
                        strDate = strDate[..4] + "/" + strDate.Substring(4, 2) + "/" + strDate.Substring(6, 2);
                        if (DateTime.TryParse(strDate, out DateTime date))
                        {
                            dtRet = date;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return dtRet;
        }

        /// <summary>
        /// WMS作業日加算を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime?> GetWmsDateAdd()
        {
            DateTime? dtRet = null;
            try
            {
                // WMS作業日を取得する
                dtRet = await GetWmsDate();

                if (dtRet != null)
                {
                    // DEFINE_SYSTEM_PARAMETERSからWMS加算日付を取得する
                    int days = 1;
                    if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
                    {
                        SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
                        days = sysParams.PC_DateInitWmsAddDays;
                    }
                    dtRet = ((DateTime)dtRet).AddDays(days);
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return dtRet;
        }

        /// <summary>
        /// システム状態区分(WMS_STATUS)取得
        /// </summary>
        /// <returns>0:通常、1:日時更新中、9:オフライン</returns>
        public async Task<(int, bool)> GetSystemStatusType()
        {
            int status = -1;
            bool isAuto = false;
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "WMS_STATUS"
                };
                ResponseValue[]? resValues = await GetResponseValue(select);
                if (null != resValues && resValues.Length > 0)
                {
                    status = GetValueInt(resValues[0], "SYSTEM_STATUS_TYPE");
                    isAuto = GetValueBool(resValues[0], "IS_AUTO_DAILY_UPDATE");
                }
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return (status, isAuto);
        }

        #region DEFINE関連全画面データ取得

        /// <summary>
        /// ページタイトルを取得する
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetPageTitleAsyncAll()
        {
            Dictionary<string, string> pageName = new();
            try
            {
                ClassNameSelect select = new()
                {
                    viewName = "DEFINE_PAGE_VALUES"
                    ,
                    tsqlHints = EnumTSQLhints.NOLOCK
                };
                select.selectParam.Add("CLASS_NAME");
                select.selectParam.Add("PAGE_NAME");
                select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

                ResponseValue[]? resValues = await GetResponseValue(select);
                if (resValues is null)
                {
                    throw new Exception("GetPageTitleAsyncAll_resValues is null");
                }

                foreach (ResponseValue item in resValues)
                {
                    string strClassName = GetValueString(item, "CLASS_NAME");
                    pageName[strClassName] = GetValueString(item, "PAGE_NAME");
                }

            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
            }
            return pageName;
        }

        /// <summary>
        /// DEFINE_COMPNENTSテーブル情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, List<ComponentsInfo>>> GetComponetnsInfoAll()
        {
            // 取得カラム名を指定する（共通）
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_COMPONENTS"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            Dictionary<string, List<ComponentsInfo>> data = new();
            List<ComponentsInfo> list = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetComponetnsInfoAll_resItems is null");
            }
            for (int i = 0; resItems.Length > i; i++)
            {
                ResponseValue item = resItems[i];
                string strClassName = GetValueString(item, "CLASS_NAME");
                ComponentsInfo newRow = new()
                {
                    ComponentName = GetValueString(item, "COMPONENT_NAME"),
                    AttributesName = GetValueString(item, "ATTRIBUTES_NAME"),
                    Value = GetValueString(item, "VALUE"),
                    ValueObjectType = GetValueByte(item, "VALUE_OBJECT_TYPE"),
                    Type = GetValueType(item, "COMPONET_DATA_TYPE"),
                    ValueMin = GetValueInt(item, "VALUE_MIN"),
                    ValueMax = GetValueInt(item, "VALUE_MAX")
                };
                list.Add(newRow);
                bool bClassChange = false;
                if (resItems.Length > i + 1)
                {
                    // 次のクラス名を判断する
                    string strNextClassName = GetValueString(resItems[i + 1], "CLASS_NAME");
                    if (strNextClassName != strClassName)
                    {
                        bClassChange = true;
                    }
                }
                else
                {
                    bClassChange = true;
                }
                if (bClassChange)
                {
                    data[strClassName] = list;
                    list = new();
                }
            }
            return data;
        }

        /// <summary>
        /// DEFINE_COMPNENT_COLUMNSテーブル情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, List<ComponentColumnsInfo>>> GetGridColumnsDataAll()
        {
            ClassNameSelect select = new()
            {
                viewName = $"dbo.GetObjectComponetColumnsDefine(null, null, null)",
                columnsDefineName = $"GetObjectComponetColumnsDefine"
            };
            select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });
            select.orderByParam.Add(new OrderByParam { field = "COMPONENT_NAME" });
            select.orderByParam.Add(new OrderByParam { field = "VIEW_NAME" });
            select.orderByParam.Add(new OrderByParam { field = "ColumnId" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            Dictionary<string, List<ComponentColumnsInfo>> data = new();
            List<ComponentColumnsInfo> list = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetGridColumnsDataAll_resItems is null");
            }

            for (int i = 0; resItems.Length > i; i++)
            {
                ResponseValue item = resItems[i];
                string strClassName = GetValueString(item, "CLASS_NAME");
                ComponentColumnsInfo newRow = new()
                {
                    ComponentName = GetValueString(item, "COMPONENT_NAME"),
                    ViewName = GetValueString(item, "VIEW_NAME"),
                    Property = GetValueString(item, "PROPERTY_KEY"),
                    Title = GetValueString(item, "PROPERTY_KEY"),
                    ValueMin = GetValueDecimalNull(item, "VALUE_MIN"),
                    ValueMax = GetValueDecimalNull(item, "VALUE_MAX"),
                    Width = GetValueInt(item, "WIDTH"),
                    Type = GetValueType(item, "COMPONET_DATA_TYPE"),
                    TypeText = GetValueString(item, "COMPONET_DATA_TYPE"),
                    TextAlign = GetValueEnum<TextAlign>(item, "TEXT_ALIGN"),
                    Resizable = GetValueBool(item, "IS_RESIZABLE"),
                    Reorderable = GetValueBool(item, "IS_REORDERABLE"),
                    Sortable = GetValueBool(item, "IS_SORTABLE"),
                    Filterable = GetValueBool(item, "IS_FILTERABLE"),
                    FormatString = GetValueString(item, "FORMAT_STRING"),
                    IsEdit = GetValueBool(item, "IS_EDIT"),
                    IsDataExport = GetValueBool(item, "IS_DATA_EXPORT"),
                    IsSearchCondition = GetValueBool(item, "IS_SEARCH_CONDITION"),
                    SummaryType = GetValueInt(item, "IS_SUMMARY"),
                    SearchValuesViewName = GetValueString(item, "SEARCH_VALUES_VIEW_NAME"),
                    SearchDataTypeKey = GetValueString(item, "SEARCH_DATA_TYPE_KEY"),
                    InputRequired = GetValueBool(item, "SEARCH_INPUT_REQUIRED"),
                    OrderbyRank = GetValueInt(item, "ORDERBY_RANK"),
                    SearchLayoutGroup = GetValueInt(item, "SEARCH_LAYOUT_GROUP"),
                    SearchLayoutDispOrder = GetValueInt(item, "SEARCH_LAYOUT_DISP_ORDER"),
                    EditInputRequired = GetValueBool(item, "EDIT_INPUT_REQUIRED"),
                    RegularExpressionString = GetValueString(item, "REGULAR_EXPRESSION_STRING"),
                    EditDialogLayoutGroup = GetValueInt(item, "EDIT_DIALOG_LAYOUT_GROUP"),
                    EditDialogLayoutDispOrder = GetValueInt(item, "EDIT_DIALOG_LAYOUT_DISP_ORDER"),
                    EditType = GetValueInt(item, "EDIT_TYPE"),
                    EditValuesViewName = GetValueString(item, "EDIT_VAUES_VIEW_NAME"),
                    EditDataTypeKey = GetValueString(item, "EDIT_DATA_TYPE_KEY"),
                    InlineEdit = GetValueBool(item, "IS_INLINE_EDIT"),
                    DataType = GetValueString(item, "DataType"),
                    MaxLength = GetValueInt(item, "MaxLength"),
                    Precision = GetValueInt(item, "Precision"),
                    Scale = GetValueInt(item, "Scale"),
                    IsNullable = GetValueBool(item, "IsNullable"),
                };

                // RadzenDataGridColumnsのPropertyはソート時に\nがある状態だとエラーが発生する
                // Sampleは改行コードを持たせていてエラーとなっていないため、それに合わせる
                newRow.Property = newRow.Property.Replace("\\n", '\n'.ToString());

                list.Add(newRow);
                bool bClassChange = false;
                if (resItems.Length > i + 1)
                {
                    // 次のクラス名を判断する
                    string strNextClassName = GetValueString(resItems[i + 1], "CLASS_NAME");
                    if (strNextClassName != strClassName)
                    {
                        bClassChange = true;
                    }
                }
                else
                {
                    bClassChange = true;
                }
                if (bClassChange)
                {
                    data[strClassName] = list;
                    list = new();
                }
            }

            return data;
        }

        /// <summary>
        /// DEFINE_COMPONENT_PROGRAMSの情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, List<ComponentProgramInfo>>> GetComponentProgramInfoAll()
        {
            // 取得カラム名を指定する（共通）
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_COMPONENT_PROGRAMS"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

            ResponseValue[]? resItems = await GetResponseValue(select);
            Dictionary<string, List<ComponentProgramInfo>> data = new();
            List<ComponentProgramInfo> list = new();
            if (resItems is null)
            {
                throw new NullReferenceException("GetComponentProgramInfoAll_resItems is null");
            }
            for (int i = 0; resItems.Length > i; i++)
            {
                ResponseValue item = resItems[i];
                string strClassName = GetValueString(item, "CLASS_NAME");
                ComponentProgramInfo newRow = new()
                {
                    CurrentMethodName = GetValueString(item, "CURRENT_METHOD_NAME"),
                    CallMethodName = GetValueString(item, "CALL_METHOD_NAME"),
                    ComponentName = GetValueString(item, "COMPONENT_NAME"),
                    ExecOrderRank = GetValueByte(item, "EXEC_ORDER_RANK"),
                    ProcessProgramName = GetValueString(item, "PROCESS_PROGRAM_NAME"),
                    AuthorityLevelLower = GetValueByte(item, "AUTHORITY_LEVEL_LOWER"),
                    PrgramCallType = GetValueByte(item, "PRGRAM_CALL_TYPE"),
                    IsProgramReturn = GetValueByte(item, "IS_PROGRAM_RETURN"),
                    RetrunDataType = GetValueType(item, "RETRUN_DATA_TYPE"),
                    TimeoutValue = GetValueInt(item, "TIMEOUT_VALUE"),
                    RetryCount = GetValueByte(item, "RETRY_COUNT"),
                    ArgumentDataSetName = GetValueString(item, "ARGUMENT_DATA_SET_NAME"),
                    IsAsync = GetValueBool(item, "IS_ASYNC")
                };
                list.Add(newRow);
                bool bClassChange = false;
                if (resItems.Length > i + 1)
                {
                    // 次のクラス名を判断する
                    string strNextClassName = GetValueString(resItems[i + 1], "CLASS_NAME");
                    if (strNextClassName != strClassName)
                    {
                        bClassChange = true;
                    }
                }
                else
                {
                    bClassChange = true;
                }
                if (bClassChange)
                {
                    data[strClassName] = list;
                    list = new();
                }
            }
            return data;
        }

        #endregion

        #region セル背景色
        /// <summary>
        /// HTML AttributesにRESULT_CATEGORYのセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrResultCatgory(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "2":
                    // 警告
                    strBkColor = "#fac152";
                    bnWhite = false;
                    break;
                case "3":
                    // 異常
                    strBkColor = "#f9777f";
                    bnWhite = false;
                    break;
                default:
                    // 正常、該当なし
                    strBkColor = "#2cc8c8";
                    bnWhite = false;
                    break;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        /// <summary>
        /// HTML Attributesに入荷状態のセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrArrivalStatus(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "0":
                    strBkColor = "#ff0000";     // 赤
                    bnWhite = false;
                    break;
                case "1":
                    strBkColor = "#ffa500";     // 橙
                    bnWhite = false;
                    break;
                case "2":
                    strBkColor = "#ffff00";     // 黄
                    bnWhite = false;
                    break;
                case "3":
                    strBkColor = "#0000ff";     // 青
                    bnWhite = true;
                    break;
                case "4":
                    strBkColor = "#008000";     // 緑
                    bnWhite = true;
                    break;
                default:
                    return;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        /// <summary>
        /// HTML Attributesに出荷状態のセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrShipmentStatus(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "0":
                    strBkColor = "#ff0000";     // 赤
                    bnWhite = false;
                    break;
                case "1":
                    strBkColor = "#ffa500";     // 橙
                    bnWhite = false;
                    break;
                case "2":
                    strBkColor = "#ffff00";     // 黄
                    bnWhite = false;
                    break;
                case "3":
                    strBkColor = "#008000";     // 緑
                    bnWhite = true;
                    break;
                case "4":
                    strBkColor = "#800080";     // 紫
                    bnWhite = true;
                    break;
                case "5":
                    strBkColor = "#0000ff";     // 青
                    bnWhite = true;
                    break;
                default:
                    return;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        /// <summary>
        /// HTML Attributesに確定状態のセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrConfirmStatus(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "True":
                    strBkColor = "#223a70";     // 紺
                    bnWhite = true;
                    break;
                default:
                    return;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        /// <summary>
        /// HTML Attributesに実績送信状態のセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrSendResultsStatus(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "True":
                    strBkColor = "#223a70";     // 紺
                    bnWhite = true;
                    break;
                default:
                    return;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        /// <summary>
        /// HTML Attributesに開始時間、締切時間の警告・超過のセル背景色を追加
        /// </summary>
        /// <param name="strRemainingTime">残時間[分]</param>
        /// <param name="intWarnTime">警告時間[分]</param>
        /// <param name="attr"></param>
        public void AddAttrWarnExcessTime(string? strRemainingTime, int intWarnTime, IDictionary<string, object> attr)
        {
            if (!int.TryParse(strRemainingTime, out int intRemainingTime))
            {
                return;
            }

            if (0 >= intRemainingTime)
            {
                // 超過
                attr.Add("style", $"background-color: #ff0000;");
            }
            else if (intWarnTime >= intRemainingTime)
            {
                // 警告
                attr.Add("style", $"background-color: #ffff00;");
            }
        }

        /// <summary>
        /// HTML Attributesに差異有状態のセル背景色変更のためstyle追加
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="attr"></param>
        public void AddAttrDifferenceStatus(string? strType, IDictionary<string, object> attr)
        {
            string strBkColor;
            bool bnWhite;
            switch (strType)
            {
                case "差異有":
                    strBkColor = "#ff0000";     // 赤
                    bnWhite = false;
                    break;
                default:
                    return;
            }
            attr.Add("style", $"background-color: {strBkColor};");
            if (bnWhite)
            {
                attr.Add("class", "white-text");
            }
        }

        #endregion

        #region 計算

        /// <summary>
        /// パーセントを計算
        /// </summary>
        /// <param name="decNum"></param>
        /// <param name="decDen"></param>
        /// <param name="intDecPoint"></param>
        /// <returns></returns>
        public decimal GetPercent(decimal decNum, decimal decDen, int intDecPoint = 0)
        {
            decimal decRet = 0.0m;
            if (decDen == 0.0m)
            {
                return decRet;
            }
            if (intDecPoint < 0)
            {
                intDecPoint = 0;
            }
            double pow = Math.Pow(10, intDecPoint);
            decRet = decNum / decDen * 100;
            return Math.Floor(decRet * (int)pow) / (int)pow;
        }

        #endregion

        #region メッセージ関連

        public async Task DialogShowOK(string message, string title = "確認", int? width = null, int? height = null)
        {
            await DialogShowOK(new Dictionary<string, object> { { "MessageContent", message } }, title, width ?? DialogShowOKWidth, height ?? DialogShowOKHeight);
        }

        public async Task DialogShowOK(Dictionary<string, object> attr, string title = "確認", int width = 350, int height = 200)
        {
            title = title.Replace("\\n", "");
            await _dialogService.OpenAsync<DialogMsgOKContent>(title,
                attr,
                new DialogOptions() { Width = $"{width}px", Height = $"{height}px", Resizable = false, Draggable = false });
        }

        public async Task<bool> DialogShowYesNo(string message, string title = "確認", int? width = null, int? height = null)
        {
            return await DialogShowYesNo(new Dictionary<string, object> { { "MessageContent", message } }, title, width ?? DialogShowYesNoWidth, height ?? DialogShowYesNoHeight);
        }

        public async Task<bool> DialogShowYesNo(Dictionary<string, object> attr, string title = "確認", int width = 350, int height = 200)
        {
            title = title.Replace("\\n", "");
            bool? ret = await _dialogService.OpenAsync<DialogMsgYesNoContent>(title,
                attr,
                new DialogOptions() { Width = $"{width}px", Height = $"{height}px", Resizable = false, Draggable = false });

            bool retb = ret is not null && (bool)ret;

            return retb;
        }

        public async Task DialogShowBusy(string message = "処理中..", int? width = null, int? height = null)
        {
            await DialogShowBusy(new Dictionary<string, object> { { "MessageContent", message } }, width ?? MessageDialogWidth, height ?? MessageDialogHeight);
        }

        public async Task DialogShowBusy(Dictionary<string, object> attr, int width = 200, int height = 155)
        {
            await _dialogService.OpenAsync<DialogBusyContent>(string.Empty,
                attr,
                new DialogOptions()
                {
                    Width = $"{width}px",
                    Height = $"{height}px",
                    Resizable = false,
                    Draggable = false,
                    ShowTitle = false,
                    ShowClose = false,
                    Style = "min-height:auto;min-width:auto;width:auto",
                    CloseDialogOnEsc = false
                });
        }

        public async Task DialogClose()
        {
            _dialogService.Close();
            await Task.Delay(0);
        }
        #endregion
    }
}
