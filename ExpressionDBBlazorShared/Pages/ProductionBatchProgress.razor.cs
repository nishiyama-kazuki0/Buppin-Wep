using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// バッチ進捗照会
/// </summary>
public partial class ProductionBatchProgress : ChildPageBasePC
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
            new TabItemInfo() { Title = "バッチ進捗", TabItem = new TabItemProductionBatchProgress1() },
            new TabItemInfo() { Title = "原料進捗", TabItem = new TabItemProductionBatchProgress2() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
    }
}

