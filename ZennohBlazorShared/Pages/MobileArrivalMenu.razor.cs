using SharedModels;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入庫メニュー
    /// </summary>
    public partial class MobileArrivalMenu : ChildPageBaseMobile
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // LocalStorage設定
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_メニュー遷移, ClassName);

            // 入庫で選択した車番、入荷明細No、入荷Noをクリアする
            await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_CARNUMBER);
            await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
            await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_NO);
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            await Task.Delay(0);

            // パレットNoの読み取り。履歴は残さず、読み取ったパレットNowo セットしてパレット照会画面に遷移。戻りは在庫の在庫メニューの在庫とする
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                // await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, value);//読み取りはパレットNoのみ
                // パレット照会画面に遷移
                NavigationManager.NavigateTo("pallet_inventory_inquiry");
            }
        }
    }
}