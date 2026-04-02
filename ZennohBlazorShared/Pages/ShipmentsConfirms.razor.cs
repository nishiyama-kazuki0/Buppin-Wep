using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出荷欠品確定
    /// </summary>
    public partial class ShipmentsConfirms : ChildPageBasePC
    {
        #region override

        /// <summary>
        /// F5ボタンクリックイベント
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
                // 選択行チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"出荷欠品を確定させる対象が選択されていません。", pageName);
                    return;
                }

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["DetailData"] = new List<IDictionary<string, object>>(_gridSelectedData)
                };

                // ダイアログ情報を取得
                Dictionary<string, object> attr = new(GetAttributes("AttributesConfirmDialog"));
                string strDialogTitle = "出荷欠品確定";
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
                dynamic ret = await DialogService.OpenAsync<DialogShipmentsConfirmContent>(
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