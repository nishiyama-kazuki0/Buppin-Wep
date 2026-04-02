using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー搬送
    /// </summary>
    public partial class MoveCompleteCorner : ChildPageBaseMobile
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
            StepItemMoveCompleteCornerViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemMoveCompleteCornerSave).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // コーナー搬送/コーナー搬送完了（他画面から戻ってきた）
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
                else if (model.LastRireki.Equals(typeof(StepItemMoveCompleteSearch).Name) ||
                    model.LastRireki.Equals(typeof(StepItemMoveCompleteSave).Name) ||
                    model.LastRireki.Equals(typeof(StepItemSortingByCornersInput).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryPallet).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryProduct).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryPick).Name)
                    )
                {
                    // 切出搬送/パレットNo読取、切出搬送/切出先入力
                    // コーナー別仕分/仕分数入力
                    // 摘取ピッキング st1-3/
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    if (!string.IsNullOrEmpty(model.PalletNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "ｺｰﾅｰﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemMoveCompleteCornerSearch() },
                new StepItemInfo() { Title = "ｺｰﾅｰ搬送確定", StepItem = new StepItemMoveCompleteCornerSave() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}