using Microsoft.JSInterop;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 予定外入荷
    /// </summary>
    public partial class ArrivalsDetailsAddMainte : ChildPageBasePC
    {
        public const string STR_ATTRIBUTE_ADD_DIALOG = "AttributesAddDialog";
        public const string STR_ATTRIBUTE_EDIT_DIALOG = "AttributesEditDialog";

        public const string STORAGEKEY_ARRIVAL_NO = "ArrivalsDetailsAddMainte_入荷No";

        public const string PROPKEY_等級_CODE = "等級";
        public const string PROPKEY_等級_NAME = "等級名";
        public const string PROPKEY_階級_CODE = "階級";
        public const string PROPKEY_階級_NAME = "階級名";
        public const string PROPKEY_荷姿_CODE = "荷姿";
        public const string PROPKEY_荷姿_NAME = "荷姿名";
        public const string PROPKEY_入数 = "入数";
        public const string PROPKEY_予定ケース数 = "入荷予定数(ケース)";
        public const string PROPKEY_予定バラ数 = "入荷予定数(バラ)";
        public const string PROPKEY_賞味期限_FRM = "賞味期限";
        public const string PROPKEY_賞味期限 = "賞味期限値";
        public const string PROPKEY_特別管理品_CODE = "特別管理品区分";
        public const string PROPKEY_特別管理品_NAME = "特別管理品";
        public const string PROPKEY_作業者説明_CODE = "作業者説明コード";
        public const string PROPKEY_作業者説明_NAME = "作業者説明";
        public const string PROPKEY_荷印 = "荷印";

        private const string PROPKEY_実績ケース数 = "入荷実績数(ケース)";
        private const string PROPKEY_実績バラ数 = "入荷実績数(バラ)";

        private const string PROPKEY_ARRIVAL_NO = "入荷No";
        private const string PROPKEY_MODE = "Mode";
        private const string PROPKEY_INVOICE_NO = "送状No";
        private const string PROPKEY_PRODUCT_CD = "品名コード";
        private const string PROPKEY_PRODUCT_NAME = "品名";
        private const string PROPKEY_SHIPPER_CD = "出荷者コード";
        private const string PROPKEY_SHIPPER_NAME = "出荷者名";
        private const string PROPKEY_PRODUCTION_AREA_CD = "産地コード";
        private const string PROPKEY_PRODUCTION_AREA_NAME = "産地名";
        private const string PROPKEY_DIVISION_CD = "課コード";
        private const string PROPKEY_DIVISION_NAME = "課名";
        private const string PROPKEY_SALES_DATE = "販売予定日";
        private const string PROPKEY_DEPARTURE_DATE = "発日";
        private const string PROPKEY_ARRIVAL_DATE = "着日";
        private const string PROPKEY_DETAIL_NO = "明細No";
        private const string PROPKEY_ARRIVAL_SOURCE = "入荷元区分";
        private const string PROPKEY_REMARKS = "備考";

        private const string STR_AUTO_INCREMENT = "自動採番";

        private CompItemInfo? compArrivalNo;
        private CompItemInfo? compInvoiceNo;
        private CompItemInfo? compProductCd;
        private CompItemInfo? compShipperCd;
        private CompItemInfo? compProductionAreaCd;
        private CompItemInfo? compDivisionCd;
        private CompItemInfo? compSalesDate;
        private CompItemInfo? compDepartureDate;
        private CompItemInfo? compArrivalDate;

        /// <summary>
        /// モード　false:追加、true:修正
        /// </summary>
        private bool mMode = false;

        /// <summary>
        /// 入荷No
        /// </summary>
        private string mArrivalNo = string.Empty;

        /// <summary>
        /// 入荷元区分
        /// </summary>
        private string mArrivalSource = "0";

        /// <summary>
        /// 備考
        /// </summary>
        private string mRemarks = string.Empty;

        private List<ValueTextInfo> mLstProduct = new();

        #region override

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            try
            {
                // 入荷No取得
                mArrivalNo = await LocalStorage.GetItemAsync<string>(STORAGEKEY_ARRIVAL_NO);

                if (string.IsNullOrEmpty(mArrivalNo))
                {
                    // 追加モード
                    mMode = false;
                }
                else
                {
                    // 修正モード
                    mMode = true;
                }

                // レンダリング抑制解除
                ChildBaseService.BasePageInitilizing = false;

                //Blazor へ状態変化を通知
                StateHasChanged();

                // グリッド更新
                await RefreshGrid();

                // コンポーネントにプロパティ設定
                SetCompProperties();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            // 検索条件保存を利用する画面のみ、検索条件初期値を取得
            Dictionary<string, object> InitialData = new();
            if (UseSaveSearch)
            {
                InitialData = await ComService.GetUserComponentSettingsInfo(ClassName);
            }

            // 初期値がセットされていない場合、DEFINE_COMPONENTSの初期値を設定
            if (InitialData.Count == 0)
            {
                List<ComponentsInfo> lstInitInfo = _componentsInfo.Where(_ => _.ComponentName == STR_ATTRIBUTE_SEARCH_INITIAL_VALUE).ToList();
                foreach (ComponentsInfo initInfo in lstInitInfo)
                {
                    if (!string.IsNullOrEmpty(initInfo.Value))
                    {
                        // 【注意】複数の初期値を設定できるコンポーネントは、カンマ(,)区切りで初期値が登録されている前提です
                        string[] values = initInfo.Value.Split(',');
                        InitialData[initInfo.AttributesName] = values.Length > 1 ? new List<string>(values) : values[0];
                    }
                }
            }

            // カラム設定データから検索条件のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = _gridColumns
                .Where(_ => _.IsSearchCondition == true)
                .OrderBy(_ => _.SearchLayoutGroup)
                .ThenBy(_ => _.SearchLayoutDispOrder)
                .ToList();

            // 検索条件コンポーネント情報を作成
            _searchCompItems = await ComService.GetCompItemInfo(listInfo, InitialData, _componentColumns, _componentsInfo, true, false);
        }

        /// <summary>
        /// F5ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF5(object? sender, object? e)
        {
            try
            {
                // 入力チェック
                if (!CheckInput())
                {
                    return;
                }

                // 明細件数チェック
                if (_gridData.Count() == 0)
                {
                    await ComService.DialogShowOK($"明細が無いため変更確定は実行出来ません。", pageName);
                    return;
                }

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["HeaderData"] = GetHeaderValues(),
                    ["DetailData"] = new List<IDictionary<string, object>>(_gridData)
                };

                // ダイアログ情報を取得
                Dictionary<string, object> attr = new(GetAttributes("AttributesConfirmDialog"));
                string strDialogTitle = "予定外入荷確定";
                int intDialogWidth = 700;
                int intDialogHeight = 400;
                if (attr.TryGetValue("DialogTitle", out object? obj))
                {
                    strDialogTitle = obj.ToString()!;
                }
                if (attr.TryGetValue("DialogWidth", out obj))
                {
                    _ = int.TryParse(obj.ToString(), out intDialogWidth);
                }
                if (attr.TryGetValue("DialogHeight", out obj))
                {
                    _ = int.TryParse(obj.ToString(), out intDialogHeight);
                }

                // ダイアログ表示
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic ret = await DialogService.OpenAsync<DialogArrivalsMaintenanceContent>(
                    $"{strDialogTitle}",
                    dlgParam,
                    new DialogOptions()
                    {
                        Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                        Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                        Resizable = true,
                        Draggable = true
                    }
                );

                if (ret is null)
                {
                    // キャンセル
                    return;
                }

                // 戻る
                if (JS is not null)
                {
                    await JS.InvokeVoidAsync("history.back");
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F6ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF6(object? sender, object? e)
        {
            try
            {
                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new(GetAttributes(STR_ATTRIBUTE_ADD_DIALOG));

                // ダイアログタイトルを取得
                string strDialogTitle = string.Empty;
                Type? tDialogType = null;
                enumDialogMode dlgMode = enumDialogMode.Add;
                string strWidth = "500";
                if (dlgParam.TryGetValue("DialogTitle", out object? obj))
                {
                    strDialogTitle = obj.ToString()!;
                }
                if (dlgParam.TryGetValue("Mode", out obj))
                {
                    dlgMode = (enumDialogMode)obj;
                }

                // ダイアログの初期値設定
                Dictionary<string, object> hInitData = GetHeaderValues();
                dlgParam["InitialData"] = hInitData;

                // 以下はダイアログでは必要ないので取り出した後削除する
                if (dlgParam.TryGetValue("DialogWidth", out obj))
                {
                    strWidth = obj.ToString()!;
                    _ = dlgParam.Remove("DialogWidth");
                }
                if (dlgParam.TryGetValue("DialogType", out obj))
                {
                    tDialogType = (Type?)obj;
                    _ = dlgParam.Remove("DialogType");
                }

                // サイドダイアログを表示する際に処理中ダイアログが表示されているとバリデートがかからないため閉じる
                DialogService.Close();
                ContainerMainLayout.SetIsBusyDialogClose(false);

                // 追加ダイアログ表示
                dynamic ret = await DialogService.OpenSideAsync<DialogArrivalsFixSideContent>($"{pageName}",
                    dlgParam,
                    new SideDialogOptions { Width = $"{strWidth}px", CloseDialogOnOverlayClick = false, Position = DialogPosition.Right, ShowMask = true });
                if (ret is null or not IDictionary<string, object>)
                {
                    return;
                }

                // 行追加
                IDictionary<string, object> retVal = (IDictionary<string, object>)ret;
                Dictionary<string, object> newRow = new();
                foreach (ComponentColumnsInfo info in _gridColumns)
                {
                    if (retVal.TryGetValue(info.Property, out object? value))
                    {
                        newRow.Add(info.Property, value);
                    }
                    else
                    {
                        if (info.Property is PROPKEY_実績ケース数 or PROPKEY_実績バラ数)
                        {
                            newRow.Add(info.Property, 0);
                        }
                        else
                        {
                            newRow.Add(info.Property, string.Empty);
                        }
                    }
                }
                _gridData.Add(newRow);

                // グリッドデータ設定
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>(_gridData);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F7ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF7(object? sender, object? e)
        {
            try
            {
                // チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"入荷明細を修正する対象が選択されていません。", pageName);
                    return;
                }

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new(GetAttributes(STR_ATTRIBUTE_EDIT_DIALOG));

                // ダイアログタイトルを取得
                string strDialogTitle = string.Empty;
                Type? tDialogType = null;
                enumDialogMode dlgMode = enumDialogMode.Edit;
                string strWidth = "500";
                if (dlgParam.TryGetValue("DialogTitle", out object? obj))
                {
                    strDialogTitle = obj.ToString()!;
                }
                if (dlgParam.TryGetValue("Mode", out obj))
                {
                    dlgMode = (enumDialogMode)obj;
                }

                // ダイアログの初期値設定
                IDictionary<string, object> initialData = (_gridSelectedData is not null && _gridSelectedData.Count > 0) ? _gridSelectedData[0] : new Dictionary<string, object>();
                Dictionary<string, object> hInitData = GetHeaderValues();
                foreach (KeyValuePair<string, object> item in hInitData)
                {
                    initialData[item.Key] = item.Value;
                }
                dlgParam["InitialData"] = initialData;

                // 以下はダイアログでは必要ないので取り出した後削除する
                if (dlgParam.TryGetValue("DialogWidth", out obj))
                {
                    strWidth = obj.ToString()!;
                    _ = dlgParam.Remove("DialogWidth");
                }
                if (dlgParam.TryGetValue("DialogType", out obj))
                {
                    tDialogType = (Type?)obj;
                    _ = dlgParam.Remove("DialogType");
                }

                // サイドダイアログを表示する際に処理中ダイアログが表示されているとバリデートがかからないため閉じる
                DialogService.Close();
                ContainerMainLayout.SetIsBusyDialogClose(false);

                // 編集ダイアログ表示
                dynamic ret = await DialogService.OpenSideAsync<DialogArrivalsFixSideContent>($"{pageName}",
                    dlgParam,
                    new SideDialogOptions { Width = $"{strWidth}px", CloseDialogOnOverlayClick = false, Position = DialogPosition.Right, ShowMask = true });
                if (ret is null or not IDictionary<string, object>)
                {
                    return;
                }

                // 行更新
                IDictionary<string, object> retVal = (IDictionary<string, object>)ret;
                foreach (IDictionary<string, object> gridItem in _gridData)
                {
                    if (gridItem == _gridSelectedData[0])
                    {
                        foreach (ComponentColumnsInfo info in _gridColumns)
                        {
                            if (retVal.TryGetValue(info.Property, out object? value))
                            {
                                gridItem[info.Property] = value;
                            }
                        }
                        break;
                    }
                }

                // グリッドデータ設定
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>(_gridData);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F8ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF8(object? sender, object? e)
        {
            try
            {
                // チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"入荷明細を削除する対象が選択されていません。", pageName);
                    return;
                }

                // 確認
                bool? ret = await ComService.DialogShowYesNo("選択行を削除しますか？", pageName);
                if (true != ret)
                {
                    return;
                }

                // 選択行を削除
                foreach (IDictionary<string, object> selectItem in _gridSelectedData)
                {
                    for (int i = 0; i < _gridData.Count(); i++)
                    {
                        if (_gridData[i] == _gridSelectedData[0])
                        {
                            _gridData.RemoveAt(i);
                            break;
                        }
                    }
                }

                // グリッドデータ設定
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>(_gridData);

                // 選択データクリア
                _gridSelectedData = null;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F11ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF11(object? sender, object? e)
        {
            await Task.Delay(0);
            try
            {
                // 検索条件クリア
                ClearSearchCondition();

                // 全件削除
                _gridData.Clear();

                // グリッドデータ設定
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>(_gridData);

                // 選択データクリア
                _gridSelectedData = null;

                // 入荷Noセット
                if (compArrivalNo?.CompObj?.Instance is CompTextBox compTextBox)
                {
                    compTextBox.InputValue = false == mMode ? STR_AUTO_INCREMENT : mArrivalNo;
                    compTextBox.Refresh();
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            try
            {
                _storedData = new Dictionary<string, object>
                {
                    // 入荷Noセット
                    [PROPKEY_ARRIVAL_NO] = mArrivalNo
                };
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        #endregion

        /// <summary>
        /// グリッド更新
        /// </summary>
        /// <returns></returns>
        private async Task RefreshGrid()
        {
            // グリッドクリア
            _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();

            // グリッド更新
            ClassNameSelect custom = new()
            {
                className = GetType().Name,
            };
            custom.whereParam.Add(PROPKEY_ARRIVAL_NO, new WhereParam { val = mArrivalNo, whereType = enumWhereType.Equal });
            _gridData = await ComService!.GetSelectGridData(_gridColumns, custom);
            _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData;

            // 選択データクリア
            _gridSelectedData = null;
        }

        /// <summary>
        /// コンポーネントのプロパティ設定
        /// </summary>
        private void SetCompProperties()
        {
            IDictionary<string, object> initItem = _gridData.Count() > 0 ? _gridData[0] : new Dictionary<string, object>();

            // 各コンポーネントのプロパティ設定
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    string strVal = string.Empty;
                    if (initItem.TryGetValue(item.TitleLabel, out object value))
                    {
                        strVal = value.ToString();
                    }

                    switch (item.TitleLabel)
                    {
                        case PROPKEY_ARRIVAL_NO:
                            // 入荷No
                            compArrivalNo = item;
                            if (item?.CompObj?.Instance is CompTextBox txtArrivalNo)
                            {
                                txtArrivalNo.Disabled = true;
                                txtArrivalNo.InputValue = false == mMode ? STR_AUTO_INCREMENT : mArrivalNo;
                                txtArrivalNo.Refresh();
                            }
                            break;
                        case PROPKEY_INVOICE_NO:
                            // 送状No
                            compInvoiceNo = item;
                            if (item?.CompObj?.Instance is CompTextBox txtInvoiceNo)
                            {
                                txtInvoiceNo.InputValue = strVal;
                                txtInvoiceNo.Refresh();
                            }
                            break;
                        case PROPKEY_PRODUCT_CD:
                            // 品名コード
                            compProductCd = item;
                            if (item?.CompObj?.Instance is CompDropDownDataGridSingle ddgProductCd)
                            {
                                mLstProduct = ddgProductCd.Data.ToList();
                                ddgProductCd.OnChangeCallback -= ProductCd_ValueChanged;
                                ddgProductCd.OnChangeCallback += ProductCd_ValueChanged;
                                ddgProductCd.InputValue = strVal;
                                ddgProductCd.Refresh();
                            }
                            break;
                        case PROPKEY_SHIPPER_CD:
                            // 出荷者コード
                            compShipperCd = item;
                            if (item?.CompObj?.Instance is CompDropDownDataGridSingle ddgShipperCd)
                            {
                                ddgShipperCd.InputValue = strVal;
                                ddgShipperCd.Refresh();
                            }
                            break;
                        case PROPKEY_PRODUCTION_AREA_CD:
                            // 産地コード
                            compProductionAreaCd = item;
                            if (item?.CompObj?.Instance is CompDropDownDataGridSingle ddgProductionAreaCd)
                            {
                                ddgProductionAreaCd.InputValue = strVal;
                                ddgProductionAreaCd.Refresh();
                            }
                            break;
                        case PROPKEY_DIVISION_CD:
                            // 課コード
                            compDivisionCd = item;
                            if (item?.CompObj?.Instance is CompDropDownDataGridSingle ddgDivisionCd)
                            {
                                ddgDivisionCd.InputValue = strVal;
                                ddgDivisionCd.Refresh();
                            }
                            break;
                        case PROPKEY_SALES_DATE:
                            // 販売予定日
                            compSalesDate = item;
                            if (item?.CompObj?.Instance is CompDatePicker dtpSalesDate)
                            {
                                if (true == mMode)
                                {
                                    dtpSalesDate.InputValue = strVal;
                                }
                                dtpSalesDate.Refresh();
                            }
                            break;
                        case PROPKEY_DEPARTURE_DATE:
                            // 発日
                            compDepartureDate = item;
                            if (item?.CompObj?.Instance is CompDatePicker dtpDepartureDate)
                            {
                                if (true == mMode)
                                {
                                    dtpDepartureDate.InputValue = strVal;
                                }
                                dtpDepartureDate.Refresh();
                            }
                            break;
                        case PROPKEY_ARRIVAL_DATE:
                            // 着日
                            compArrivalDate = item;
                            if (item?.CompObj?.Instance is CompDatePicker dtpArrivalDate)
                            {
                                if (true == mMode)
                                {
                                    dtpArrivalDate.InputValue = strVal;
                                }
                                dtpArrivalDate.Refresh();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // 入荷元区分
            {
                if (initItem.TryGetValue(PROPKEY_ARRIVAL_SOURCE, out object? obj))
                {
                    mArrivalSource = obj?.ToString();
                }
            }
            // 備考
            {
                if (initItem.TryGetValue(PROPKEY_REMARKS, out object? obj))
                {
                    mRemarks = obj?.ToString();
                }
            }
        }

        /// <summary>
        /// ヘッダコンポーネント入力値取得
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetHeaderValues()
        {
            Dictionary<string, object> retVal = new()
            {
                // モード
                [PROPKEY_MODE] = mMode,
                // 入荷No
                [PROPKEY_ARRIVAL_NO] = mArrivalNo,
                // 入荷元区分
                [PROPKEY_ARRIVAL_SOURCE] = mArrivalSource,
                // 備考
                [PROPKEY_REMARKS] = mRemarks
            };

            // 送状No
            {
                retVal[PROPKEY_INVOICE_NO] = string.Empty;
                if (compInvoiceNo?.CompObj?.Instance is CompTextBox compTextBox)
                {
                    if (!string.IsNullOrEmpty(compTextBox.InputValue))
                    {
                        retVal[PROPKEY_INVOICE_NO] = compTextBox.InputValue;
                    }
                }
            }
            // 品名コード・品名
            {
                retVal[PROPKEY_PRODUCT_CD] = string.Empty;
                retVal[PROPKEY_PRODUCT_NAME] = string.Empty;
                if (compProductCd?.CompObj?.Instance is CompDropDownDataGridSingle compDropDownDataGridSingle)
                {
                    ValueTextInfo? info = compDropDownDataGridSingle.GetSelectedData();
                    if (null != info)
                    {
                        retVal[PROPKEY_PRODUCT_CD] = info.Value;
                        retVal[PROPKEY_PRODUCT_NAME] = info.Text;
                    }
                }
            }
            // 出荷者コード・出荷者名
            {
                retVal[PROPKEY_SHIPPER_CD] = string.Empty;
                retVal[PROPKEY_SHIPPER_NAME] = string.Empty;
                if (compShipperCd?.CompObj?.Instance is CompDropDownDataGridSingle compDropDownDataGridSingle)
                {
                    ValueTextInfo? info = compDropDownDataGridSingle.GetSelectedData();
                    if (null != info)
                    {
                        retVal[PROPKEY_SHIPPER_CD] = info.Value;
                        retVal[PROPKEY_SHIPPER_NAME] = info.Text;
                    }
                }
            }
            // 産地コード・産地名
            {
                retVal[PROPKEY_PRODUCTION_AREA_CD] = string.Empty;
                retVal[PROPKEY_PRODUCTION_AREA_NAME] = string.Empty;
                if (compProductionAreaCd?.CompObj?.Instance is CompDropDownDataGridSingle compDropDownDataGridSingle)
                {
                    ValueTextInfo? info = compDropDownDataGridSingle.GetSelectedData();
                    if (null != info)
                    {
                        retVal[PROPKEY_PRODUCTION_AREA_CD] = info.Value;
                        retVal[PROPKEY_PRODUCTION_AREA_NAME] = info.Text;
                    }
                }
            }
            // 課コード・課名
            {
                retVal[PROPKEY_DIVISION_CD] = string.Empty;
                retVal[PROPKEY_DIVISION_NAME] = string.Empty;
                if (compDivisionCd?.CompObj?.Instance is CompDropDownDataGridSingle compDropDownDataGridSingle)
                {
                    ValueTextInfo? info = compDropDownDataGridSingle.GetSelectedData();
                    if (null != info)
                    {
                        retVal[PROPKEY_DIVISION_CD] = info.Value;
                        retVal[PROPKEY_DIVISION_NAME] = info.Text;
                    }
                }
            }
            // 販売予定日
            {
                retVal[PROPKEY_SALES_DATE] = string.Empty;
                if (compSalesDate?.CompObj?.Instance is CompDatePicker compDatePicker)
                {
                    if (!string.IsNullOrEmpty(compDatePicker.InputValue))
                    {
                        retVal[PROPKEY_SALES_DATE] = compDatePicker.InputValue;
                    }
                }
            }
            // 発日
            {
                retVal[PROPKEY_DEPARTURE_DATE] = string.Empty;
                if (compDepartureDate?.CompObj?.Instance is CompDatePicker compDatePicker)
                {
                    if (!string.IsNullOrEmpty(compDatePicker.InputValue))
                    {
                        retVal[PROPKEY_DEPARTURE_DATE] = compDatePicker.InputValue;
                    }
                }
            }
            // 着日
            {
                retVal[PROPKEY_ARRIVAL_DATE] = string.Empty;
                if (compArrivalDate?.CompObj?.Instance is CompDatePicker compDatePicker)
                {
                    if (!string.IsNullOrEmpty(compDatePicker.InputValue))
                    {
                        retVal[PROPKEY_ARRIVAL_DATE] = compDatePicker.InputValue;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            if (editForm is null)
            {
                return true;
            }

            //EditContextのValidate()メソッドを実行することでSubmitと同等のイベントが発火
            return editForm!.EditContext.Validate();
        }

        /// <summary>
        /// 品名コードValueChanged
        /// </summary>
        /// <param name="args"></param>
        private void ProductCd_ValueChanged(object args)
        {
            try
            {
                ValueTextInfo? info = mLstProduct.Where(_ => _.Value == args.ToString()).FirstOrDefault();
                if (info != null)
                {
                    // 課コード
                    if (compDivisionCd?.CompObj?.Instance is CompDropDownDataGridSingle ddgDivisionCd)
                    {
                        ddgDivisionCd.InputValue = info.Value7;
                        ddgDivisionCd.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }
    }
}