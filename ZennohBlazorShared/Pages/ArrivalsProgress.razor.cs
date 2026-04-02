using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷進捗照会
    /// </summary>
    public partial class ArrivalsProgress : ChildPageBasePC
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
                new TabItemInfo() { Title = "出荷者別", TabItem = new TabItemArrivalsProgressSuppliers() },
                new TabItemInfo() { Title = "品名別", TabItem = new TabItemArrivalsProgressProducts() },
            };
            TabsExtendAttributes.Add("TabItems", list);

            await Task.Delay(0);
        }
    }
}