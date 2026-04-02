using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷予実照会
    /// </summary>
    public partial class Arrivals : ChildPageBasePC
    {
        #region override

        /// <summary>
        /// F6ボタンクリックイベント
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
                // チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"入荷情報を修正する対象が選択されていません。", pageName);
                    return;
                }

                // 選択行取得
                Dictionary<string, object> row = new(_gridSelectedData[0]);

                // WMS単独管理有無をチェック
                int intWms = 0;
                if (row.TryGetValue("WMS単独管理有無", out object? value))
                {
                    _ = int.TryParse(value.ToString(), out intWms);
                }
                if (1 != intWms)
                {
                    await ComService.DialogShowOK($"上位より受信した入荷予定のため修正できません。", pageName);
                    return;
                }

                // 選択行の入荷No取得
                string strArrivalNo = string.Empty;
                if (_gridSelectedData[0].TryGetValue("入荷No", out value))
                {
                    strArrivalNo = value.ToString();
                }

                // LocalStorage設定
                await LocalStorage.SetItemAsStringAsync(ArrivalsDetailsAddMainte.STORAGEKEY_ARRIVAL_NO, strArrivalNo);

                // 画面遷移
                NavigationManager.NavigateTo("arrivals_details_add_mainte");
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
        public override async Task メソッド実行2(ComponentProgramInfo info)
        {
            try
            {
                //権限チェック
                if (!await CheckAuthorities())
                {
                    return;
                }
                // LocalStorageクリア
                await LocalStorage.RemoveItemAsync(ArrivalsDetailsAddMainte.STORAGEKEY_ARRIVAL_NO);

                // 画面遷移
                NavigationManager.NavigateTo("arrivals_details_add_mainte");
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
                    await ComService.DialogShowOK($"外部倉庫に入庫する対象が選択されていません。", pageName);
                    return;
                }

                // 選択行データ取得
                IDictionary<string, object> initialData = (_gridSelectedData is not null && _gridSelectedData.Count > 0) ? _gridSelectedData[0] : new Dictionary<string, object>();

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["InitialData"] = initialData
                };

                // ダイアログ情報を取得
                Dictionary<string, object> attr = new(GetAttributes("AttributesDialogArrivalsSatelliteStockupContent"));
                string strDialogTitle = "外部倉庫入庫";
                int intDialogWidth = 1280;
                int intDialogHeight = 791;
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

                //外部倉庫入庫ダイアログ
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic ret = await DialogService.OpenAsync<DialogArrivalsSatelliteStockupContent>(
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

                await グリッド更新(new Data.ComponentProgramInfo() { ComponentName = STR_ATTRIBUTE_GRID });
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 画面遷移用のパラメータをセットする
        /// </summary>
        /// <param name="info"></param>
        public override async Task SetNavigateParam(ComponentProgramInfo info)
        {
            if (Attributes.ContainsKey(info.ComponentName))
            {
                // StorageのKeyを取得する
                string storageKey = string.Empty;
                foreach (KeyValuePair<string, object> keyval in Attributes[info.ComponentName])
                {
                    storageKey = keyval.Value.ToString();
                    break;
                }

                // StorageのKeyが有れば
                if (!string.IsNullOrEmpty(storageKey))
                {
                    // PC画面遷移用パラメータクリア
                    _ = ComService.ClearPCTransParam(storageKey);

                    // グリッドに選択行があれば、PC画面間パラメータクセット
                    if (_gridSelectedData != null && _gridSelectedData?.Count() > 0)
                    {
                        // 入荷状態はパラメータとして渡さない
                        IDictionary<string, object> param = new Dictionary<string, object>(_gridSelectedData[0]);
                        if (param.ContainsKey("入荷状態"))
                        {
                            _ = param.Remove("入荷状態");
                        }
                        _ = ComService.SetPCTransParam(storageKey, param);
                    }
                }
            }
            await Task.Delay(0);
        }

        #endregion

        #region private

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                if ("入荷状態名" == args.Column.Title)
                {
                    // 入荷状態の背景色変更
                    if (args.Data.TryGetValue("入荷状態", out object? value))
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