using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using SharedModels;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 在庫メニュー
/// </summary>
public partial class MobileInventoryContorolMenu : ChildPageBaseMobile
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // LocalStorage設定
        await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_メニュー遷移, ClassName);

        // 在庫メニュー関連で使用した入荷明細Noをクリアする（現在パレット分割のStep2で使用）
        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_STOCK_ARRIVAL_DETAIL_NO);
    }

    /// <summary>
    /// HTスキャン処理
    /// </summary>
    /// <param name="scanData"></param>
    protected override async Task HtService_HtScanEvent(ScanData scanData)
    {
        await Task.Delay(0);

        // パレットNoの読み取り。履歴は残さず、読み取ったパレットNowo セットしてパレット照会画面に遷移。戻りは在庫の在庫メニューの在庫とする
        string value = scanData.strStringData;//読み取った文字列を取得する

        if (IsPalletBarcode(value))
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            // await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, value);//読み取りはパレットNoのみ
            // パレット照会画面に遷移
            //下記のURLに移動
            NavigationManager.NavigateTo("pallet_inventory_inquiry");
        }
    }

}