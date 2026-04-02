using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出荷個別引当
    /// </summary>
    public partial class ShipmentsSeparateAllocate : ChildPageBasePC
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
                    await ComService.DialogShowOK($"出荷の引当を行う対象が選択されていません。", pageName);
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
                Dictionary<string, object> attr = new(GetAttributes("AttributesDialogShipmentsManualAllocateControl"));
                string strDialogTitle = "在庫選択引当設定";
                int intDialogWidth = 1000;
                int intDialogHeight = 724;
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

                // 在庫選択引当設定ダイアログ
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic ret = await DialogService.OpenAsync<DialogShipmentsManualAllocateControl>(
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
                else if ("出荷状態名" == args.Column.Title)
                {
                    // 出荷状態の背景色変更
                    if (args.Data.TryGetValue("出荷状態", out object? value))
                    {
                        ComService.AddAttrShipmentStatus(value?.ToString(), args.Attributes);
                    }
                }
                else if ("確定状態名" == args.Column.Title)
                {
                    // 確定状態の背景色変更
                    if (args.Data.TryGetValue("確定状態", out object? value))
                    {
                        ComService.AddAttrConfirmStatus(value?.ToString(), args.Attributes);
                    }
                }
                else if ("実績送信状態名" == args.Column.Title)
                {
                    // 実績送信状態の背景色変更
                    if (args.Data.TryGetValue("実績送信状態", out object? value))
                    {
                        ComService.AddAttrSendResultsStatus(value?.ToString(), args.Attributes);
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