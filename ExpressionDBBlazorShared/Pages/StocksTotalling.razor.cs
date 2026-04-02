using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 在庫集計
/// </summary>
public partial class StocksTotalling : ChildPageBasePC
{
    /// <summary>
    /// コンポーネントが初期化されるときに呼び出されます。
    /// 子ページで全体で使用したい処理を記載
    /// </summary>
    protected override void OnInitialized()
    {
        // キーダウンイベントを受けるイベントの追加は行わない
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected override void Dispose()
    {
        // キーダウンイベントを受けるイベントの削除は行わない
    }

    protected override async Task OnInitializedAsync()
    {
        // TabsExtendにタブ画面を追加する
        List<TabItemInfo> list =
        [
            new TabItemInfo() { Title = "品名別", TabItem = new TabItemStocksTotallingProduct() },
            new TabItemInfo() { Title = "入荷No別", TabItem = new TabItemStocksTotallingNyukaNo() },
            new TabItemInfo() { Title = "パレットNo別", TabItem = new TabItemStocksTotallingPalletNo() },
            new TabItemInfo() { Title = "ゾーン別", TabItem = new TabItemStocksTotallingZone() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
        //pageName = "在庫集計";
        //base.OnUpdateParentPageTitle(pageName);
        //await base.OnInitializedAsync();
    }
}