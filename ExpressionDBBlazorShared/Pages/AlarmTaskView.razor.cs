using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// アラーム通知
/// </summary>
public partial class AlarmTaskView : ChildPageBasePC
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
            new TabItemInfo() { Title = "アラーム通知", TabItem = new TabItemAlarmTaskList() },
            new TabItemInfo() { Title = "アラーム内容", TabItem = new TabItemAlarmContent() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
        //pageName = "在庫集計";
        //base.OnUpdateParentPageTitle(pageName);
        //await base.OnInitializedAsync();
    }
}