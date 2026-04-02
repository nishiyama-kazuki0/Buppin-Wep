using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// マニュアル出庫
    /// </summary>
    public partial class ShipmentsManualControl : ChildPageBasePC
    {
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
                // チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"出荷内容を設定する対象が選択されていません。", pageName);
                    return;
                }

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["GridColumns"] = new List<Data.ComponentColumnsInfo>(_gridColumns),
                    ["GridData"] = new List<IDictionary<string, object>>(_gridSelectedData)
                };

                // ダイアログ情報を取得
                Dictionary<string, object> attr = new(GetAttributes("AttributesDialogShipmentsManualControl"));
                string strDialogTitle = "マニュアル出庫設定";
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

                // マニュアル出庫設定ダイアログ
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic ret = await DialogService.OpenAsync<DialogShipmentsManualControl>(
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
    }
}