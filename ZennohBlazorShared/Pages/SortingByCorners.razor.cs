using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー別仕分
    /// </summary>
    public partial class SortingByCorners : ChildPageBaseMobile
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
        /// 初期処理
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            StepItemSortingByCornersViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemSortingByCornersInput).Name) ||
                    model.LastRireki.Equals(typeof(StepItemSortingByCornersSave).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // コーナー別仕分/仕分数入力、コーナー別仕分/コーナー仕分確定（他画面から戻ってきた）
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
                else if (model.LastRireki.Equals(typeof(StepItemMoveCompleteSave).Name))
                {
                    // 切出搬送/切出先入力
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "切出済ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemSortingByCornersSearch() },
                new StepItemInfo() { Title = "ﾋﾟｯｸ済在庫入力", StepItem = new StepItemSortingByCornersInput() },
                new StepItemInfo() { Title = "ﾋﾟｯｸ仕分確定", StepItem = new StepItemSortingByCornersSave() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}