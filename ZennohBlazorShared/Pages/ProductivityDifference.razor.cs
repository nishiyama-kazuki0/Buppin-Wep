using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 作業実績管理一覧
    /// </summary>
    public partial class ProductivityDifference : ChildPageBasePC
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
            List<TabItemInfo> list = new()
            {
                new TabItemInfo() { Title = "差異一覧", TabItem = new TabItemProductivityDifferenceList() },
                new TabItemInfo() { Title = "時間帯別", TabItem = new TabItemProductivityDifferenceTimeZone() },
                new TabItemInfo() { Title = "仮置場残作業一覧", TabItem = new TabItemProductivityDifferenceWorkList() },
            };
            TabsExtendAttributes.Add("TabItems", list);

            await Task.Delay(0);
        }
    }
}