using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// AGV状況照会
/// </summary>
public partial class AGVTaskView : ChildPageBasePC
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
            new TabItemInfo() { Title = "原料供給", TabItem = new TabItemAGVTaskMaterialSupply() },
            new TabItemInfo() { Title = "専用容器", TabItem = new TabItemAGVTaskMaterialInput() },
            new TabItemInfo() { Title = "タスク一覧", TabItem = new TabItemAGVTaskList() },
            new TabItemInfo() { Title = "タスク履歴", TabItem = new TabItemAGVTaskHistory() },
            new TabItemInfo() { Title = "ST状況", TabItem = new TabItemAGVStation() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
        //pageName = "在庫集計";
        //base.OnUpdateParentPageTitle(pageName);
        //await base.OnInitializedAsync();
    }
}