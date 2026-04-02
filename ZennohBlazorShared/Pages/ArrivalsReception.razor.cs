using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷受付設定
    /// </summary>
    public partial class ArrivalsReception : ChildPageBasePC
    {
        public const string STORAGEKEY_RECEPTION_NO = "ArrivalsReception_受付No";

        private const string PROPKEY_ARRIVAL_NO = "入荷No";
        private const string PROPKEY_DETAIL_NO = "明細No";
        private const string PROPKEY_RECEPTION_NO = "受付No";
        private const string PROPKEY_CAR_NUMBER = "車番";
        private const string PROPKEY_TRUCK_TYPES = "トラック区分";
        private const string PROPKEY_CARGO_TYPES = "積荷区分";
        private const string PROPKEY_PHONE_NUMBER = "携帯番号";
        private const string PROPKEY_REMARKS = "備考";

        private const string PROPKEY_STOCKUP_CASE_QTY = "在庫計上数(ケース)";
        private const string PROPKEY_STOCKUP_BARA_QTY = "在庫計上数(バラ)";

        private CompItemInfo? compReceptionNo;
        private CompItemInfo? compCarNumber;
        private CompItemInfo? compTruckTypes;
        private CompItemInfo? compCargoTypes;
        private CompItemInfo? compPhoneNumber;
        private CompItemInfo? compRemarks;

        #region Override

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            try
            {
                // 受付No取得
                string strReceptionNo = await LocalStorage.GetItemAsync<string>(STORAGEKEY_RECEPTION_NO);
                await LocalStorage.RemoveItemAsync(STORAGEKEY_RECEPTION_NO);

                // レンダリング抑制解除
                ChildBaseService.BasePageInitilizing = false;

                //Blazor へ状態変化を通知
                StateHasChanged();

                // コンポーネントにプロパティ設定
                SetCompProperties(strReceptionNo);

                // 受付Noを受け取った場合、検索実行
                if (!string.IsNullOrEmpty(strReceptionNo))
                {
                    await ProcSearchAsync();
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F3ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task メソッド実行(ComponentProgramInfo info)
        {
            try
            {
                //権限チェック
                if (!await CheckAuthorities())
                {
                    return;
                }
                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["SelectedArrivalsData"] = _gridData
                };

                // ダイアログ情報を取得
                // TODO 1600などの固定値はAttriを使用してDEFINE_COMPONENTなどから取得するようにする
                int intDialogWidth = 1600;
                int intDialogHeight = 900;

                // 設定追加ダイアログ表示
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic retVal = await DialogService.OpenAsync<DialogArrivalsReceptionContent>(
                    $"{pageName}",
                    dlgParam,
                    new DialogOptions()
                    {
                        Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                        Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                        Resizable = true,
                        Draggable = true
                    }
                );

                if (null == retVal)
                {
                    return;
                }

                if (retVal is not List<IDictionary<string, object>>)
                {
                    return;
                }

                if (compReceptionNo?.CompObj?.Instance is CompTextBox compTextBox)
                {
                    // 受付No取得
                    string strReceptionNo = compTextBox.InputValue;
                    if (string.IsNullOrEmpty(strReceptionNo))
                    {
                        // 受付Noの採番処理
                        strReceptionNo = await ComService.GetManagementId(ClassName, SharedConst.workCategory.NyukaUketuke);

                        compTextBox.InputValue = strReceptionNo;
                        compTextBox.Refresh();
                    }
                }

                // 選択データをループ
                List<IDictionary<string, object>> retList = (List<IDictionary<string, object>>)retVal;
                foreach (IDictionary<string, object> retItem in retList)
                {
                    bool bnExists = false;
                    foreach (IDictionary<string, object> gridItem in _gridData)
                    {
                        if (retItem[PROPKEY_ARRIVAL_NO].ToString() == gridItem[PROPKEY_ARRIVAL_NO].ToString() &&
                            retItem[PROPKEY_DETAIL_NO].ToString() == gridItem[PROPKEY_DETAIL_NO].ToString())
                        {
                            bnExists = true;
                            break;
                        }

                    }

                    if (!bnExists)
                    {
                        // 存在しない場合のみ追加
                        Dictionary<string, object> newRow = new();
                        foreach (ComponentColumnsInfo infos in _gridColumns)
                        {
                            if (retItem.TryGetValue(infos.Property, out object? value))
                            {
                                newRow.Add(infos.Property, value);
                            }
                            else
                            {
                                newRow.Add(infos.Property, string.Empty);
                            }
                        }
                        _gridData.Add(newRow);
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
        /// F6ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task メソッド実行2(ComponentProgramInfo info)
        {
            try
            {
                //権限チェック
                if (!await CheckAuthorities())
                {
                    return;
                }
                // 検索実行
                await ProcSearchAsync();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F9ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task メソッド実行3(ComponentProgramInfo info)
        {
            try
            {
                //権限チェック
                if (!await CheckAuthorities())
                {
                    return;
                }
                // チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"入荷受付を取消する対象が選択されていません。", pageName);
                    return;
                }
                // 在庫計上数を確認して、パレット紐付まで実績が存在する場合は取消不可とする //TODO できれば固定文字で特定したくはない
                if (_gridSelectedData!.Any(_ =>
                {
                    return int.TryParse(_[PROPKEY_STOCKUP_CASE_QTY]?.ToString(), out int caseQty) &&
                        int.TryParse(_[PROPKEY_STOCKUP_BARA_QTY]?.ToString(), out int baraQty)
                        && caseQty + baraQty > 0;
                }))
                {
                    await ComService.DialogShowOK($"パレット紐付済数が存在する入荷予定は取消できません。", pageName);
                    return;
                }

                // 確認
                bool? ret = await ComService.DialogShowYesNo("選択行を取消しますか？", pageName);
                if (true != ret)
                {
                    return;
                }

                // 選択行を削除
                foreach (IDictionary<string, object> selectItem in _gridSelectedData!)
                {
                    for (int i = 0; i < _gridData.Count(); i++)
                    {
                        IDictionary<string, object> item = _gridData[i];

                        if (selectItem[PROPKEY_ARRIVAL_NO].ToString() == item[PROPKEY_ARRIVAL_NO].ToString() &&
                            selectItem[PROPKEY_DETAIL_NO].ToString() == item[PROPKEY_DETAIL_NO].ToString())
                        {
                            _gridData.RemoveAt(i);
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
                base.ClearSearchCondition();

                // グリッドクリア
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();

                // 選択データクリア
                _gridSelectedData = null;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 確定前チェック処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            // 受付Noがセットされているか
            if (compReceptionNo?.CompObj?.Instance is CompTextBox compTextBox)
            {
                if (string.IsNullOrEmpty(compTextBox.InputValue))
                {
                    await ComService.DialogShowOK($"{PROPKEY_RECEPTION_NO}が選択されていません。", pageName);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            try
            {
                _storedData = new Dictionary<string, object>();

                // 受付No
                {
                    if (compReceptionNo?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        _storedData[compReceptionNo.TitleLabel] = compTextBox.InputValue;
                    }
                }
                // 車番
                {
                    if (compCarNumber?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        _storedData[compCarNumber.TitleLabel] = compTextBox.InputValue;
                    }
                }
                // トラック区分
                {
                    if (compTruckTypes?.CompObj?.Instance is CompDropDown compDropDown)
                    {
                        _storedData[compTruckTypes.TitleLabel] = compDropDown.InputValue;
                    }
                }
                // 積荷区分
                {
                    if (compCargoTypes?.CompObj?.Instance is CompDropDown compDropDown)
                    {
                        _storedData[compCargoTypes.TitleLabel] = compDropDown.InputValue;
                    }
                }
                // 携帯番号
                {
                    if (compPhoneNumber?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        _storedData[compPhoneNumber.TitleLabel] = compTextBox.InputValue;
                    }
                }
                // 備考
                {
                    if (compRemarks?.CompObj?.Instance is CompTextArea compTextArea)
                    {
                        _storedData[compRemarks.TitleLabel] = compTextArea.InputValue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ストアドデータ設定_テーブルデータ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();
                if (_gridData is not null)
                {
                    foreach (IDictionary<string, object> rows in _gridData)
                    {
                        Dictionary<string, object> rowdata = new();
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            rowdata[data.Key] = data.Value;
                        }
                        _storedTableData.Add(rowdata);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        #endregion

        #region private

        /// <summary>
        /// コンポーネントのプロパティ設定
        /// </summary>
        private void SetCompProperties(string strReceptionNo)
        {
            // 各コンポーネントのプロパティ設定
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    switch (item.TitleLabel)
                    {
                        case PROPKEY_CAR_NUMBER:
                            // 車番
                            compCarNumber = item;
                            break;
                        case PROPKEY_TRUCK_TYPES:
                            // トラック区分
                            compTruckTypes = item;
                            break;
                        case PROPKEY_CARGO_TYPES:
                            // 積荷区分
                            compCargoTypes = item;
                            break;
                        case PROPKEY_PHONE_NUMBER:
                            // 携帯番号
                            compPhoneNumber = item;
                            break;
                        case PROPKEY_RECEPTION_NO:
                            // 受付No
                            compReceptionNo = item;
                            if (item?.CompObj?.Instance is CompTextBox txtReceptionNo)
                            {
                                txtReceptionNo.Disabled = true;
                                txtReceptionNo.InputValue = strReceptionNo;
                                txtReceptionNo.Refresh();
                            }
                            break;
                        case PROPKEY_REMARKS:
                            // 備考
                            compRemarks = item;
                            if (item?.CompObj?.Instance is CompTextArea txaRemarks)
                            {
                                txaRemarks.Cols = 60;
                                txaRemarks.Rows = 3;
                                txaRemarks.Refresh();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        private async Task ProcSearchAsync()
        {
            try
            {
                // 検索条件生成
                ClassNameSelect custom = new()
                {
                    className = GetType().Name,
                };
                // 車番
                {
                    if (compCarNumber?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        if (!string.IsNullOrEmpty(compTextBox.InputValue))
                        {
                            custom.whereParam.Add(compCarNumber.TitleLabel, new WhereParam { val = compTextBox.InputValue, whereType = enumWhereType.LikeStart });
                        }
                    }
                }
                // トラック区分
                {
                    if (compTruckTypes?.CompObj?.Instance is CompDropDown compDropDown)
                    {
                        if (!string.IsNullOrEmpty(compDropDown.InputValue))
                        {
                            custom.whereParam.Add(compTruckTypes.TitleLabel, new WhereParam { val = compDropDown.InputValue, whereType = enumWhereType.Equal });
                        }
                    }
                }
                // 積荷区分
                {
                    if (compCargoTypes?.CompObj?.Instance is CompDropDown compDropDown)
                    {
                        if (!string.IsNullOrEmpty(compDropDown.InputValue))
                        {
                            custom.whereParam.Add(compCargoTypes.TitleLabel, new WhereParam { val = compDropDown.InputValue, whereType = enumWhereType.Equal });
                        }
                    }
                }
                // 携帯番号
                {
                    if (compPhoneNumber?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        if (!string.IsNullOrEmpty(compTextBox.InputValue))
                        {
                            custom.whereParam.Add(compPhoneNumber.TitleLabel, new WhereParam { val = compTextBox.InputValue, whereType = enumWhereType.LikeStart });
                        }
                    }
                }
                // 受付No
                {
                    if (compReceptionNo?.CompObj?.Instance is CompTextBox compTextBox)
                    {
                        if (!string.IsNullOrEmpty(compTextBox.InputValue))
                        {
                            custom.whereParam.Add(compReceptionNo.TitleLabel, new WhereParam { val = compTextBox.InputValue, whereType = enumWhereType.Equal });
                        }
                    }
                }

                // OrderBy設定
                custom.orderByParam = OrderByParamGet();

                // 検索結果取得
                List<IDictionary<string, object>> searchGridData = await ComService!.GetSelectGridData(_gridColumns, custom);

                if (searchGridData.Count() > 0)
                {
                    // 受付Noが複数あるかチェック
                    List<string> lstReceptionNo = new();
                    foreach (IDictionary<string, object> item in searchGridData)
                    {
                        string value = string.Empty;
                        if (item.ContainsKey(PROPKEY_RECEPTION_NO))
                        {
                            value = item[PROPKEY_RECEPTION_NO].ToString();
                        }
                        if (!string.IsNullOrEmpty(value) && !lstReceptionNo.Contains(value))
                        {
                            lstReceptionNo.Add(value);
                        }
                    }

                    string strReceptionNo = string.Empty;
                    if (lstReceptionNo.Count() > 1)
                    {
                        // 受付Noが複数ある場合

                        // ダイアログ情報を取得
                        Dictionary<string, object> attr = new(GetAttributes("AttributesDialogArrivalsReceptionNoSelect"));
                        string strDialogTitle = "受付No選択";
                        int intDialogWidth = 400;
                        int intDialogHeight = 700;
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
                        dynamic ret = await DialogService.OpenAsync<DialogArrivalsReceptionNoSelect>(
                            $"{strDialogTitle}",
                            new Dictionary<string, object>() { { "LstReceptionNo", lstReceptionNo.OrderBy(_ => _).ToList() } },
                            new DialogOptions()
                            {
                                Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                                Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                                Resizable = true,
                                Draggable = true
                            }
                        );

                        if (ret == null)
                        {
                            // キャンセル
                            return;
                        }
                        strReceptionNo = ret;
                    }
                    else
                    {
                        // 受付Noが一つの場合
                        strReceptionNo = lstReceptionNo[0];
                    }

                    // 検索結果を受付Noで絞り込み
                    for (int i = searchGridData.Count() - 1; i >= 0; i--)
                    {
                        if (strReceptionNo != searchGridData[i][PROPKEY_RECEPTION_NO].ToString())
                        {
                            searchGridData.RemoveAt(i);
                        }
                    }

                    // 選択された受付Noのデータを入力条件にセットする
                    if (searchGridData.Count() > 0)
                    {
                        // 受付No
                        {
                            if (compReceptionNo?.CompObj?.Instance is CompTextBox compTextBox)
                            {
                                compTextBox.InputValue = strReceptionNo;
                                compTextBox.Refresh();
                            }
                        }
                        // 車番
                        {
                            if (compCarNumber?.CompObj?.Instance is CompTextBox compTextBox)
                            {
                                compTextBox.InputValue = searchGridData[0][PROPKEY_CAR_NUMBER].ToString();
                                compTextBox.Refresh();
                            }
                        }
                        // トラック区分
                        {
                            if (compTruckTypes?.CompObj?.Instance is CompDropDown compDropDown)
                            {
                                compDropDown.InputValue = searchGridData[0][PROPKEY_TRUCK_TYPES].ToString();
                                compDropDown.Refresh();
                            }
                        }
                        // 積荷区分
                        {
                            if (compCargoTypes?.CompObj?.Instance is CompDropDown compDropDown)
                            {
                                compDropDown.InputValue = searchGridData[0][PROPKEY_CARGO_TYPES].ToString();
                                compDropDown.Refresh();
                            }
                        }
                        // 携帯番号
                        {
                            if (compPhoneNumber?.CompObj?.Instance is CompTextBox compTextBox)
                            {
                                compTextBox.InputValue = searchGridData[0][PROPKEY_PHONE_NUMBER].ToString();
                                compTextBox.Refresh();
                            }
                        }
                        // 備考
                        {
                            if (compRemarks?.CompObj?.Instance is CompTextArea compTextArea)
                            {
                                compTextArea.InputValue = searchGridData[0][PROPKEY_REMARKS].ToString();
                                compTextArea.Refresh();
                            }
                        }
                    }
                }

                // グリッドデータ設定
                Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>(searchGridData);

                // 選択データクリア
                _gridSelectedData = null;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                if ("受付状態" == args.Column.Title)
                {
                    // 入荷状態の背景色変更
                    if (args.Data.TryGetValue("受付状態区分", out object? value))
                    {
                        ComService.AddAttrArrivalStatus(value?.ToString(), args.Attributes);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}