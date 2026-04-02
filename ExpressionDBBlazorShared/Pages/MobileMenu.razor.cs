using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using SharedModels;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// HTメニュー
/// </summary>
public partial class MobileMenu : ChildPageBaseMobile
{
    /// <summary>
    /// 初期処理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        // HTメニューのトップ情報取得
        MenuInfo infoHtTop = await ComService.GetMenuInfoHtTopAsync();
        if (infoHtTop != null)
        {
            pageName = infoHtTop.menuName;
            MenuId = infoHtTop.menuId;
        }

        OnUpdateParentPageTitle(pageName);
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// HTスキャン処理
    /// 西山
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