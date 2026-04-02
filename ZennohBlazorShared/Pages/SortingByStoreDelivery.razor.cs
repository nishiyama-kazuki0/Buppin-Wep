using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【摘取】
    /// </summary>
    public partial class SortingByStoreDelivery : ChildPageBaseMobile
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

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            StepItemSortingByStoreDeliveryViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model!.IsRireki)
            {

            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "納品先選択", StepItem = new StepItemSortingByStoreDeliverySelect() },
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemSortingByStoreDeliveryPallet() },
                new StepItemInfo() { Title = "品名選択", StepItem = new StepItemSortingByStoreDeliveryProduct() },
                new StepItemInfo() { Title = "仕分入力", StepItem = new StepItemSortingByStoreDeliverySave() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}