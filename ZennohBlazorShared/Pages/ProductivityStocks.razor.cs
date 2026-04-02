using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
	/// <summary>
	/// 在庫移動実績管理
	/// </summary>
	public partial class ProductivityStocks : ChildPageBasePC
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
                new TabItemInfo() { Title = "作業者別", TabItem = new TabItemProductivityStocksPersons() },
                new TabItemInfo() { Title = "ゾーン別", TabItem = new TabItemProductivityStocksZone() },
            };
            TabsExtendAttributes.Add("TabItems", list);

            await Task.Delay(0);
        }
    }
}