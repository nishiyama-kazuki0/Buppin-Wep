using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 自動倉庫状況照会
/// </summary>
public partial class AutoWarehouseTaskView : ChildPageBasePC
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
            //トラッキングテーブルがないので一旦コメント(2024/12/16)
            //new TabItemInfo() { Title = "1F共通", TabItem = new TabItem1FShareConveyorTask() },
            //new TabItemInfo() { Title = "5F常温", TabItem = new TabItem5FDryConveyorTask() },
            //new TabItemInfo() { Title = "5F冷凍", TabItem = new TabItem5FFrozenConveyorTask() },
            new TabItemInfo() { Title = "自動倉庫(常温)", TabItem = new TabItemAutoWarehouseDry() },
            new TabItemInfo() { Title = "自動倉庫(冷凍)", TabItem = new TabItemAutoWarehouseFrozen() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
        //pageName = "在庫集計";
        //base.OnUpdateParentPageTitle(pageName);
        //await base.OnInitializedAsync();
    }
}