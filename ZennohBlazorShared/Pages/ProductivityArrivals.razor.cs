using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    public partial class ProductivityArrivals : ChildPageBasePC
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
                new TabItemInfo() { Title = "作業者別", TabItem = new TabItemProductivityArrivalsPersons() },
                new TabItemInfo() { Title = "作業者・ゾーン別", TabItem = new TabItemProductivityArrivalsPersonsZone() },
                new TabItemInfo() { Title = "時間帯別", TabItem = new TabItemProductivityArrivalsTimeZone() },
            };
            TabsExtendAttributes.Add("TabItems", list);

            await Task.Delay(0);
        }
    }
}