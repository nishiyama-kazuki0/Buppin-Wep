using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 原料投入状況
/// </summary>
public partial class MaterialInputEquipTaskView : ChildPageBasePC
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
            new TabItemInfo() { Title = "投入1", TabItem = new TabItemMaterialInputEquip1() },
            new TabItemInfo() { Title = "投入2", TabItem = new TabItemMaterialInputEquip2() },
            new TabItemInfo() { Title = "洗浄乾燥機", TabItem = new TabItemMaterialInputEquipCleaningDryer() },
        ];
        TabsExtendAttributes.Add("TabItems", list);

        await Task.Delay(0);
        //pageName = "在庫集計";
        //base.OnUpdateParentPageTitle(pageName);
        //await base.OnInitializedAsync();
    }
}