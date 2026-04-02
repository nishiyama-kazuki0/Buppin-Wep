using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 切出搬送
    /// </summary>
    public partial class MoveComplete : ChildPageBaseMobile
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
            StepItemMoveCompleteViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemMoveCompleteSearch).Name) ||
                    model.LastRireki.Equals(typeof(StepItemMoveCompleteSave).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // 切出搬送/切出先入力（他画面から戻ってきた）
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    if (!string.IsNullOrEmpty(model.PalletNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                }
                else if (model.LastRireki.Equals(typeof(StepItemPickingPalletPick).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingPalletByDeliveryPick).Name))
                {
                    // パレットピッキング【倉庫別】/ピック確定
                    // パレットピッキング【倉庫配送先別】/ピック確定
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
                else if (model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryPallet).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryProduct).Name) ||
                    model.LastRireki.Equals(typeof(StepItemPickingItemByDeliveryPick).Name))
                {
                    // 摘取ピック【倉庫配送先別】
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    if (!string.IsNullOrEmpty(model.PalletNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                }
                else if (model.LastRireki.Equals(typeof(StepItemMovePalletInput).Name))
                {
                    // パレット移動/移動先入力
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemMoveCompleteSearch() },
                new StepItemInfo() { Title = "切出搬送確定", StepItem = new StepItemMoveCompleteSave() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}