using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 出荷進捗照会
/// </summary>
public partial class ShipmentsProgress : ChildPageBasePC
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
            new TabItemInfo() { Title = "倉庫配送先別", TabItem = new TabItemShipmentsProgressDeliveries() },
            new TabItemInfo() { Title = "倉庫別", TabItem = new TabItemShipmentsProgressAreas() },
            new TabItemInfo() { Title = "コーナー別", TabItem = new TabItemShipmentsProgressCorners() },

        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
    }
}

