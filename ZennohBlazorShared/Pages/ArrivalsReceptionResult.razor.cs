using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷受付一覧
    /// </summary>
    public partial class ArrivalsReceptionResult : ChildPageBasePC
    {
        /// <summary>
        /// F4ボタンクリックイベント
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
                    await ComService.DialogShowOK($"入荷受付を修正する対象が選択されていません。", pageName);
                    return;
                }

                // 選択行の受付No取得
                string strReceptionNo = string.Empty;
                if (_gridSelectedData[0].TryGetValue("受付No", out object value))
                {
                    strReceptionNo = value.ToString();
                }

                // LocalStorage設定
                await LocalStorage.SetItemAsStringAsync(ArrivalsReception.STORAGEKEY_RECEPTION_NO, strReceptionNo);

                // 画面遷移
                NavigationManager.NavigateTo("arrivals_reception");
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }
    }
}